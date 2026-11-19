using Plugin.Maui.Audio;

namespace ElectronicStore.Services;

// Воспроизведение звуковых сигналов приложения (через Plugin.Maui.Audio).
// Плеер создаётся лениво и переиспользуется.
public static class SoundService
{
    private static IAudioPlayer? successPlayer;

    public static void PlaySuccess()
    {
        try
        {
            successPlayer ??= CreatePlayer("success.wav");
            successPlayer.Seek(0);
            successPlayer.Play();
        }
        catch
        {
            // Звук — необязательное украшение, ошибки воспроизведения игнорируем
        }
    }

    private static IAudioPlayer CreatePlayer(string fileName)
    {
        var stream = FileSystem.OpenAppPackageFileAsync(fileName).Result;
        return AudioManager.Current.CreatePlayer(stream);
    }
}
