namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class TestStruct2
    {
        [StructMember]
        public sbyte num1 { get; set; }

        [StructMember]
        public double num2 { get; set; }

        [StructMember]
        public long num3 { get; set; }

        public void OnAfterRead() { }
        public void OnBeforeWrite(Context context) { }
    }
}
