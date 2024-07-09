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
}