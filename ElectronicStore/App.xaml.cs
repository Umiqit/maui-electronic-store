using ElectronicStore.Services;

namespace ElectronicStore
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Глобальный перехват необработанных исключений (диагностика)
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                CrashLog.Write("AppDomain.UnhandledException: " + e.ExceptionObject);

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                CrashLog.WriteException("TaskScheduler.UnobservedTaskException", e.Exception);
                e.SetObserved();
            };

#if WINDOWS
            Microsoft.UI.Xaml.Application.Current.UnhandledException += (s, e) =>
            {
                CrashLog.Write("WinUI.UnhandledException: " + e.Message + (e.Exception != null ? Environment.NewLine + e.Exception : ""));
                e.Handled = true; // не даём приложению закрыться до прочтения лога
            };
#endif

            CrashLog.Write("=== App создан ===");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            CrashLog.Write("CreateWindow");
            return new Window(new AppShell());
        }
    }
}
