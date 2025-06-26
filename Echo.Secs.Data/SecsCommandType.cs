namespace Echo.Secs.Data
{
    public enum SecsCommandType : byte
    {
        Data,
        SelectReq,
        SelectRsp,
        DeSelectReq,
        DeSelectRsp,
        LinkTestReq,
        LinkTestRsp
    }
}
