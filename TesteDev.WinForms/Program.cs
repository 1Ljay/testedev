using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TesteDev.WinForms
{
    internal static class Program
    {
        public static string ConnectionString { get; private set; } = "";

        [STAThread]
        static void Main()
        {
            try
            {
                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(jsonPath))
                {
                    var json = File.ReadAllText(jsonPath);
                    using var doc = JsonDocument.Parse(json);
                    ConnectionString = doc.RootElement
                        .GetProperty("ConnectionStrings")
                        .GetProperty("Pg")
                        .GetString() ?? "";
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                ConnectionString = "Host=localhost;Port=5432;Database=TesteDev;Username=app_user;Password=SenhaForte!123;Pooling=true;";
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
