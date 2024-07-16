namespace BVS;

internal static class DBFunctions
{
    private static readonly object lo = new();

    internal static void WriteLog(Log log)
    {
        lock(lo)
        {
            try
            {
                using DBContext db = new(Settings.year);
                db.WriteLog(log);
            }
            catch (Exception ex)
            {
                Logging.WriteLogDirectlyToFile("WriteLog", ex.ToString());
            }
        }
    }

    internal static List<Log>? ReadLogs()
    {
        lock(lo)
        {
            try
            {
                using DBContext db = new(Settings.year);
                return db.ReadLogs();
            }
            catch (Exception ex)
            {
                Logging.WriteLogDirectlyToFile("ReadLog", ex.ToString());
                return null;
            }
        }
    }

    internal static PdfTemplateObject? GetPdfTemplateObject(int ID)
    {
        lock(lo)
        {
            try
            {
                using DBContext db = new(Settings.year);
                return db.GetPdfTemplateObject(ID);
            }
            catch (Exception ex)
            {
                Logging.WriteLogDirectlyToFile("GetPdfTemplateObject", ex.ToString());
                return null;
            }
        }
    }

    internal static CustomerObject? GetCustomerObject(int ID)
    {
        lock(lo)
        {
            try
            {
                using DBContext db = new(Settings.year);
                return db.GetCustomerObject(ID);
            }
            catch (Exception ex)
            {
                Logging.WriteLogDirectlyToFile("GetCustomerObject", ex.ToString());
                return null;
            }
        }
    }
}