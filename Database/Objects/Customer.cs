using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerID { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; } 
    public string? Street { get; set; }
    public string? StreetNumber { get; set; } 
    public string? PostalCode { get; set; } 
    public string? City { get; set; }
    public string? Birthday { get; set; }
    public string? JoinDate { get; set; } 
    public decimal? PaidAmount { get; set; } 

    List<CustomerInvoiceItem> CustomerInvoiceItems { get; set; } = new();
    List<CustomerPdfFile> CustomerPdfFiles { get; set; } = new();
}