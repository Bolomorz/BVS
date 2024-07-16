namespace BVS;

internal class CustomerObject
{
    internal required Customer Customer { get; set; }
    internal required List<CustomerInvoiceItemObject> CustomerInvoiceItemObjects { get; set; } 

    internal string? GetKeyValue(string key)
    {
        switch(key)
        {
            case "CustomerGivenName": return Customer.GivenName;
            case "CustomerSurname": return Customer.Surname;
            case "CustomerStreet": return Customer.Street;
            case "CustomerStreetNumber": return Customer.StreetNumber;
            case "CustomerPostalCode": return Customer.PostalCode;
            case "CustomerCity": return Customer.City;
            case "CustomerBirthday": return Customer.Birthday;
            case "CustomerJoinDate": return Customer.JoinDate;
            case "CustomerPaidAmount": return Customer.PaidAmount.ToString();
            default: return null;
        }
    }
}

internal class CustomerInvoiceItemObject
{
    internal required CustomerInvoiceItem CustomerInvoiceItem { get; set; }
    internal required InvoiceItem InvoiceItem { get; set; }
    internal string? GetKeyValue(string key)
    {
        switch(key)
        {
            case "InvoiceItemValue": return CustomerInvoiceItem.Value.ToString();
            case "InvoiceItemActive": return CustomerInvoiceItem.Active.ToString();
            case "InvoiceItemName": return InvoiceItem.Name;
            case "InvoiceItemDescription": return InvoiceItem.Description;
            default: return null;
        }
    }
}