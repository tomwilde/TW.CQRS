using System;
using System.Linq;
using System.Reflection;

namespace UBS.Ipv.Evita.Common.Core.Tests.Helpers
{
    public class Invokers
    {
        public static S GetPrivateField<T, S>(T sut, string fieldName)
        {
            var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (S)fieldInfo.GetValue(sut);
        }

        public static void SetPrivateField<T>(T sut, string fieldName, object value)
        {
            var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo == null)
            {
                // fail over to static members
                fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            }

            fieldInfo.SetValue(sut, value);
        }

        public class Generic
        {
            public static void InvokeNonPublicVoidMemberOnSUT<S>(S sut, string memberName, Type[] genericTypes, params object[] parameters)
            {
                var method = typeof(S).GetMethod(memberName, BindingFlags.Instance | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(genericTypes);
                method.Invoke(sut, parameters);
            }

            public static T InvokeNonPublicMemberOnSUT<S, T>(S sut, string memberName, Type[] genericTypes, params object[] parameters)
            {
                var method = typeof(S).GetMethod(memberName, BindingFlags.Instance | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(genericTypes);
                return (T)method.Invoke(sut, parameters);
            }
        }

        public static void InvokeNonPublicVoidMemberOnSUT<S>(S sut, string memberName, params object[] parameters)
        {
            typeof(S).GetMethod(memberName, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(sut, parameters);
        }

        public static T InvokeNonPublicMemberOnSUT<S, T>(S sut, string memberName, params object[] parameters)
        {
            var methods = typeof(S).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                   .Where(x => x.Name == memberName)
                                   .ToList();

            if (methods.Count() == 1)
            {
                return (T)methods.First().Invoke(sut, parameters);
            }
            else
            {
                // try and match up arg / parameter types
                var matches = methods.Where(x =>
                {
                    var methodParameterTypes = x.GetParameters().Select(p => p.ParameterType).ToList();
                    var invocationTypes = parameters.Select(y => y.GetType()).ToList();
                    return methodParameterTypes.SequenceEqual(invocationTypes);
                }).ToList();

                if (matches.Any())
                {
                    if (matches.Count == 1)
                        return (T) matches.First().Invoke(sut, parameters);
                    else
                        throw new Exception("Couldnt match up the method to call with a single method. :/");
                }
                else
                {
                    throw new Exception("Couldnt match up the method to call with anything. :/");
                }
            }
        }

        public static void PrivateSetterOnPublicProperty<T>(T sut, string propertyName, params object[] data)
        {
            var prop = sut.GetType().GetMember(propertyName).First() as PropertyInfo;

            var setter = prop.GetSetMethod(true);

            setter.Invoke(sut, data);
        }
    }
}