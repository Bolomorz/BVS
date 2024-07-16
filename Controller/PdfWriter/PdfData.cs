namespace BVS;

internal class PdfData
{
    public int PdfTemplateID { get; set; } = 0;
    public List<int> CustomerIDs { get; set; } = new();
    public List<PdfCustomerData> PdfCustomerDatas { get; set; } = new();
}

internal class PdfCustomerData
{
    internal List<PdfKeyValuePair> Data { get; set; } = new();
}