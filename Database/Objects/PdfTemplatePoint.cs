using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplatePoint
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplatePointID { get; set; }
    public double? X { get; set; }
    public double? Y { get; set; }
    
    [ForeignKey("PdfTemplateGeometry")]
    public int PdfTemplateGeometryID { get; set; }
    public PdfTemplateGeometry? PdfTemplateGeometry { get; set; }
}