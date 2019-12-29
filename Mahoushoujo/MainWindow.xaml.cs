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
using SMMLib.Net;
using SMMLib.Utilities;
using SMMLib.Data;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Mahoushoujo {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            SharedModule.logSystem = new LogManager(Information.WorkPath.Enter("mahoushoujo.log").Path);
            SMMLib.Net.NetworkMethod.NewRequest += (url, parameter) => {
                SharedModule.logSystem.WriteLog("[Network][Info]New request. URL: " + url + " Parameter: " + parameter);
            };
            SharedModule.logSystem.NewLog += (obj) => {
                this.Dispatcher.Invoke(new Action(() => {
                    if (uiLog_Loglist.Items.Count == 100) uiLog_Loglist.Items.RemoveAt(0);
                    uiLog_Loglist.Items.Add(obj);
                    uiLog_Loglist.ScrollIntoView(uiLog_Loglist.Items[uiLog_Loglist.Items.Count - 1]);
                }));
            };

            App.Current.Exit += (obj, e) => {
                SharedModule.logSystem.Close();
            };

            uiLoginDomain.Text = SharedModule.configManager.Configuration["Server"];
            uiSetting_AssumeUTC.IsChecked = SharedModule.configManager.Configuration["AssumeUTC"].ConvertToBoolean();
            uiSetting_DisplayUTC.IsChecked = SharedModule.configManager.Configuration["DisplayUTC"].ConvertToBoolean();

            //bind data context
            uiQueryUserResult.DataContext = userList;
            uiQueryCompetitionResult.DataContext = competitionList;
            uiQueryRecordResult.DataContext = recordList;
            uiQueryTournamentResult.DataContext = tournamentList;
            uiQueryRegistryResult.DataContext = registryList;
            uiQueryMapPoolResult.DataContext = mapPoolList;
            uiQueryMapResult.DataContext = mapList;
        }


        #region score manager related

        ObservableCollection<Data.OperationUserQuery> userList = new ObservableCollection<Data.OperationUserQuery>();
        ObservableCollection<Data.OperationCompetitionQuery> competitionList = new ObservableCollection<Data.OperationCompetitionQuery>();
        ObservableCollection<Data.OperationRecordQuery> recordList = new ObservableCollection<Data.OperationRecordQuery>();
        ObservableCollection<Data.OperationTournamentQuery> tournamentList = new ObservableCollection<Data.OperationTournamentQuery>();
        ObservableCollection<Data.OperationRegistryQuery> registryList = new ObservableCollection<Data.OperationRegistryQuery>();
        ObservableCollection<Data.OperationMapPoolQuery> mapPoolList = new ObservableCollection<Data.OperationMapPoolQuery>();
        ObservableCollection<Data.OperationMapQuery> mapList = new ObservableCollection<Data.OperationMapQuery>();

        #endregion

        #region login

        private void funcLogin(object sender, RoutedEventArgs e) {
            SharedModule.smmcore.ChangeDomain(uiLoginDomain.Text);
            //test version
            var (status, version) = SharedModule.smmcore.Version();
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Information.SMMProtocolVersion != version) {
                MessageBox.Show("Unmatched SMM protocol version", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //login
            var cache = SharedModule.smmcore.Login(uiLoginUserName.Text, uiLoginPassword.Password);
            if (!cache.IsSuccess) {
                MessageBox.Show("Fail to login: " + cache.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!SharedModule.smmcore.Priority.HasFlag(SM_Priority.Admin)) {
                MessageBox.Show("Not enough permission", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SharedModule.configManager.Configuration["Server"] = uiLoginDomain.Text;
            SharedModule.configManager.Save();
            MessageBox.Show("Log in successfully!", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
            uiLoginLayer.Visibility = Visibility.Collapsed;
            uiOperLayer.Visibility = Visibility.Visible;
        }



        #endregion

        #region drag/misc oper

        private bool WhetherAssumeUTC {
            get { return SharedModule.configManager.Configuration["AssumeUTC"].ConvertToBoolean(); }
        }

        private void func_assumeUtc(object sender, RoutedEventArgs e) {
            SharedModule.configManager.Configuration["AssumeUTC"] = uiSetting_AssumeUTC.IsChecked.UniformBoolean().ConvertToInt().ToString();
            SharedModule.configManager.Save();
        }

        private void func_displayUtc(object sender, RoutedEventArgs e) {
            SharedModule.configManager.Configuration["DisplayUTC"] = uiSetting_DisplayUTC.IsChecked.UniformBoolean().ConvertToInt().ToString();
            SharedModule.configManager.Save();
        }

        #endregion

        #region menu oper




        #endregion

        #region oper

        private void func_queryUser(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationUser_Query(new SMMLib.Data.SMMInputBuilder.UserQueryFilter(
                uiQueryUser_WhetherName.IsChecked.UniformBoolean(),
                uiQueryUser_WhetherVagueName.IsChecked.UniformBoolean() ? $"%{uiQueryUser_Name.Text}%" : uiQueryUser_Name.Text));
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            userList.Clear();
            foreach (var item in data)
                userList.Add(new Data.OperationUserQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void func_queryCompetition(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationCompetition_Query(new SMMLib.Data.SMMInputBuilder.CompetitionQueryFilter(
                uiQueryCompetition_WhetherID.IsChecked.UniformBoolean(),
                uiQueryCompetition_WhetherName.IsChecked.UniformBoolean(),
                uiQueryCompetition_WhetherStartDate.IsChecked.UniformBoolean(),
                uiQueryCompetition_WhetherEndDate.IsChecked.UniformBoolean(),
                uiQueryCompetition_WhetherCDK.IsChecked.UniformBoolean(),
                uiQueryCompetition_WhetherMap.IsChecked.UniformBoolean(),

                JsonConvert.DeserializeObject<List<long>>(uiQueryCompetition_ID.Text),
                uiQueryCompetition_Name.Text,
                uiQueryCompetition_StartDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiQueryCompetition_EndDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiQueryCompetition_CDK.Text,
                uiQueryCompetition_Map.Text
                ));
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            competitionList.Clear();
            foreach (var item in data)
                competitionList.Add(new Data.OperationCompetitionQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

    }
}
