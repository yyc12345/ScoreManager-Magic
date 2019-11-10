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

namespace ScoreManager_Magic {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        private ConfigurationWindow configWindow = new ConfigurationWindow();

        public MainWindow() {
            InitializeComponent();
        }

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
        private void uiMenuConfig_Click(object sender, RoutedEventArgs e) {
            configWindow.Show();
        }

        private void uiMenuExit_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }

        #endregion



    }
}
