using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;
public class CustomerInvoiceItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerInvoiceItemID { get; set; }
    public decimal? Value { get; set; }
    public bool? Active { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public Customer? Customer { get; set; }

    [ForeignKey("InvoiceItem")]
    public int InvoiceItemID { get; set; }
    public InvoiceItem? InvoiceItem { get; set; }
}