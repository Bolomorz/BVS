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
    public DbSet<PdfTemplateItem>? PdfTemplateItems{ get; set; }
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
    #endregion

    #region INVOICE
    #endregion

    #region PDF
    #endregion
}

