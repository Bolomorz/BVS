using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class PdfTemplateKeyValue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PdfTemplateKeyValueID { get; set; }
    public int? IndexInFormatString { get; set; }

    [ForeignKey("PdfTemplateString")]
    public int PdfTemplateStringID { get; set; }
    public PdfTemplateString? PdfTemplateString { get; set; }

    [ForeignKey("PdfKey")]
    public int PdfKeyID { get; set; }
    public PdfKey? PdfKey { get; set; }
}