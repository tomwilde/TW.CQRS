using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TW.Commons.Interfaces;

namespace TW.Commons.Serialisation
{
    public class Serialiser : ISerialiser
    {
        public Serialiser()
        {
        }

        public virtual T DeSerialise<T>(Stream stream) where T : class
        {
            T result;
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (stream)
            {
                result = xmlSerializer.Deserialize(stream) as T;
            }

            return result;
        }

        public virtual string SerialiseAs(Type type, object data)
        {
            var xs = new XmlSerializer(type);
            return SerialiseUsingMemoryStream(xs, data);
        }

        public string Serialise<T>(T data, bool clean = false) where T : class
        {
            var xs = new XmlSerializer(typeof (T));
            return SerialiseUsingMemoryStream(xs, data, clean);
        }

        private string SerialiseUsingMemoryStream(XmlSerializer xs, object data, bool clean = false)
        {
            var buffer = string.Empty;
            using (var stream = new MemoryStream())
            {

                var writerSettings = new XmlWriterSettings();

                if (clean)
                {
                    writerSettings.OmitXmlDeclaration = true;
                    writerSettings.Encoding = Encoding.ASCII;
                }
                else
                {
                    writerSettings.Encoding = Encoding.UTF8;
                }

                using (var writer = XmlWriter.Create(stream, writerSettings))
                {
                    xs.Serialize(writer, data);
                    buffer = Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            return buffer;
        }

        public T DeSerialise<T>(string data) where T : class
        {
            var xs = new XmlSerializer(typeof(T));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return xs.Deserialize(ms) as T;
            }
        }

        public virtual void Serialise(Type type, Stream stream, object data)
        {
            InnerSerialise(stream, new XmlSerializer(type), data);
        }

        public virtual void Serialise<T>(Stream stream, T data) where T : class
        {
            InnerSerialise(stream, new XmlSerializer(typeof(T)), data);
        }

        private void InnerSerialise(Stream stream, XmlSerializer xmlSerializer, object data)
        {
            using (stream)
            {
                xmlSerializer.Serialize(stream, data);
            }
        }

        public T DeSerialiseFromPath<T>(string path) where T : class
        {
            var data = System.IO.File.ReadAllText(path);

            return DeSerialise<T>(data);
        }

        public virtual void SerialiseToPath<T>(string path, T data) where T : class
        {
            throw new NotImplementedException();

           // make sure the target directory exists...
           // new FileUtilsNS().EnsureDirectoryExists(path);

           var strdata = Serialise(data);
           System.IO.File.WriteAllText(path, strdata);
        }

        public object DeSerialiseAs(Type type, string data)
        {
            var xs = new XmlSerializer(type);

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return xs.Deserialize(ms);
            }
        }
    }
}