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
    /// MapItem.xaml 的交互逻辑
    /// </summary>
    public partial class MapItem : UserControl {
        public MapItem() {
            InitializeComponent();
        }

        public void ApplyMapName(string mapName) {
            this.uiMapName.Text = mapName;
        }

        public void ApplyMapImage(string mapName) {
            if (mapName == "") this.uiMapThumbs.ImageSource = new BitmapImage(new Uri("../Resources/DefaultMap.jpg", UriKind.Relative));
            else this.uiMapThumbs.ImageSource = new BitmapImage(new Uri(SMMLib.Utilities.Information.WorkPath.Enter("map").Enter(mapName + ".jpg").Path, UriKind.Absolute));
        }

        public void ApplyEnable(bool isEnable) {
            if (isEnable) {
                this.uiMapName.Opacity = 1;
                this.uiMapName.FontStyle = FontStyles.Normal;
                uiMapBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(176, 171, 168));
            } else {
                this.uiMapName.Opacity = 0.5;
                this.uiMapName.FontStyle = FontStyles.Italic;
                uiMapBorder.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
