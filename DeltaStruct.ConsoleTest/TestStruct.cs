using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public class TestStruct
    {
        [StructMember]
        public int num1 { get; set; }

        [StructMember]
        public long num2 { get; set; }
    }
}
