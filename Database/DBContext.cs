using Microsoft.EntityFrameworkCore;

namespace BVS;

public class DBContext : DbContext
{
    #region DBSET
    public DbSet<Customer>? Customers { get; set; }
    public DbSet<CustomerInvoiceItem>? CustomerInvoiceItems { get; set; }
    public DbSet<CustomerPdfFile>? CustomerPdfFiles { get; set; }
    public DbSet<Data>? Datas { get; set; }
    public DbSet<InvoiceItem>? InvoiceItems { get; set; }
    public DbSet<Log>? Logs { get; set; }
    public DbSet<PdfKey>? PdfKeys { get; set; }
    public DbSet<PdfTemplate>? PdfTemplates { get; set; }
    public DbSet<PdfTemplateGeometry>? PdfTemplateGeometries { get; set; }
    public DbSet<PdfTemplateImage>? PdfTemplateImages { get; set; }
    public DbSet<PdfTemplateDynamicItem>? PdfTemplateDynamicItems{ get; set; }
    public DbSet<PdfTemplateStaticItem>? PdfTemplateStaticItems { get; set; }
    public DbSet<PdfTemplateKeyValue>? PdfTemplateKeyValues { get; set; }
    public DbSet<PdfTemplatePoint>? PdfTemplatePoints { get; set; }
    public DbSet<PdfTemplateString>? PdfTemplateStrings { get; set; }
    #endregion
    public string dbpath;
    public DBContext(int year)
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        dbpath = System.IO.Path.Join(path, $"db{year}.db");

        this.Database.Migrate();
        this.SaveChanges();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseSqlite($"Data Source={dbpath}");

    protected override void OnModelCreating(ModelBuilder model)
        => base.OnModelCreating(model);

    #region LOG
    public void WriteLog(Log log)
    {
        if(Logs is null)
        {
            return;
        }
        Logs.Add(log);
        this.SaveChanges();
    }

    public List<Log>? ReadLogs()
    {
        if(Logs is null)
        {
            return null;
        }
        return this.Logs.ToList();
    }
    #endregion

    #region CUSTOMER
    internal CustomerObject? GetCustomerObject(int CustomerID)
    {
        if(CustomerID < 1 || Customers is null || CustomerInvoiceItems is null || InvoiceItems is null) return null;

        var customers = Customers.Where(k => k.CustomerID == CustomerID).ToList();
        if(customers.Count == 0) return null;
        var customer = customers[0];

        List<CustomerInvoiceItemObject> ciio = new();
        var ciitems = CustomerInvoiceItems.Where(k => k.CustomerID == CustomerID).ToList();
        foreach(var ciitem in ciitems)
        {
            var iitems = InvoiceItems.Where(k => k.InvoiceItemID == ciitem.InvoiceItemID).ToList();
            if(iitems.Count == 0) continue;
            var iitem = iitems[0];
            ciio.Add(new(){CustomerInvoiceItem = ciitem, InvoiceItem = iitem});
        }
        return new(){Customer = customer, CustomerInvoiceItemObjects = ciio};
    }
    #endregion

    #region INVOICE
    #endregion

    #region PDF
    internal PdfTemplateObject? GetPdfTemplateObject(int PdfTemplateID)
    {
        if(PdfTemplateID < 1 || PdfTemplates is null || PdfTemplateDynamicItems is null) return null;

        var templates = PdfTemplates.Where(k => k.PdfTemplateID == PdfTemplateID).ToList();
        if (templates.Count == 0) return null;
        var template = templates[0];

        List<PdfTemplateDynamicItemCollection> dic = new();
        var ditems = PdfTemplateDynamicItems.Where(k => k.PdfTemplateID == PdfTemplateID).ToList();
        foreach(var ditem in ditems)
        {
            var ptdic = GetPdfTemplateDynamicItemCollection(ditem);
            if(ptdic is not null) dic.Add(ptdic);
        }

        return new(){Template = template, DynamicItemCollections = dic};
    }
    private PdfTemplateDynamicItemCollection? GetPdfTemplateDynamicItemCollection(PdfTemplateDynamicItem ditem)
    {
        if(PdfTemplateStrings is null || PdfTemplateGeometries is null || PdfTemplateImages is null) return null;
        List<ITemplateItem> items = new();

        var strings = PdfTemplateStrings.Where(k => k.PdfTemplateDynamicItemID == ditem.PdfTemplateDynamicItemID).ToList();
        foreach(var item in strings)
        {
            var psi = GetPdfStringItem(item);
            if(psi is not null) items.Add(psi);
        }

        var geometries = PdfTemplateGeometries.Where(k => k.PdfTemplateDynamicItemID == ditem.PdfTemplateDynamicItemID).ToList();
        foreach(var item in geometries)
        {
            var pgi = GetPdfGeometryItem(item);
            if (pgi is not null) items.Add(pgi);
        }

        var images = PdfTemplateImages.Where(k => k.PdfTemplateDynamicItemID == ditem.PdfTemplateDynamicItemID).ToList();
        foreach(var item in images)
        {
            var pii = GetPdfImageItem(item);
            if (pii is not null) items.Add(pii);
        }
        return new(){Item = ditem, Items = items};
    }
    private PdfStringItem? GetPdfStringItem(PdfTemplateString item)
    {
        if(PdfTemplateKeyValues is null || PdfKeys is null) return null;
        List<PdfStringItem.PdfKeyItem> keyItems = new();
        var keyValues = PdfTemplateKeyValues.Where(k => k.PdfTemplateStringID == item.PdfTemplateStringID).ToList();
        foreach(var keyValue in keyValues)
        {
            var keys = PdfKeys.Where(k => k.PdfKeyID == keyValue.PdfKeyID).ToList();
            keyItems.Add(new(){Key = keys[0], TemplateKeyValue = keyValue});
        }
        return new(){FormatString = item, Keys = keyItems};
    }
    private PdfGeometryItem? GetPdfGeometryItem(PdfTemplateGeometry item)
    {
        if(PdfTemplatePoints is null) return null;
        var points = PdfTemplatePoints.Where(k => k.PdfTemplateGeometryID == item.PdfTemplateGeometryID).ToList();
        return new(){Geometry = item, Points = points};
    }
    private PdfImageItem? GetPdfImageItem(PdfTemplateImage item)
    {
        if(Datas is null) return null;
        var datas = Datas.Where(k => k.DataID == item.DataID).ToList();
        return new(){Image = item, Data = datas[0]};
    }
    #endregion
}

