using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class TestStruct1
    {
        [StructMember]
        public int num1 { get; set; }

        [StructMember]
        public long num2 { get; set; }

        [StructMember]
        public TestStruct2 test1 { get; set; }
    }
}
