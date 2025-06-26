namespace Echo.Secs.Data
{
    /// <summary>
    /// SECS数据项结构
    /// </summary>
    public class SecsDataItem
    {
        public SecsItemType Type { get; set; }
        public object? Value { get; set; }

        public override string ToString()
        {
            return $"[{Type}] {Value}";
        }
    }
}
