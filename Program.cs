using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsPegadoEspecial {
    class Program {
        const string APP_NAME = "Pegado especial";
        [STAThread]
        static void Main(string[] args) {
            TryRegisterApp();
            string currentLocation = args.Length == 0 ? Application.StartupPath : args[0].Replace("\"", "");
            string trailingTemplate = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            if (Clipboard.ContainsText()) {
                File.WriteAllText(string.Format(@"{0}\recorte {1}.txt", currentLocation, trailingTemplate), Clipboard.GetText(), Encoding.UTF8);
            }

            if (Clipboard.ContainsImage()) {
                Clipboard.GetImage().Save(string.Format(@"{0}\imagen {1}.jpg", currentLocation, trailingTemplate));
            }
        }
        static void TryRegisterApp() {
            const string rootKey = @"HKEY_CURRENT_USER";
            const string directoryKey = @"SOFTWARE\Classes\Directory";
            const string backgroundShellKey = @"Background\shell";
            const string shellKey = "shell";
            const string commandDataTemplate = "\"{0}\" \"{1}\"";
            

            string backgroundCommandData = string.Format(commandDataTemplate, Application.ExecutablePath, "%V");
            object currentBackgroundCommandData = Registry.GetValue(string.Format(@"{0}\{1}\{2}\{3}\command", rootKey, directoryKey, backgroundShellKey, APP_NAME), "", null);
            if (currentBackgroundCommandData == null || backgroundCommandData.CompareTo(currentBackgroundCommandData) != 0) {
                string iconData = string.Format("\"{0}\",0", Application.ExecutablePath);
                RegistryKey root_key = Registry.CurrentUser.CreateSubKey(directoryKey);

                // Menú contextual al hacer click derecho en la parte vacía de un directorio
                RegistryKey key = root_key.CreateSubKey(backgroundShellKey);
                key = key.CreateSubKey(APP_NAME);
                key.SetValue("", "Crear fichero con contenidos del portapapeles");
                key.SetValue("Icon", iconData);
                key = key.CreateSubKey("command");
                key.SetValue("", backgroundCommandData);

                // Menú contextual al hacer click derecho en un fichero
                key = root_key.CreateSubKey(shellKey);
                key = key.CreateSubKey(APP_NAME);
                key.SetValue("", "Pegar contenidos del portapapeles");
                key.SetValue("Icon", iconData);
                key = key.CreateSubKey("command");
                key.SetValue("", string.Format(commandDataTemplate, Application.ExecutablePath, "%1"));
            }
        }
    }
}
