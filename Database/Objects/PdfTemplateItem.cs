using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplateItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateItemID { get; set; }
    public double? VerticalOffsetToPreviousItem { get; set; }
    public bool? AlwaysStartNewPage { get; set; }
    public bool? RepeatForEachDataObject { get; set; }

    [ForeignKey("PdfTemplate")]
    public int PdfTemplateID { get; set; }
    public PdfTemplate? PdfTemplate { get; set; }

    List<PdfTemplateString> PdfTemplateStrings { get; set; } = new();
    List<PdfTemplateGeometry> PdfTemplateGeometries { get; set; } = new();
    List<PdfTemplateImage> PdfTemplateImages { get; set; } = new();
}