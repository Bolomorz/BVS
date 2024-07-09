using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;

public class CustomerPdfFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? CustomerPdfFileID { get; set; }

    public byte[]? PdfFile { get; set; }
    public DateTime? Time { get; set; }

    [ForeignKey("PdfTemplate")]
    public int PdfTemplateID { get; set; }
    public PdfTemplate? PdfTemplate { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public Customer? Customer { get; set; }
}