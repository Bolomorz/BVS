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
    public double? HorizontalEnd { get; set; }
    public int? Brush { get; set; }
    public string? FontFamily { get; set; } 
    public double? FontSize { get; set; }

    [ForeignKey("PdfTemplateDynamicItem")]
    public int PdfTemplateDynamicItemID { get; set; }
    public PdfTemplateDynamicItem? PdfTemplateDynamicItem { get; set; }

    [ForeignKey("PdfTemplateStaticItem")]
    public int PdfTemplateStaticItemID { get; set; }
    public PdfTemplateDynamicItem? PdfTemplateStaticItem { get; set; }

    List<PdfTemplateKeyValue> PdfTemplateKeyValues { get; set; } = new();
}