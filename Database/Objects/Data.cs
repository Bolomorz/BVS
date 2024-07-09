using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class Data
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DataID { get; set; }
    public byte[]? Bytes { get; set; } 

    List<PdfTemplateImage> PdfTemplateImages { get; set; } = new();
}