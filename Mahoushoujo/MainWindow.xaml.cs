using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SMMLib.Net;
using SMMLib.Utilities;
using SMMLib.Data;

namespace Mahoushoujo {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            logSystem = new LogManager(Information.WorkPath.Enter("mahoushoujo.log").Path);
            smmcore = new ScoreManager();
            configManager = new ConfigManager(Information.WorkPath.Enter("mahoushoujo.cfg").Path, new Dictionary<string, string>() {
#if DEBUG
            {"Server", "http://yyc.bkt.moe:6666/" },
#else
            {"Server", "http://yyc.bkt.moe:7777/" },
#endif
            });

            uiLoginDomain.Text = configManager.Configuration["Server"];
        }

        #region score manager related

        ScoreManager smmcore;
        LogManager logSystem;
        ConfigManager configManager;


        #endregion

        #region login

        private void funcLogin(object sender, RoutedEventArgs e) {
            smmcore.ChangeDomain(uiLoginDomain.Text);
            //test version
            var (status, version) = smmcore.Version();
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Information.GetSMMProtocolVersion() != version) {
                MessageBox.Show("Unmatched SMM protocol version", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //login
            var cache = smmcore.Login(uiLoginUserName.Text, uiLoginPassword.Password);
            if (!cache.IsSuccess) {
                MessageBox.Show("Fail to login: " + cache.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if ((smmcore.Priority & SM_Priority.Admin) != SM_Priority.Admin) {
                MessageBox.Show("Not enough permission", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Log in successfully!", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
            uiLoginLayer.Visibility = Visibility.Collapsed;
            uiOperLayer.Visibility = Visibility.Visible;
        }



        #endregion

        #region oper



        #endregion

    }
}
