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

namespace ScoreManager_Magic.UI.Windows {
    /// <summary>
    /// ConfigurationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigurationWindow : Window {
        public ConfigurationWindow() {
            InitializeComponent();

            //bind event
            SharedModule.logSystem.NewLog += (obj) => {
                this.Dispatcher.Invoke(new Action(() => {
                    if (uiLogLoglist.Items.Count == 100) uiLogLoglist.Items.RemoveAt(0);
                    uiLogLoglist.Items.Add(obj);
                    uiLogLoglist.ScrollIntoView(uiLogLoglist.Items[uiLogLoglist.Items.Count - 1]);
                }));
            };
        }

    }
}
