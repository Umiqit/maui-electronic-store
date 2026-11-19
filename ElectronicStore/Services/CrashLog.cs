namespace ElectronicStore.Services;

// Диагностический лог: пишет необработанные исключения и этапы запуска
// в %LOCALAPPDATA%\ElectronicStore\crash.log
public static class CrashLog
{
    private static readonly string Path =
        System.IO.Path.Combine(FileSystem.AppDataDirectory, "crash.log");

    private static readonly object Gate = new();

    public static void Write(string message)
    {
        try
        {
            lock (Gate)
            {
                File.AppendAllText(Path,
                    $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}");
            }
        }
        catch
        {
            // диагностика не должна ронять приложение
        }
    }

    public static void WriteException(string where, Exception ex)
    {
        Write($"{where}: {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
    }
}
