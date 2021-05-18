using System;
using System.Collections.Generic;

namespace DeltaStruct
{
    public static class Serializers
    {
        private static Dictionary<Type, Type> types;

        static Serializers()
        {
            types = new Dictionary<Type, Type>();
        }

        public static void Register<TInst, TSerializer>() where TSerializer : ISerializer<TInst>
        {
            types.Add(typeof(TInst), typeof(TSerializer));
        }

        public static bool Has<TInst>()
        {
            return types.ContainsKey(typeof(TInst));
        }

        public static ISerializer<TInst> Get<TInst>(Context context)
        {
            var type = types[typeof(TInst)];
            var inst = Activator.CreateInstance(type, context);
            return inst as ISerializer<TInst>;
        }
    }
}
