using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplateImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateImageID { get; set; }
    public double? VerticalStart { get; set; }
    public double? VerticalEnd { get; set; }
    public double? HorizontalStart { get; set; }
    public double? HorizontalEnd { get; set; }

    [ForeignKey("Data")]
    public int DataID { get; set; }
    public Data? Data { get; set; }

    [ForeignKey("PdfTemplateItem")]
    public int PdfTemplateItemID { get; set; }
    public PdfTemplateItem? PdfTemplateItem { get; set; }
}