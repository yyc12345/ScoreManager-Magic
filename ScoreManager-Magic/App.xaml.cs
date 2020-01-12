using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Text;
using System.IO;

namespace ScoreManager_Magic {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {

            //single app mode
            Process[] pro = Process.GetProcesses();
            int n = pro.Where(p => p.ProcessName.Equals("ScoreManager-Magic")).Count();
            if (n > 1) {
                MessageBox.Show("程序已经运行！请不要多开程序！", "ScoreManager-Magic");
                Current.Shutdown();
            }

            base.OnStartup(e);

#if DEBUG
#else
            AppDomain.CurrentDomain.UnhandledException += (sender, ex) => {
                if (ex.ExceptionObject is System.Exception) {
                    var exx = (System.Exception)ex.ExceptionObject;
                    UncatchedErrorHandle(exx.Message, exx.StackTrace);
                }
            };
#endif

            LoadLanguage();
        }

        private void LoadLanguage() {
            var langInfo = CultureInfo.CurrentCulture.Name;
            ScoreManager_Magic.Core.I18NProcessor.ChangeLanguage(langInfo);
        }

        private void UncatchedErrorHandle(string message, string stackTrace) {
            try {
                var fs = new StreamWriter("scoremanager-error.log", false, Encoding.UTF8);
                fs.WriteLine("[SYS][ERROR] FATAL ERROR !");
                fs.WriteLine(message);
                fs.WriteLine(stackTrace);
                fs.Close();
                fs.Dispose();
            } catch {
                ;//skip
            }

            MessageBox.Show("一个意外错误发生，导致程序异常退出，请将您的操作过程以及本程序文件夹下的scoremanager-error.log文件发送给开发者，讲述您是如何引发这个错误的，从而帮助我们修正这个错误。", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
            SharedModule.logSystem.Close();
            App.Current.Shutdown();
        }

    }
}
