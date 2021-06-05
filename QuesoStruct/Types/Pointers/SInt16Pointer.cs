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
namespace QuesoStruct.Types.Pointers
{
    [StructType]
    public partial class SInt16Pointer<TInst> : IStructReference<TInst>
        where TInst : class, IStructInstance
    {
        public IPointerOwner Owner => Parent as IPointerOwner;

        public long OffsetValue => PointerValue + Owner.RelativeOffsetBase.Offset.Value + Owner.AddedOffsetFromBase;

        public bool IsResolved { get; private set; }

        public void Update()
        {
            var ptr = Instance?.Offset - Owner.RelativeOffsetBase?.Offset - Owner.AddedOffsetFromBase;
            if (ptr.HasValue)
            {
                PointerValue = (short)ptr;
                IsResolved = true;
            }
            else
            {
                Owner.SetNullPointer(this);
                IsResolved = Instance == null;
            }
        }

        [StructMember]
        public short PointerValue { get; set; }
    }
}
