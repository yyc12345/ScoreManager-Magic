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
using System.IO;

namespace BTLD {
    /// <summary>
    /// Monitor.xaml 的交互逻辑
    /// </summary>
    public partial class Monitor : Window {
        public Monitor() {
            InitializeComponent();

            var videoFile = Information.WorkPath.Enter("bg.mp4").Path;
            if (File.Exists(videoFile)) {
                uiBg.Source = new Uri(videoFile, UriKind.Absolute);
                uiBg.LoadedBehavior = MediaState.Manual;
                uiBg.Play();
            }
        }

        private void uiBg_MediaEnded(object sender, RoutedEventArgs e) {
            uiBg.Position = TimeSpan.FromMilliseconds(2 * 1000 + 400);
            uiBg.Play();
        }
    }
}
