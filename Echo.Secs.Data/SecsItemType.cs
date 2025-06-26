namespace Echo.Secs.Data
{
    /// <summary>
    /// SECS-II数据项类型（基于SEMI标准）
    /// </summary>
    public enum SecsItemType : byte
    {
        List = 0x00,
        Binary = 0x20,   // 00100000
        Boolean = 0x24,  // 00100100
        ASCII = 0x40,    // 01000000
        I8 = 0x60,       // 01100000
        I1 = 0x64,       // 01100100
        I2 = 0x68,       // 01101000
        I4 = 0x70,       // 01110000
        F8 = 0x80,       // 10000000
        F4 = 0x90,       // 10010000
        U8 = 0xA0,       // 10100000
        U1 = 0xA4,       // 10100100
        U2 = 0xA8,       // 10101000
        U4 = 0xB0,       // 10110000
        Empty = 0xFF
    }
}
