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
            LoadLanguage();
        }

        private void LoadLanguage() {
            var langInfo = CultureInfo.CurrentCulture.Name;
            ScoreManager_Magic.Core.I18NProcessor.ChangeLanguage(langInfo);
        }

    }
}
