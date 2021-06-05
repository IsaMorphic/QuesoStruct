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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuesoStruct.Types.Collections
{
    public interface IDoublyLinkedItem<TInst> : IStructInstance
        where TInst : class, IDoublyLinkedItem<TInst>
    {
        TInst Prev { get; set; }
        TInst Next { get; set; }
    }

    public partial class LinkedList
    {
        [StructType]
        public partial class Double<TInst> : IList<TInst>
            where TInst : class, IDoublyLinkedItem<TInst>
        {
            [StructMember]
            public TInst Start { get; set; }

            public TInst this[int index]
            {
                get
                {
                    TInst curr = Start;
                    for (int i = 0; i < index; i++)
                    {
                        curr = curr.Next;
                    }
                    return curr;
                }
                set
                {
                    TInst before = Start;
                    for (int i = 0; i < index - 1; i++)
                    {
                        before = before.Next;
                    }
                    TInst after = before.Next.Next;

                    before.Next = value;
                    value.Prev = before;

                    value.Next = after;
                    after.Prev = value;
                }
            }

            public int Count
            {
                get
                {
                    int count = 0;
                    TInst curr = Start;

                    while (curr != null) { curr = curr?.Next; count++; }

                    return count;
                }
            }

            public bool IsReadOnly => false;

            public void Add(TInst item)
            {
                if (this.Any())
                {
                    var last = this.Last();
                    last.Next = item;
                    item.Prev = last;
                }
                else Start = item;
            }

            public void Insert(int index, TInst item)
            {
                TInst before = Start;
                for (int i = 0; i < index - 1; i++)
                {
                    before = before.Next;
                }
                TInst after = before.Next;

                before.Next = item;
                item.Next = after;
            }

            public bool Remove(TInst item)
            {
                TInst before = Start;
                while (before != item) { before = before?.Next; };

                if (before == null)
                {
                    return false;
                }
                else
                {
                    before.Next = before.Next.Next;
                    before.Next.Prev = before;
                    return true;
                }
            }

            public void RemoveAt(int index)
            {
                TInst before = Start;
                for (int i = 0; i < index - 1; i++)
                {
                    before = before.Next;
                }

                before.Next = before.Next.Next;
                before.Next.Prev = before;
            }

            public void Clear()
            {
                Start = null;
            }

            public int IndexOf(TInst item)
            {
                int index = 0;
                TInst curr = Start;

                while (curr != item) { curr = curr?.Next; index++; }

                if (curr == null)
                    return -1;
                else
                    return index;
            }

            public bool Contains(TInst item)
            {
                return IndexOf(item) < 0;
            }

            public void CopyTo(TInst[] array, int arrayIndex)
            {
                TInst curr = Start;
                for (int i = 0; i < Count; i++)
                {
                    array[arrayIndex + i] = curr;
                    curr = curr.Next;
                }
            }

            class DoublyLinkedListEnumerator : IEnumerator, IEnumerator<TInst>
            {
                private readonly TInst start;

                public TInst Current { get; private set; }
                object IEnumerator.Current => Current;

                public DoublyLinkedListEnumerator(TInst start)
                {
                    Current = this.start = start;
                }

                public bool MoveNext() => (Current = Current.Next) != null;
                public void Reset() => Current = start;

                public void Dispose() { }
            }

            public IEnumerator<TInst> GetEnumerator()
            {
                return new DoublyLinkedListEnumerator(Start);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
