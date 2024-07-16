using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateID { get; set; }
    public string? Name { get; set; }
    public double? VerticalStart { get; set; }
    public double? VerticalEnd { get; set; }
    public double? HorizontalStart { get; set; }
    public double? HorizontalEnd { get; set; }
    public string? Orientation { get; set; }

    List<PdfTemplateDynamicItem> PdfTemplateDynamicItems { get; set; } = new();
    List<PdfTemplateStaticItem> PdfTemplateStaticItems { get; set; } = new();
    List<CustomerPdfFile> CustomerPdfFiles { get; set; } = new();
}