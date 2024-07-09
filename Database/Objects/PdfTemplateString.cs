using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplateString
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateStringID { get; set; }
    public string? FormatString { get; set; } 
    public double? VerticalStart { get; set; }
    public double? HorizontalStart { get; set; }
    public int? Brush { get; set; }
    public string? FontFamily { get; set; } 
    public double? FontSize { get; set; }

    [ForeignKey("PdfTemplateItem")]
    public int PdfTemplateItemID { get; set; }
    public PdfTemplateItem? PdfTemplateItem { get; set; }

    List<PdfTemplateKeyValue> PdfTemplateKeyValues { get; set; } = new();
}