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
            this.uiUserAvatar.ImageSource = SharedModule.userManager[username];
        }

        public void ApplyEnable(bool isEnable) {
            if (isEnable) {
                this.uiUserName.Opacity = 1;
                this.uiAvatarContainer.Opacity = 1;
                this.uiUserName.FontStyle = FontStyles.Normal;
            } else {
                this.uiUserName.Opacity = 0.5;
                this.uiAvatarContainer.Opacity = 0.5;
                this.uiUserName.FontStyle = FontStyles.Italic;
            }
        }
    }
}
