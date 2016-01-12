using System;
using System.IO;

namespace TW.Commons.Interfaces
{
    public interface ISerialiser
    {
        T DeSerialise<T>(Stream stream) where T : class;
        string SerialiseAs(Type type, object data);
        string Serialise<T>(T data, bool clean = false) where T : class;
        T DeSerialise<T>(string data) where T : class;
        void Serialise(Type type, Stream stream, object data);
        void Serialise<T>(Stream stream, T data) where T : class;
        T DeSerialiseFromPath<T>(string path) where T : class;
        void SerialiseToPath<T>(string path, T data) where T : class;
        object DeSerialiseAs(Type type, string data);
    }
}