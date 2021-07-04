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
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace QuesoStruct.Types.Collections
{
    public interface ICollectionOwner<TInst> : IStructInstance
        where TInst : IStructInstance
    {
        long? ItemCount { get; }
        bool TerminateOnStreamEnd { get; }

        bool IsTerminator(IStructInstance inst);
    }

    public class Collection<TInst> : IStructInstance, IList<TInst>
        where TInst : IStructInstance
    {
        public class Serializer : ISerializer<Collection<TInst>>, ISerializer
        {
            public Type InstanceType { get; } = typeof(Collection<TInst>);

            public Collection<TInst> Read(Context context)
            {
                var inst = new Collection<TInst>(context);
                context.TryAddInstance(inst);

                var owner = context.Current as ICollectionOwner<TInst>;
                if ((owner?.ItemCount).HasValue)
                {
                    for (var i = 0L; i < owner.ItemCount; i++)
                    {
                        context.Current = inst;
                        inst.Add(Serializers.Get<TInst>().Read(context));
                    }
                }
                else
                {
                    TInst item;
                    do
                    {
                        context.Current = inst;
                        inst.Add(item = Serializers.Get<TInst>().Read(context));
                    } while (!(owner?.IsTerminator(item) ?? false) && !(context.Stream.Position >= context.Stream.Length && (owner?.TerminateOnStreamEnd ?? true)));
                }

                return inst;
            }

            public void Write(Collection<TInst> inst, Context context)
            {
                var stream = context.Stream;

                if(inst.Offset.HasValue) stream.Seek(inst.Offset.Value, SeekOrigin.Begin);
                else inst.SetOffsetWithRefUpdate(stream.Position);

                context.TryAddInstance(inst);
                foreach (var item in inst)
                {
                    Serializers.Get<TInst>().Write(item, context);
                }
            }

            IStructInstance ISerializer.Read(Context context) => Read(context);
            void ISerializer.Write(IStructInstance inst, Context context) => Write(inst as Collection<TInst>, context);
        }

        public static void Init()
        {
            Serializers.Register<Collection<TInst>, Serializer>();
        }

        public Collection(Context context) : this(context.Current)
        {
            Offset = context.Stream.Position;
        }

        public Collection(IStructInstance parent = null) 
        {
            Parent = parent;
            References = new HashSet<IStructReference>(); 
            items = new List<TInst>(); 
        }

        public long? Offset { get; set; }
        public IStructInstance Parent { get; set; }

        public IStructInstance RelativeOffsetBase { get; }
        public HashSet<IStructReference> References { get; }

        private readonly List<TInst> items;

        public int Count => ((ICollection<TInst>)items).Count;
        public bool IsReadOnly => ((ICollection<TInst>)items).IsReadOnly;
        public TInst this[int index] { get => ((IList<TInst>)items)[index]; set => ((IList<TInst>)items)[index] = value; }

        public int IndexOf(TInst item)
        {
            return ((IList<TInst>)items).IndexOf(item);
        }

        public void Insert(int index, TInst item)
        {
            ((IList<TInst>)items).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<TInst>)items).RemoveAt(index);
        }

        public void Add(TInst item)
        {
            ((ICollection<TInst>)items).Add(item);
        }

        public void Clear()
        {
            ((ICollection<TInst>)items).Clear();
        }

        public bool Contains(TInst item)
        {
            return ((ICollection<TInst>)items).Contains(item);
        }

        public void CopyTo(TInst[] array, int arrayIndex)
        {
            ((ICollection<TInst>)items).CopyTo(array, arrayIndex);
        }

        public bool Remove(TInst item)
        {
            return ((ICollection<TInst>)items).Remove(item);
        }

        public IEnumerator<TInst> GetEnumerator()
        {
            return ((IEnumerable<TInst>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }
    }
}
