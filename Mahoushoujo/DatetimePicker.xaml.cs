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

namespace Mahoushoujo {
    /// <summary>
    /// DatetimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class DatetimePicker : UserControl {
        public DatetimePicker() {
            InitializeComponent();

            //init combox
            for (int i = 0; i < 24; i++) uiHour.Items.Add(i.ToString().PadLeft(2, '0'));
            for (int i = 0; i < 60; i++) {
                uiMinute.Items.Add(i.ToString().PadLeft(2, '0'));
                uiSecond.Items.Add(i.ToString().PadLeft(2, '0'));
            }

            uiHour.SelectedIndex = 0;
            uiMinute.SelectedIndex = 0;
            uiSecond.SelectedIndex = 0;

            uiDatepicker.SelectedDate = DateTime.Now;
        }

        public DateTime GetDatetimePickerData() {
            return DateTime.Now;
        }
    }
}
