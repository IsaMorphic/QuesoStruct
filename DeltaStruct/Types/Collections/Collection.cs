﻿using System.Collections;
using System.Collections.Generic;

namespace DeltaStruct.Types.Collections
{
    public interface ICollectionOwner<TInst> : IStructInstance
        where TInst : IStructInstance
    {
        long? ItemCount { get; }
        bool IsTerminator(IStructInstance inst);
    }

    public class Collection<TInst> : IStructInstance, IList<TInst>
        where TInst : IStructInstance
    {
        public class Serializer : ISerializer<Collection<TInst>>
        {
            public Collection<TInst> Read(Context context)
            {
                var inst = new Collection<TInst>(context);
                var owner = context.Current as ICollectionOwner<TInst>;

                context.Current = inst.Parent;

                if (owner.ItemCount.HasValue)
                {
                    for (var i = 0L; i < owner.ItemCount; i++)
                    {
                        inst.Add(Serializers.Get<TInst>().Read(context));
                    }
                }
                else
                {
                    TInst item;
                    do
                    {
                        inst.Add(item = Serializers.Get<TInst>().Read(context));
                    } while (!owner.IsTerminator(item));
                }

                context.TryAddInstance(inst);
                return inst;
            }

            public void Write(Collection<TInst> inst, Context context)
            {
                foreach (var item in inst)
                {
                    Serializers.Get<TInst>().Write(item, context);
                }
            }
        }

        public static void Init()
        {
            Serializers.Register<Collection<TInst>, Serializer>();
        }

        public Collection(Context context) : this()
        {
            Offset = context.Stream.Position;
            Parent = context.Current;
        }

        public Collection() 
        { 
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