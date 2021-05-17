using System;
using System.Collections.Generic;

namespace DeltaStruct
{
    public static class Serializers
    {
        private static Dictionary<Type, object> serializers;

        static Serializers()
        {
            serializers = new Dictionary<Type, object>();
        }

        public static void Register<TInst, TSerializer>() where TSerializer : ISerializer<TInst>, new()
        {
            serializers.Add(typeof(TInst), new TSerializer());
        }

        public static ISerializer<TInst> Get<TInst>()
        {
            return serializers[typeof(TInst)] as ISerializer<TInst>;
        }
    }
}
