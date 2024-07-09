namespace BVS;

internal static class Logging
{
    internal static void WriteLog(string code, string description)
    {
        DBFunctions.WriteLog(new Log(){Time = DateTime.Now.ToString(), Code = code, Description = description});
    }

    internal static void WriteLogDirectlyToFile(string code, string description)
    {
        using StreamWriter sw = new("Controller/Logging/Log.txt", append: true);
        sw.WriteLine("Log:");
        sw.WriteLine($"{DateTime.Now}\n{code}\n{description}");
        sw.WriteLine("----------");
    }

    internal static void ProduceLogFile()
    {
        using StreamWriter sw = new("LogFile.txt");
        var logs = DBFunctions.ReadLogs();
        if(logs is not null)
        {
            foreach (var log in logs)
            {
                sw.WriteLine($"{log.Time}\n{log.Code}\n{log.Description}");
                sw.WriteLine("----------");
            }
        }
    }
}