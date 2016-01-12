using System;

namespace TW.Commons.IoC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class FactoryType : Attribute
    {
        private readonly Type type;

        public FactoryType(Type type)
        {
            this.type = type;
        }

        public Type TypeToRegisterWithContainer
        {
            get { return type; }
        }
    }
}