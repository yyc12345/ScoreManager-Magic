using ScoreManager_Magic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Threading.Tasks;
using SMMLib.Utilities;

namespace ScoreManager_Magic.UI.Windows {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        private ConfigurationWindow configWindow = new ConfigurationWindow();

        public MainWindow() {
            InitializeComponent();
            configWindow.Show();

            uiStatus.Text = "空闲";

            //bind event
            SharedModule.SelectCompetitionCallback += (hash, name, cdk) => {
                this.Dispatcher.Invoke(new Action(() => {
                    uiMap.Text = name;
                    uiCDK.Text = cdk;
                    currentHash = hash;
                }));
            };

            SharedModule.anticheat.DetectCheat += (str) => {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("检测到如下文件被修改，即将自动结束游戏：" + str, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    uiStatus.Text = "空闲";
                    uiStatus.Foreground = new SolidColorBrush(Colors.White);
                    uiTime.Text = "--:--:--.----";
                    uiScore.Text = "----";

                    uiMenuStop.IsEnabled = false;
                    uiMenuStart.IsEnabled = true;
                    uiMenuExit.IsEnabled = true;
                    configWindow.FreezeUI(false);
                }));

                playingHash = "";

                //kill ballance
                Process[] ps = Process.GetProcesses();
                foreach (Process item in ps) {
                    if (item.ProcessName.ToLower() == "player") {
                        item.Kill();
                    }
                }

