using System;
using System.Collections.Generic;
using System.Reflection;

namespace QuesoStruct
{
    public static class Serializers
    {
        private static readonly Dictionary<Type, object> serializers;

        static Serializers()
        {
            serializers = new Dictionary<Type, object>();
        }

        public static void Register<TInst, TSerializer>() 
            where TSerializer : ISerializer<TInst>, new()
            where TInst : IStructInstance
        {
            serializers.Add(typeof(TInst), new TSerializer());
        }

        public static bool Has(Type instType)
        {
            return serializers.ContainsKey(instType);
        }

        public static bool Has<TInst>()
        {
            return serializers.ContainsKey(typeof(TInst));
        }

        public static ISerializer Get(Type instType) 
        {
            if (!Has(instType))
            {
                instType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, null);
            }

            var inst = serializers[instType];
            return inst as ISerializer;
        }

        public static ISerializer<TInst> Get<TInst>() 
            where TInst : IStructInstance
        {
            var type = typeof(TInst);

            if (!Has<TInst>())
            {
                type.GetMethod("Init", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, null);
            }

            var inst = serializers[type];
            return inst as ISerializer<TInst>;
        }
    }
}
