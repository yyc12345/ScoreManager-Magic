using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace ScoreManager_Magic {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
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
                SharedModule.logSystem.WriteLog("[SYS][ERROR] FATAL ERROR !");
                SharedModule.logSystem.WriteLog(message);
                SharedModule.logSystem.WriteLog(stackTrace);

            } catch {
                ;//skip
            }

            MessageBox.Show("一个意外错误发生，导致程序异常退出，请将您的操作过程以及本程序文件夹下的log文件发送给开发者，帮助我们修正这个错误。", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
            SharedModule.logSystem.Close();
            App.Current.Shutdown();
        }

    }
}
