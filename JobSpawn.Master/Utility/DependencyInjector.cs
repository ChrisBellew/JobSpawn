using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JobSpawn.Utility
{
    public static class DependencyInjector
    {
        private static readonly ConcurrentDictionary<Type, Type> types = new ConcurrentDictionary<Type, Type>();

        public static void AddType<TContract, TConcrete>() where TConcrete : TContract
        {
            types.GetOrAdd(typeof(TContract), typeof(TConcrete));
        }

        public static InstanceContext CreateInstanceContext()
        {
            return new InstanceContext();
        }
        
        public class InstanceContext
        {
            private static readonly ConcurrentDictionary<Type, object> instances = new ConcurrentDictionary<Type, object>();

            public TContract GetInstance<TContract>()
            {
                return (TContract) GetInstance(typeof(TContract), new Dictionary<string, object>());
            }

            public TContract GetInstance<TContract>(Dictionary<string, object> partialArguments)
            {
                return (TContract) GetInstance(typeof(TContract), partialArguments);
            }

            public object GetInstance(Type contractType, Dictionary<string, object> partialArguments)
            {
                return instances.GetOrAdd(contractType, x => CreateInstance(contractType, partialArguments));
            }

            private object CreateInstance(Type contractType, Dictionary<string, object> partialArguments)
            {
                var concreteType = types[contractType];
                var constructor = concreteType.GetConstructors().Single();
                var arguments = constructor.GetParameters().Select(parameter => partialArguments.ContainsKey(parameter.Name) ? partialArguments[parameter.Name] : GetInstance(parameter.ParameterType, new Dictionary<string, object>())).ToArray();
                
                return constructor.Invoke(arguments);
            }
        }
    }
}
