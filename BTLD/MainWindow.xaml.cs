using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using SMMLib.Utilities;

namespace BTLD {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            monitorWindow = new Monitor();
            monitorWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            //save
            SharedModule.configManager.Save();
            SharedModule.logManager.Close();
        }


        Monitor monitorWindow;

        private void funcMain_Resolution(object sender, RoutedEventArgs e) {
            try {
                var cache = uiMain_Resolution.Text.Split('x');
                monitorWindow.Interface_SetSize(int.Parse(cache[0]), int.Parse(cache[1]));
            } catch {
                MessageBox.Show("Error");
            }
        }

        private void funcMain_SwitchPage(object sender, RoutedEventArgs e) {
            switch (((Button)sender).Name) {
                case "uiMain_PageWelcome":
                    monitorWindow.Interface_SwitchPage(MonitorPage.Welcome);
                    break;
                case "uiMain_PageTimer":
                    monitorWindow.Interface_SwitchPage(MonitorPage.Timer);
                    break;
                case "uiMain_PageParticipant":
                    monitorWindow.Interface_SwitchPage(MonitorPage.Participant);
                    break;
                case "uiMain_PageMapPool":
                    monitorWindow.Interface_SwitchPage(MonitorPage.MapPool);
                    break;
                case "uiMain_PageGrouping":
                    monitorWindow.Interface_SwitchPage(MonitorPage.Grouping);
                    break;
                case "uiMain_PageMapPicker":
                    monitorWindow.Interface_SwitchPage(MonitorPage.MapPicker);
                    break;
                case "uiMain_PageCompetition":
                    monitorWindow.Interface_SwitchPage(MonitorPage.Competition);
                    break;
                default:
                    break;
            }
        }

        private void funcMain_VideoControl(object sender, RoutedEventArgs e) {
            monitorWindow.Interface_VideoControl(((Button)sender).Name == "uiMain_VideoPlay");
        }



        private void funcTimer_SetTimer(object sender, RoutedEventArgs e) {
            try {
                var cache = uiTimer_Time.Text.Split('-');
                monitorWindow.Interface_SetTimer(int.Parse(cache[0]) * 60 + int.Parse(cache[1]));
            } catch {
                MessageBox.Show("Error");
            }
        }



        private void funcParticipant_Read(object sender, RoutedEventArgs e) {
            uiParticipant_ParticipantList.Items.Clear();

            foreach (var i in Directory.GetFiles(Information.WorkPath.Enter("user").Path, "*.jpg", SearchOption.TopDirectoryOnly)) {
                uiParticipant_ParticipantList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(i));
            }

        }

        private void funcParticipant_Apply(object sender, RoutedEventArgs e) {
            var ls = new List<string>();
            foreach (var item in uiParticipant_ParticipantList.Items) {
                ls.Add((string)item);
            }
            monitorWindow.Interface_SetParticipant(ls);
        }



        private void funcMapPool_Read(object sender, RoutedEventArgs e) {
            uiMapPool_MapPoolList.Items.Clear();

            foreach (var i in Directory.GetFiles(Information.WorkPath.Enter("map").Path, "*.jpg", SearchOption.TopDirectoryOnly)) {
                uiMapPool_MapPoolList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(i));
            }
        }

        private void funcMapPool_Apply(object sender, RoutedEventArgs e) {
            var ls = new List<string>();
            foreach (var item in uiMapPool_MapPoolList.Items) {
                ls.Add((string)item);
            }
            monitorWindow.Interface_SetMapPool(ls);
        }



        private void funcGrouping_Read(object sender, RoutedEventArgs e) {
            uiGrouping_EnableList.Items.Clear();
            uiGrouping_DisableList.Items.Clear();

            foreach (var item in uiParticipant_ParticipantList.Items) {
                uiGrouping_EnableList.Items.Add((string)item);
            }
        }

        private void funcGrouping_Apply(object sender, RoutedEventArgs e) {
            var ls = new List<string>();
            foreach (var item in uiGrouping_EnableList.Items) {
                ls.Add((string)item);
            }
            monitorWindow.Interface_SetGrouping(ls);
        }

        private void funcGrouping_Pick(object sender, RoutedEventArgs e) {
            MessageBox.Show($"Pick result: {monitorWindow.Interface_PickGrouping()}");
        }

        private void funcGrouping_LeftShift(object sender, RoutedEventArgs e) {
            if (uiGrouping_DisableList.SelectedIndex == -1) return;
            uiGrouping_EnableList.Items.Add((string)uiGrouping_DisableList.SelectedItem);
            uiGrouping_DisableList.Items.RemoveAt(uiGrouping_DisableList.SelectedIndex);
        }

        private void funcGrouping_RightShift(object sender, RoutedEventArgs e) {
            if (uiGrouping_EnableList.SelectedIndex == -1) return;
            uiGrouping_DisableList.Items.Add((string)uiGrouping_EnableList.SelectedItem);
            uiGrouping_EnableList.Items.RemoveAt(uiGrouping_EnableList.SelectedIndex);
        }



        private void funcMapPicker_Read(object sender, RoutedEventArgs e) {
            uiMapPicker_EnableList.Items.Clear();
            uiMapPicker_DisableList.Items.Clear();

            foreach (var item in uiMapPool_MapPoolList.Items) {
                uiMapPicker_EnableList.Items.Add((string)item);
            }
        }

        private void funcMapPicker_Apply(object sender, RoutedEventArgs e) {
            var ls1 = new List<string>();
            var ls2 = new List<string>();
            foreach (var item in uiMapPicker_EnableList.Items) 
                ls1.Add((string)item);
            foreach (var item in uiMapPicker_DisableList.Items)
                ls2.Add((string)item);

            monitorWindow.Interface_SetMapPicker(ls1, ls2);
        }

        private void funcMapPicker_Pick(object sender, RoutedEventArgs e) {
            MessageBox.Show($"Pick result: {monitorWindow.Interface_PickMap()}");
        }

        private void funcMapPicker_LeftShift(object sender, RoutedEventArgs e) {
            if (uiMapPicker_DisableList.SelectedIndex == -1) return;
            uiMapPicker_EnableList.Items.Add((string)uiMapPicker_DisableList.SelectedItem);
            uiMapPicker_DisableList.Items.RemoveAt(uiMapPicker_DisableList.SelectedIndex);
        }

        private void funcMapPicker_RightShift(object sender, RoutedEventArgs e) {
            if (uiMapPicker_EnableList.SelectedIndex == -1) return;
            uiMapPicker_DisableList.Items.Add((string)uiMapPicker_EnableList.SelectedItem);
            uiMapPicker_EnableList.Items.RemoveAt(uiMapPicker_EnableList.SelectedIndex);
        }



        private void funcCompetition_Add(object sender, RoutedEventArgs e) {
            try {
                var input = Microsoft.VisualBasic.Interaction.InputBox("Input competition words(with format name/date):");
                var cache = input.Split('/');
                if (cache.Length < 2) throw new ArgumentException();
                uiCompetition_PastList.Items.Add(input);
            } catch  {
                MessageBox.Show("Error");
            }
        }

        private void funcCompetition_Rm(object sender, RoutedEventArgs e) {
            if (uiCompetition_PastList.SelectedIndex == -1) return;
            uiCompetition_PastList.Items.RemoveAt(uiCompetition_PastList.SelectedIndex);
        }

        private void funcCompetition_Apply(object sender, RoutedEventArgs e) {
            var ls1 = new List<string>();
            var ls2 = new List<string>();
            foreach (var item in uiCompetition_PastList.Items)
                ls1.Add((string)item);
            foreach (var item in uiCompetition_NowList.Items)
                ls2.Add((string)item);

            monitorWindow.Interface_SetCompetition(ls1, ls2);
        }

        private void uiCompetition_LeftShift(object sender, RoutedEventArgs e) {
            if (uiCompetition_NowList.SelectedIndex == -1) return;
            uiCompetition_PastList.Items.Add((string)uiCompetition_NowList.SelectedItem);
            uiCompetition_NowList.Items.RemoveAt(uiCompetition_NowList.SelectedIndex);
        }

        private void funcCompetition_RightShift(object sender, RoutedEventArgs e) {
            if (uiCompetition_PastList.SelectedIndex == -1) return;
            uiCompetition_NowList.Items.Add((string)uiCompetition_PastList.SelectedItem);
            uiCompetition_PastList.Items.RemoveAt(uiCompetition_PastList.SelectedIndex);
        }
    }
}
