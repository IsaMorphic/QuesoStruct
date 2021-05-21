﻿using System;
using System.Collections.Generic;

namespace DeltaStruct
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

        public static bool Has<TInst>()
        {
            return serializers.ContainsKey(typeof(TInst));
        }

        public static ISerializer<TInst> Get<TInst>() 
            where TInst : IStructInstance
        {
            var inst = serializers[typeof(TInst)];
            return inst as ISerializer<TInst>;
        }
    }
}
