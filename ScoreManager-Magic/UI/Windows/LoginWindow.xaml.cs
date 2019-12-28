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
using System.Windows.Shapes;
using SMMLib.Utilities;
using System.Threading.Tasks;
using SMMLib.Data;

namespace ScoreManager_Magic.UI.Windows {
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent();

            //apply saved config
            if (SharedModule.configManager.Configuration["RememberUser"].ConvertToInt().ConvertToBoolean())
                uiUserName.Text = SharedModule.configManager.Configuration["RememberedUser"];
            if (SharedModule.configManager.Configuration["RememberPassword"].ConvertToInt().ConvertToBoolean())
                uiPassword.Password = SharedModule.configManager.Configuration["RememberedPassword"];
            uiServer.Text = SharedModule.configManager.Configuration["Server"];
        }

        private void freezeUIControl(bool isFreeze) {
            if (isFreeze) {
                uiUserName.IsEnabled = false;
                uiPassword.IsEnabled = false;
                uiServer.IsEnabled = false;
                uiRemName.IsEnabled = false;
                uiRemPassword.IsEnabled = false;
                uiLoginBtn.IsEnabled = false;
            } else {
                uiUserName.IsEnabled = true;
                uiPassword.IsEnabled = true;
                uiServer.IsEnabled = true;
                uiRemName.IsEnabled = true;
                uiRemPassword.IsEnabled = true;
                uiLoginBtn.IsEnabled = true;
            }
        }

        private async void func_login(object sender, RoutedEventArgs e) {
            freezeUIControl(true);
            var user = uiUserName.Text;
            var password = uiPassword.Password;
            var server = uiServer.Text;

            //set server
            SharedModule.smm.ChangeDomain(server);

            //test version
            var (status, version) = await TaskEx.Run(() => { return SharedModule.smm.Version(); });
            if (!status.IsSuccess) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("登陆失败，远程服务器返回如下错误：" + status.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    freezeUIControl(false);
                }));
                return;
            }
            if (Information.SMMProtocolVersion != version) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("ScoreManager-Magic接口协议版本比配失败，请检查程序版本", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    freezeUIControl(false);
                }));
                return;
            }

            //login
            var cache = await TaskEx.Run(() => { return SharedModule.smm.Login(user, password); });
            if (!cache.IsSuccess) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("登陆失败，远程服务器返回如下错误：" + cache.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    freezeUIControl(false);
                }));
                return;
            }
            if (!SharedModule.smm.Priority.HasFlag(SM_Priority.User)) {
                //try logout
                await TaskEx.Run(() => { SharedModule.smm.Logout(); });
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("您的账号已被封禁，所以您无法登录，如果有任何异意，请联系管理员。", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    freezeUIControl(false);
                }));
                return;
            }

            //login ok, record config
            if (SharedModule.configManager.Configuration["RememberUser"].ConvertToInt().ConvertToBoolean())
                SharedModule.configManager.Configuration["RememberedUser"] = user;
            if (SharedModule.configManager.Configuration["RememberPassword"].ConvertToInt().ConvertToBoolean())
                SharedModule.configManager.Configuration["RememberedPassword"] = password;
            SharedModule.configManager.Configuration["Server"] = server;
            SharedModule.configManager.Save();

            //send message and open main window
            this.Dispatcher.Invoke(new Action(() => {
                MessageBox.Show("登陆成功，欢迎您：" + user, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                freezeUIControl(false);
                var realWindow = new MainWindow();
                realWindow.Show();
                this.Close();
            }));
            
        }

        private void func_remName(object sender, RoutedEventArgs e) {
            if (uiRemName.IsChecked.UniformBoolean()) SharedModule.configManager.Configuration["RememberUser"] = "1";
            else SharedModule.configManager.Configuration["RememberUser"] = "0";

            SharedModule.configManager.Save();
        }

        private void func_remPassword(object sender, RoutedEventArgs e) {
            if (uiRemPassword.IsChecked.UniformBoolean()) SharedModule.configManager.Configuration["RememberPassword"] = "1";
            else SharedModule.configManager.Configuration["RememberPassword"] = "0";

            SharedModule.configManager.Save();
        }
    }
}
