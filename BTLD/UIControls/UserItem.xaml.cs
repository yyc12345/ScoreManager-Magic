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

namespace BTLD.UIControls {
    /// <summary>
    /// UserItem.xaml 的交互逻辑
    /// </summary>
    public partial class UserItem : UserControl {
        public UserItem() {
            InitializeComponent();
        }

        public void ApplyUsername(string username) {
            this.uiUserName.Text = username;
        }

        public void ApplyUserAvatar(string username) {
            if (username == "") this.uiUserAvatar.ImageSource = new BitmapImage(new Uri("../Resources/DefaultUser.jpg", UriKind.Relative));
            else this.uiUserAvatar.ImageSource = new BitmapImage(new Uri(SMMLib.Utilities.Information.WorkPath.Enter("user").Enter(username + ".jpg").Path, UriKind.Absolute));
        }
    }
}
