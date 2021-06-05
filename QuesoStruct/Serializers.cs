/*
    QuesoStruct makes .NET based binary serialization code neat and easy
    Copyright (C) 2021 Chosen Few Software

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
