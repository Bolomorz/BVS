using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfKey
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfKeyID { get; set; }
    public string? Key { get; set; } 

    List<PdfTemplateKeyValue> PdfTemplateKeyValues { get; set; } = new();
}