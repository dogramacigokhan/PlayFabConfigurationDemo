public class ContentData
{
    public int IntValue { get; set; }
    public bool BoolValue { get; set; }
    public string StringValue { get; set; }
    public ComplexData ComplexValue { get; set; }

    public class ComplexData
    {
        public int InnerIntValue { get; set; }
        public bool InnerBoolValue { get; set; }
    }

    /// <summary>
    /// Default data to be used when the remote data cannot be downloaded for any reason.
    /// </summary>
    public static readonly ContentData Default = new ContentData
    {
        IntValue = 42,
        BoolValue = true,
        StringValue = "Local-data",
        ComplexValue = new ComplexData
        {
            InnerIntValue = 4242,
            InnerBoolValue = false,
        }
    };
}