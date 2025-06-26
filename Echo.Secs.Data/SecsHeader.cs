namespace Echo.Secs.Data
{
    public struct SecsHeader
    {
        public uint Length;
        public ushort DeviceId;
        public SecsCommand Cmd;
        public SecsCommandType CmdType;
        public bool Wait;
        public uint RandomMask;
    }
}
