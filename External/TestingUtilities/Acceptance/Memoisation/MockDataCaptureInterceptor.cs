using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Castle.Core.Interceptor;
using TW.Commons.Interfaces;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    public class MockDataCaptureInterceptor : IInterceptor, IDisposable
    {
        public const string DelimiterText = "<|MockDataCaptureInterceptorDelimiter|>";
        
        private readonly ILogger _logger;
        private readonly BinaryFormatter serialiser = new BinaryFormatter();

        private readonly string _name;
        private readonly string _outputFilePath;
        private readonly byte[] _delimiter;

        private FileStream _stream;

        private int callCount;

        public MockDataCaptureInterceptor(ILoggerFactory loggerFactory, Type type, string storagePath, int? id = null)
        {
            _name = type.Name;
            
            _outputFilePath = Path.Combine(storagePath, id == null
                ? string.Format("{0}.capture", _name)
                : string.Format("{0}_{1}.capture", _name, id));

            _logger = loggerFactory.Create();

            _delimiter = Encoding.ASCII.GetBytes(DelimiterText);
        }

        public void Intercept(IInvocation invocation)
        {
            EnsureOpenStream();

            callCount++;

            SerialiseParametersToDisk(invocation);
            WriteDelimiter();

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                _logger.ErrorFormat("Target threw an exception!");
                throw;
            }

            SerialiseOutputToDisk(invocation);
            WriteDelimiter();
        }

        private void WriteDelimiter()
        {
            _stream.Write(_delimiter, 0, _delimiter.Length);
        }

        private void EnsureOpenStream()
        {
            if (!File.Exists(_outputFilePath) && _stream != null)
            {
                // capture the fact someone else deleted the capture file... 
                _stream.Close();
                _stream = null;
            }

            if (_stream == null) _stream = new FileStream(_outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Write | FileShare.Delete);
        }

        private void SerialiseParametersToDisk(IInvocation invocation)
        {
            try
            {
                serialiser.Serialize(_stream,
                    new Parameters()
                    {
                        CallNumber = callCount,
                        Method = invocation.MethodInvocationTarget.Name,
                        Types = new List<string>(invocation.MethodInvocationTarget.GetParameters().Select(p => GetReadableTypeName(p.ParameterType))),
                        Data = invocation.Arguments
                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                _stream.Close();
                throw;
            }
        }

        private string GetReadableTypeName(Type t)
        {
            if (!t.IsGenericType) return t.Name;

            var genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));

            var genericArgs = string.Join(",", t.GetGenericArguments().Select(GetReadableTypeName).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        private void SerialiseOutputToDisk(IInvocation invocation)
        {
            var data = new ReturnValue()
            {
                CallNumber = callCount,
                Method = invocation.MethodInvocationTarget.Name,
                Type = GetReadableTypeName(invocation.Method.ReturnType),
                Data = invocation.ReturnValue
            };

            try
            {
                serialiser.Serialize(_stream, data);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                _stream.Close();
                throw;
            }
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

        ~MockDataCaptureInterceptor()
        {
            Dispose(false);
        }

    }
}