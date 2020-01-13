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
    /// CompetitionItem.xaml 的交互逻辑
    /// </summary>
    public partial class CompetitionItem : UserControl {
        public CompetitionItem() {
            InitializeComponent();
        }

        public void ApplyData(string name, string time) {
            uiCompetitionDesc.Text = name;
            uiCompetitionTime.Text = time;
        }
    }
}
