namespace BVS;

internal class KeyValue
{
    internal required string Key { get; set; }
    internal required string Value { get; set; }
}

internal class PdfKeyValuePair
{
    public required string Value { get; set; }
    public required int PdfTemplateKeyValueID { get; set; }
}

internal class PdfFile
{
    public required bool Success { get; set; }
    public byte[]? PdfBytes { get; set; }
    public string? FileName { get; set; }
}

internal class PdfFileQueryItem
{
    public required int CustomerPdfFileID { get; set; }
    public required Customer Customer { get; set; }
    public required string TemplateName { get; set; }
    public required DateTime TimeOfSaving { get; set; }
}