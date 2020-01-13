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
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;

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

            currentPage = MonitorPage.Welcome;
        }

        #region public interface

        MonitorPage currentPage;

        public void Interface_SetSize(int width, int height) {
            this.Width = width;
            this.Height = height;
        }

        public void Interface_SwitchPage(MonitorPage pages) {
            if (currentPage == pages) return;
            //disappear
            var sb = new Storyboard();
            var animationClose = new DoubleAnimation();
            animationClose.From = 1;
            animationClose.To = 0;
            animationClose.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            switch (currentPage) {
                case MonitorPage.Welcome:
                    Storyboard.SetTarget(animationClose, this.uiPageWelcome);
                    break;
                case MonitorPage.Timer:
                    Storyboard.SetTarget(animationClose, this.uiPageTimer);
                    break;
                case MonitorPage.Participant:
                    Storyboard.SetTarget(animationClose, this.uiPageParticipant);
                    break;
                case MonitorPage.MapPool:
                    Storyboard.SetTarget(animationClose, this.uiPageMapPool);
                    break;
                case MonitorPage.Grouping:
                    Storyboard.SetTarget(animationClose, this.uiPageGrouping);
                    break;
                case MonitorPage.MapPicker:
                    Storyboard.SetTarget(animationClose, this.uiPageMapPicker);
                    break;
                case MonitorPage.Competition:
                    Storyboard.SetTarget(animationClose, this.uiPageCompetition);
                    break;
                default:
                    break;
            }
            Storyboard.SetTargetProperty(animationClose, new PropertyPath("Opacity"));
            sb.Children.Add(animationClose);

            //appear
            var animationShow = new DoubleAnimation();
            animationShow.From = 0;
            animationShow.To = 1;
            animationShow.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            switch (pages) {
                case MonitorPage.Welcome:
                    Storyboard.SetTarget(animationShow, this.uiPageWelcome);
                    break;
                case MonitorPage.Timer:
                    Storyboard.SetTarget(animationShow, this.uiPageTimer);
                    break;
                case MonitorPage.Participant:
                    Storyboard.SetTarget(animationShow, this.uiPageParticipant);
                    break;
                case MonitorPage.MapPool:
                    Storyboard.SetTarget(animationShow, this.uiPageMapPool);
                    break;
                case MonitorPage.Grouping:
                    Storyboard.SetTarget(animationShow, this.uiPageGrouping);
                    break;
                case MonitorPage.MapPicker:
                    Storyboard.SetTarget(animationShow, this.uiPageMapPicker);
                    break;
                case MonitorPage.Competition:
                    Storyboard.SetTarget(animationShow, this.uiPageCompetition);
                    break;
                default:
                    break;
            }
            Storyboard.SetTargetProperty(animationShow, new PropertyPath("Opacity"));
            sb.Children.Add(animationShow);

            sb.Begin();
            currentPage = pages;
        }


        #endregion

        #region internal control

        private void uiBg_MediaEnded(object sender, RoutedEventArgs e) {
            uiBg.Position = TimeSpan.FromMilliseconds(2 * 1000 + 400);
            uiBg.Play();
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, 0x00A1, 2, 0);
        }

        #endregion
    }
}
