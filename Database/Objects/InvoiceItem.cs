using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;
public class InvoiceItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvoiceItemID { get; set; }
    public string? Name { get; set; } 
    public string? Description { get; set; } 
    public decimal? DefaultValue { get; set; } 
    public string? TransformFormula { get; set; } 
    public string? KeyWord { get; set; }

    List<CustomerInvoiceItem> CustomerInvoiceItems { get; set; } = new();
}