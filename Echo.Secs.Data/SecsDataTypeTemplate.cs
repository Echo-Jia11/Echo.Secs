namespace Echo.Secs.Data
{
    public enum SecsDataTypeTemplate : byte
    {
        List1 = 0x01, // 00000001
        List2 = 0x02, // 00000010
        List3 = 0x03, // 00000011
        Binary = 0x21, // 00100001
        boolean = 0x25, // 00100101
        Ascii1 = 0x41, // 01000001
        Ascii2 = 0x42, // 01000010
        Ascii3 = 0x43, // 01000011
        Long = 0x61, // 01100001
        SByte = 0x65, // 01100101
        Short = 0x69, // 01101001
        Int = 0x71, // 01110001
        Double = 0x81, // 10000001
        Float = 0x91, // 10010001
        ULong = 0xA1, // 10100001
        Byte = 0xA5, // 10100101
        UShort = 0xA9, // 10101001
        UInt = 0xB1, // 10110001
    }
}
