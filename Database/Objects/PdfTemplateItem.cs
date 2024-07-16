using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplateDynamicItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateDynamicItemID { get; set; }
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

public class PdfTemplateStaticItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateStaticItemID { get; set; }
    public double? VerticalStart { get; set; }
    public bool? DrawOnEachNewPage { get; set; }

    [ForeignKey("PdfTemplate")]
    public int PdfTemplateID { get; set; }
    public PdfTemplate? PdfTemplate { get; set; }

    List<PdfTemplateString> PdfTemplateStrings { get; set; } = new();
    List<PdfTemplateGeometry> PdfTemplateGeometries { get; set; } = new();
    List<PdfTemplateImage> PdfTemplateImages { get; set; } = new();
}