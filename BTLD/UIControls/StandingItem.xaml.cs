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
    /// StandingItem.xaml 的交互逻辑
    /// </summary>
    public partial class StandingItem : UserControl {
        public StandingItem() {
            InitializeComponent();
        }

        public double CircleSize {
            get { return (double)GetValue(CircleSizeProperty); }
            set { SetValue(CircleSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircleSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircleSizeProperty =
            DependencyProperty.Register("CircleSize", typeof(double), typeof(StandingItem), new PropertyMetadata(100.0));

        public enum StandingPosition {
            Gold,
            Silver,
            Copper
        }

        public void Apply(string name, StandingPosition position) {
            switch (position) {
                case StandingPosition.Gold:
                    uiAvatar.Stroke = new SolidColorBrush(Color.FromRgb(218, 178, 115));
                    break;
                case StandingPosition.Silver:
                    uiAvatar.Stroke = new SolidColorBrush(Color.FromRgb(233, 233, 216));
                    break;
                case StandingPosition.Copper:
                    uiAvatar.Stroke = new SolidColorBrush(Color.FromRgb(181, 163, 101));
                    break;
                default:
                    uiAvatar.Stroke = new SolidColorBrush(Colors.White);
                    break;
            }

            uiUsername.Text = name;
            uiAvatarThumbs.ImageSource = SharedModule.userManager[name];
        }
    }
}
