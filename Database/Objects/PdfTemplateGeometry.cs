using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public enum PdfGeometryType {Arc, Bezier, ClosedCurve, Curve, Ellipse, Line, Pie, Polygon, Rectangle, RoundedRectangle}
public class PdfTemplateGeometry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateGeometryID { get; set; }
    public int? Pen { get; set; }
    public int? Type { get; set; }
    public int? FillMode { get; set; }
    public double? VerticalStart { get; set; }
    public double? HorizontalStart { get; set; }
    public double? VerticalEnd { get; set; }
    public double? HorizontalEnd { get; set; }
    public double? StartAngle { get; set; }
    public double? SweepAngle { get; set; }
    public double? Tension { get; set; }
    public double? EllipseWidth { get; set; }
    public double? EllipseHeight { get; set;}

    [ForeignKey("PdfTemplateDynamicItem")]
    public int PdfTemplateDynamicItemID { get; set; }
    public PdfTemplateDynamicItem? PdfTemplateDynamicItem { get; set; }

    [ForeignKey("PdfTemplateStaticItem")]
    public int PdfTemplateStaticItemID { get; set; }
    public PdfTemplateDynamicItem? PdfTemplateStaticItem { get; set; }

    List<PdfTemplatePoint> PdfTemplatePoints { get; set; } = new();
}