                //stop watching
                SharedModule.anticheat.StopAnticheat();
                SharedModule.nemosWatcher.StartMonitor();
            };

            SharedModule.nemosWatcher.LevelStart += async (index) => {
                await TaskEx.Run(() => {
                    var file = index.ToString().PadLeft(2, '0');
                    playingHash = HashComput.SHA256FromFile(SharedModule.configManager.Configuration["BallancePath"] + $"\\3D Entities\\Level\\Level_{file}.NMO");
                });

                this.Dispatcher.Invoke(new Action(() => {
                    if (playingHash == currentHash) uiMap.Foreground = new SolidColorBrush(Colors.LightGreen);
                    else uiMap.Foreground = new SolidColorBrush(Colors.Red);

                    uiTime.Text = "--:--:--.----";
                    uiScore.Text = "----";

                    uiStatus.Text = "正在进行游戏";
                    uiStatus.Foreground = new SolidColorBrush(Colors.Yellow);
                }));

            };

            SharedModule.nemosWatcher.LevelEndButFail += () => {
                this.Dispatcher.Invoke(new Action(() => {
                    uiStatus.Text = "准备就绪";
                    uiStatus.Foreground = new SolidColorBrush(Colors.LightGreen);
                    uiMap.Foreground = new SolidColorBrush(Colors.White);
                }));
            };

            SharedModule.nemosWatcher.LevelEnd += async (data) => {
                var status = await TaskEx.Run(() => {
                    return SharedModule.smm.Submit(
                        data.InstallOn,
                        playingHash,
                        data.HSScore,
                        data.SRTime,
                        data.LifeUp,
                        data.LifeLost,
                        data.ExtraPoints,
                        data.SubExtraPoints,
                        data.Trafo,
                        data.Checkpoint,
                        data.Verify,
                        data.Token);
                });

                SharedModule.Raise_NewSubmitCallback(playingHash, data);
                playingHash = "";

                this.Dispatcher.Invoke(new Action(() => {
                    uiStatus.Text = "准备就绪";
                    uiStatus.Foreground = new SolidColorBrush(Colors.LightGreen);
                    uiTime.Text = data.SRTime.SRTimeFormat();
                    uiScore.Text = data.HSScore.ToString();
                    uiMap.Foreground = new SolidColorBrush(Colors.White);
                }));
            };
        }

        string currentHash = "";
        string playingHash = "";

        #region window operation

        /// <summary>
        /// moving window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiBaseGrid_MouseDown(object sender, MouseButtonEventArgs e) {
            Win32Window.SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, Win32Window.WM_NCLBUTTONDOWN, (int)Win32Window.HitTest.HTCAPTION, 0);
        }

        /// <summary>
        /// core processor for listen window size change
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {

            //当鼠标输入且窗口大小正常时接受大小改变
            if (msg == Win32Window.WM_NCHITTEST && this.WindowState == WindowState.Normal) {
                return WmNCHitTest(lParam, ref handled);
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// method for process window size change
        /// </summary>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WmNCHitTest(IntPtr lParam, ref bool handled) {

            Point mousePoint = new Point();
            //边角识别宽度
            int cornerWidth = 8;
            //边框宽度
            int customBorderThickness = 5;
            // Update cursor point  
            // The low-order word specifies the x-coordinate of the cursor.  
            // #define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))  
            mousePoint.X = (int)(short)(lParam.ToInt32() & 0xFFFF);
            // The high-order word specifies the y-coordinate of the cursor.  
            // #define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))  
            mousePoint.Y = (int)(short)(lParam.ToInt32() >> 16);

            // Do hit test  
            handled = true;
            if (Math.Abs(mousePoint.Y - Top) <= cornerWidth
                && Math.Abs(mousePoint.X - Left) <= cornerWidth) { // Top-Left  
                handled = false;
                return IntPtr.Zero;
                //return new IntPtr((int)Win32.HitTest.HTTOPLEFT);
            } else if (Math.Abs(ActualHeight + Top - mousePoint.Y) <= cornerWidth
                  && Math.Abs(mousePoint.X - Left) <= cornerWidth) { // Bottom-Left  
                return new IntPtr((int)Win32Window.HitTest.HTBOTTOMLEFT);
            } else if (Math.Abs(mousePoint.Y - Top) <= cornerWidth
                  && Math.Abs(ActualWidth + Left - mousePoint.X) <= cornerWidth) { // Top-Right  
                handled = false;
                return IntPtr.Zero;
                //return new IntPtr((int)Win32.HitTest.HTTOPRIGHT);
            } else if (Math.Abs(ActualWidth + Left - mousePoint.X) <= cornerWidth
                  && Math.Abs(ActualHeight + Top - mousePoint.Y) <= cornerWidth) { // Bottom-Right  
                return new IntPtr((int)Win32Window.HitTest.HTBOTTOMRIGHT);
            } else if (Math.Abs(mousePoint.X - Left) <= customBorderThickness) { // Left  
                return new IntPtr((int)Win32Window.HitTest.HTLEFT);
            } else if (Math.Abs(ActualWidth + Left - mousePoint.X) <= customBorderThickness) { // Right  
                return new IntPtr((int)Win32Window.HitTest.HTRIGHT);
            } else if (Math.Abs(mousePoint.Y - Top) <= customBorderThickness) { // Top  
                handled = false;
                return IntPtr.Zero;
                //return new IntPtr((int)Win32.HitTest.HTTOP);
            } else if (Math.Abs(ActualHeight + Top - mousePoint.Y) <= customBorderThickness) { // Bottom  
                return new IntPtr((int)Win32Window.HitTest.HTBOTTOM);
            } else {
                handled = false;
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// registry window size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SourceInitialized(object sender, EventArgs e) {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");

            source.AddHook(new HwndSourceHook(this.WndProc));
        }




        #endregion

        #region menu

        private void uiMenuExit_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }

        private void uiMenuStart_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("游戏即将启动，可能会消耗一些时间", "ScoreManager-Magic", MessageBoxButton.OK);
            uiMenuStart.IsEnabled = false;
            uiMenuExit.IsEnabled = false;
            configWindow.FreezeUI(true);

            try {
                //apply registry
                SharedModule.registryHelper.TargetDir = SharedModule.configManager.Configuration["BallancePath"];
                SharedModule.registryHelper.ForceWrite();

                //start nemos and anticheat
                SharedModule.nemosWatcher.StartMonitor();
                var result = SharedModule.anticheat.StartAnticheat();
                if (!result) throw new InvalidOperationException("检测到Ballance文件不正确，请确保使用原版Ballance");

            } catch (Exception ee) {
                MessageBox.Show("启动失败，原因如下：" + ee.Message, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                uiMenuStart.IsEnabled = true;
                uiMenuExit.IsEnabled = true;
                configWindow.FreezeUI(false);
                return;
            }

            //try create shortcut
            try {
                var ballancePath = SharedModule.configManager.Configuration["BallancePath"];
                var shell = new WshShell();
                var shortcut = (IWshShortcut)shell.CreateShortcut("rungame.lnk");
                shortcut.TargetPath = ballancePath + "\\Bin\\Player.exe";
                shortcut.WorkingDirectory = ballancePath + "\\Bin\\";

                shortcut.Save();
                Process.Start("rungame.lnk");
                System.IO.File.Delete("rungame.lnk");
            } catch {
                MessageBox.Show("由于一些错误，我们无法帮助您启动游戏，您需要自行前往游戏目录下手动启动游戏，程序将会自动监测", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            uiStatus.Text = "准备就绪";
            uiStatus.Foreground = new SolidColorBrush(Colors.LightGreen);
            uiMenuStop.IsEnabled = true;
        }

        private void uiMenuStop_Click(object sender, RoutedEventArgs e) {
            uiMenuStop.IsEnabled = false;

            uiStatus.Text = "空闲";
            uiStatus.Foreground = new SolidColorBrush(Colors.White);
            uiTime.Text = "--:--:--.----";
            uiScore.Text = "----";

            playingHash = "";

            //kill ballance
            Process[] ps = Process.GetProcesses();
            foreach (Process item in ps) {
                if (item.ProcessName.ToLower() == "player") {
                    item.Kill();
                }
            }

            //stop watching
            SharedModule.anticheat.StopAnticheat();
            SharedModule.nemosWatcher.StartMonitor();

            uiMenuStart.IsEnabled = true;
            uiMenuExit.IsEnabled = true;
            configWindow.FreezeUI(false);
        }
        #endregion


    }
}
