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
        if(key == InvoiceItem.KeyWord + "Value") return CustomerInvoiceItem.Value.ToString();
        if(key == InvoiceItem.KeyWord + "Active") return CustomerInvoiceItem.Active.ToString();
        if(key == InvoiceItem.KeyWord + "Name") return InvoiceItem.Name;
        if(key == InvoiceItem.KeyWord + "Description") return InvoiceItem.Description;
        return null;
    }
}