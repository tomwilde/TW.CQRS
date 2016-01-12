using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Castle.Core.Interceptor;
using Ionic.Zip;
using TW.Commons.Interfaces;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    public class MockDataReplayInterceptor<T> : IInterceptor, IDisposable
    {
        private readonly ILogger _logger;

        private readonly string _storagePath;
        private readonly bool _usePreCache;
        private readonly List<int> _offsets;
        private int _currentOffset;

        private readonly BinaryFormatter serialiser = new BinaryFormatter();
        readonly Dictionary<string, dynamic> overrides = new Dictionary<string, dynamic>();

        private int _callCount;
        private int _transactionLogLength;
        private readonly string _name;
        private readonly string _replayFilePath;

        private Stream _stream;
        private readonly byte[] _delimiter;

        public MockDataReplayInterceptor(ILoggerFactory loggerFactory, string storagePath, bool preCache = true, int? id = null)
        {
            _name = typeof(T).Name;
            _logger = loggerFactory.Create();

            _logger.InfoFormat(id == null 
                ? string.Format("Replaying<{0}>", typeof(T))
                : string.Format("Replaying<{0}> ({1})", typeof(T), id));

            if (!Path.IsPathRooted(storagePath))
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                var root = entryAssembly == null ?  Environment.CurrentDirectory : entryAssembly.Location;
                storagePath = Path.Combine(root, storagePath);
            }

            _storagePath = storagePath;
            _usePreCache = preCache;

            _replayFilePath = Path.Combine(storagePath, id == null 
                ? string.Format("{0}.capture", _name)
                : string.Format("{0}_{1}.capture", _name, id));
            
            if (File.Exists(string.Format("{0}.zip", _replayFilePath)))
            {
                _logger.InfoFormat("Using zipped data...");
                _replayFilePath = string.Format("{0}.zip", _replayFilePath);
            }
            EnsureDirectoryExists(_storagePath);

            _delimiter = Encoding.ASCII.GetBytes(MockDataCaptureInterceptor.DelimiterText);
            
            if (File.Exists(_replayFilePath))
                _logger.InfoFormat(id == null 
                    ? string.Format("Ready to replay for service: {0}", typeof(T))
                    : string.Format("Ready to replay for service: {0} ({1})", typeof(T), id));
            else
                throw new Exception(string.Format("Nothing to replay file missing: {0}", _replayFilePath));
            _offsets = BuildOffsetIndex();
        }

        private List<int> BuildOffsetIndex()
        {
            _logger.InfoFormat("Building index into transaction log.");

            EnsureOpenStream();

            _transactionLogLength = (int)_stream.Length;
            var offsets = new List<int>() { 0 };
            var buffer = new byte[_stream.Length];
            
            var br = new BinaryReader(_stream);
            
            _logger.InfoFormat("  loading...");
            br.Read(buffer, 0, (int) _stream.Length);

            offsets.AddRange(DetermineOffsets(buffer));
                           
            if (_usePreCache)
            {
                _logger.InfoFormat("Precaching replay stream in memory for speed.");
            }
            else
            {
                _stream.Close();
                _stream = null;
                br.Dispose();
            }

            _logger.InfoFormat("[Done]");

            return offsets;
        }
        public void Intercept(IInvocation invocation)
        {
            _callCount++;
            //_logger.InfoFormat("Intercepting svc call cc:{0} id:{1} {2}::{3}", _callCount, typesBeingReplayed[_name], typeof(T), invocation.MethodInvocationTarget.Name);

            EnsureOpenStream();

            if (overrides.ContainsKey(invocation.Method.Name))
            {
                // _logger.InfoFormat("Overriding playback of: {0}", invocation.Method.Name);
                invocation.ReturnValue = overrides[invocation.Method.Name];
                return;
            }

            // Parameters should match before we assess a call, this allows us to take an imprecise capture and replay it, eg: if we record a little earlier this will skip until it matches.
            Tuple<bool, string> result;
            do
            {
                Parameters parameters = null;
                do
                {
                    parameters = GetNextParameterSet();
                } while (parameters == null);

                result = VerifyArgumentsMatch(parameters, invocation);
                if (!result.Item1) _logger.WarnFormat("[Skipping] {0}", result.Item2);

            } while(!result.Item1);

            var returnData = GetNextReturnDataSet();

            VerifyReturnObject(invocation.MethodInvocationTarget.ReturnParameter, returnData);

            invocation.ReturnValue = returnData.Data;
        }

        private void EnsureOpenStream()
        {
            if(_stream == null)
            {
                if (_replayFilePath.EndsWith(".zip"))
                {
                    var zipFile = new ZipFile(_replayFilePath);
                    _stream = new MemoryStream();
                    zipFile[0].Extract(_stream);
                    _stream.Seek(0, 0); // Must reset the position of the stream back to the beginning
                }
                else
                    _stream = new FileStream(_replayFilePath, FileMode.Open,FileAccess.Read, FileShare.Read);
            }
        }

        private void VerifyReturnObject(ParameterInfo returnValue, ReturnValue returnData)
        {
            try
            {
                var officialType = returnValue.ParameterType.IsGenericType 
                                        ? GetReadableTypeName(returnValue.ParameterType)
                                        : returnValue.ParameterType.Name;

                if (returnData.Data == null)
                {
                    return;
                }


                if (!officialType.Equals(returnData.Type))
                    throw new Exception(string.Format("Return value officialType/deserialisedType mismatch: {0}/{1}",
                                                      officialType, returnData.Type));

                // _logger.Info("(Return object validated)");
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private ReturnValue GetNextReturnDataSet()
        {
            using (var ms = new MemoryStream(GetNextItemData()))
            {
                return serialiser.Deserialize(ms) as ReturnValue;
            }
        }

        private Parameters GetNextParameterSet()
        {
            using (var ms = new MemoryStream(GetNextItemData()))
            {
                return serialiser.Deserialize(ms) as Parameters;
            }
        }

        private byte[] GetNextItemData()
        {
            var x=0;
            try
            {
                var start = _currentOffset == 0 ? 0 : _offsets[_currentOffset];
                var end = (_currentOffset+1 == _offsets.Count) ? _transactionLogLength : _offsets[_currentOffset+1];

                _currentOffset++;

                _stream.Seek(start, SeekOrigin.Begin);

                var l = end - start;
                var buffer = new byte[l];

                _stream.Read(buffer,0,l);
                return buffer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<int> DetermineOffsets(byte[] data)
        {
            var offsets = new List<int>();
            
            var length = _delimiter.Length;
            var j=0;

            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] == _delimiter[j])
                {
                    j++;
                }
                else
                {
                    j=0;
                }

                if (j == length)
                {
                    offsets.Add(i+1);    
                    j=0;
                }
            }

            return offsets;
        }

        private Tuple<bool, string> VerifyArgumentsMatch(Parameters parameters, IInvocation invocation)
        {
            if (parameters.Method != invocation.MethodInvocationTarget.Name)
                return Tuple.Create(false, string.Format("Method name mismatch: {0} != {1}", parameters.Method, invocation.MethodInvocationTarget.Name));

            if (parameters.Types.Count != invocation.Arguments.Count())
            {
                if (parameters.Types.Count > invocation.Arguments.Count())
                    return Tuple.Create(false, string.Format("Parameter type/invocation count mismatch? [less] {0}::{1}(...)", invocation.MethodInvocationTarget.DeclaringType, invocation.Method.Name));

                // watch out for optional arguments...
                var invParamaters = invocation.Method.GetParameters().Select(x => new 
                { 
                    IsOptional = (x.Attributes & ParameterAttributes.Optional) == ParameterAttributes.Optional,
                }).ToList();

                var areWeMissingAnythingNotOptional = invParamaters.Skip(parameters.Types.Count).Any(x => !x.IsOptional);
                
                if (areWeMissingAnythingNotOptional)
                    return Tuple.Create(false, string.Format("Parameter type/invocation count mismatch? {0}::{1}(...)", invocation.MethodInvocationTarget.DeclaringType, invocation.Method.Name));
            }

            for (var i = 0; i < parameters.Types.Count; i++)
            {
                var type = parameters.Types[i];
             
                var invocationValue = invocation.Arguments[i];
                if (invocationValue != null)
                {
                    var invocationType = invocation.Arguments[i].GetType();

                    var invocationTypeName = invocation.Method.GetParameters()[i].ParameterType.Name;

                    // var invocationTypeName = invocationType.Name;

                    if (type.Contains("<") && type.Contains(">"))
                    {
                        if (invocationType.IsGenericType)
                        {
                            // unmangle the name
                            invocationTypeName = GetReadableTypeName(invocationType);
                        }
                        else
                        {
                            return Tuple.Create(false, string.Format("Expected a generic type: {0} but got: {1}", type, invocationType.Name));
                        }
                    }

                    if (!type.Equals(invocationTypeName))
                        return Tuple.Create(false, string.Format("Parameter / Invocation type mismatch: {0}/{1}", type, invocationTypeName));
                }
            }

            //_logger.Info("(Arguments validated)");
            return Tuple.Create(true, "");
        }

        private string GetReadableTypeName(Type t)
        {
            if (!t.IsGenericType) return t.Name;

            var genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));

            var genericArgs = string.Join(",", t.GetGenericArguments().Select(GetReadableTypeName).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        public string EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(path))) path = Path.GetDirectoryName(path);
            if (path != null && !Directory.Exists(path)) Directory.CreateDirectory(path);

            _logger.InfoFormat("Using storage directory @ {0}", path);
            return path;
        }

        public void RegisterDefaultImplementation<R>(string methodToOverride, R returnValue)
        {
            overrides.Add(methodToOverride, returnValue);
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose of managed resources.
                }

                // Clean up unmanaged resources here. 
                if (_stream != null) _stream.Close();

                // Mark disposing as done.
                disposed = true;
            }
        }

        ~MockDataReplayInterceptor()
        {
            Dispose(false);
        }
    }
}