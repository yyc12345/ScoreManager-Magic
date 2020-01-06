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

        private void SetClipboard(string str) {
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void func_assumeUtc(object sender, RoutedEventArgs e) {
            SharedModule.configManager.Configuration["AssumeUTC"] = uiSetting_AssumeUTC.IsChecked.UniformBoolean().ConvertToInt().ToString();
            SharedModule.configManager.Save();
        }

        private void func_displayUtc(object sender, RoutedEventArgs e) {
            SharedModule.configManager.Configuration["DisplayUTC"] = uiSetting_DisplayUTC.IsChecked.UniformBoolean().ConvertToInt().ToString();
            SharedModule.configManager.Save();
        }

        private void func_dropMapfile(object sender, DragEventArgs e) {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            try {
                var file = files[0];
                uiAddMap_Hash.Text = HashComput.SHA256FromFile(file);

                var info = new System.IO.FileInfo(file);
                uiAddMap_Name.Text = info.Name;
            } catch {
                MessageBox.Show("Fail to read file.", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region menu oper

        private void func_menuUserJsonName(object sender, RoutedEventArgs e) {
            var ls = new List<string>();

            foreach (var item in uiQueryUserResult.SelectedItems) ls.Add(((Data.OperationUserQuery)item).sm_name);
            SetClipboard(JsonConvert.SerializeObject(ls));
        }
        private void func_menuUserName(object sender, RoutedEventArgs e) {
            var index = uiQueryUserResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(userList[index].sm_name);
        }


        private void func_menuCompetitionJsonID(object sender, RoutedEventArgs e) {
            var ls = new List<long>();

            foreach (var item in uiQueryCompetitionResult.SelectedItems) ls.Add(((Data.OperationCompetitionQuery)item).sm_id);
            SetClipboard(JsonConvert.SerializeObject(ls));
        }
        private void func_menuCompetitionID(object sender, RoutedEventArgs e) {
            var index = uiQueryCompetitionResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(competitionList[index].sm_id.ToString());
        }
        private void func_menuCompetitionMap(object sender, RoutedEventArgs e) {
            var index = uiQueryCompetitionResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(competitionList[index].sm_map.ToString());
        }
        private void func_menuCompetitionBanMap(object sender, RoutedEventArgs e) {
            var index = uiQueryCompetitionResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(competitionList[index].sm_map.ToString());
        }


        private void func_menuRecordScore(object sender, RoutedEventArgs e) {
            var index = uiQueryRecordResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(recordList[index].sm_score.ToString());
        }
        private void func_menuRecordSRTime(object sender, RoutedEventArgs e) {
            var index = uiQueryRecordResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(recordList[index].sm_srTime.ToString());
        }
        private void func_menuRecordMap(object sender, RoutedEventArgs e) {
            var index = uiQueryRecordResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(recordList[index].sm_map);
        }

        private void func_menuTournamentName(object sender, RoutedEventArgs e) {
            var index = uiQueryTournamentResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(tournamentList[index].sm_tournament);
        }
        private void func_menuTournamentSchedule(object sender, RoutedEventArgs e) {
            var index = uiQueryTournamentResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(tournamentList[index].sm_schedule);
        }


        private void func_menuRegistryName(object sender, RoutedEventArgs e) {
            var index = uiQueryRegistryResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(registryList[index].sm_user);
        }
        private void func_menuRegistryTournament(object sender, RoutedEventArgs e) {
            var index = uiQueryRegistryResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(registryList[index].sm_tournament);
        }


        private void func_menuMapPoolHash(object sender, RoutedEventArgs e) {
            var index = uiQueryMapPoolResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(mapPoolList[index].sm_hash);
        }
        private void func_menuMapPoolTournament(object sender, RoutedEventArgs e) {
            var index = uiQueryMapPoolResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(mapPoolList[index].sm_tournament);
        }


        private void func_menuMapJsonHash(object sender, RoutedEventArgs e) {
            var ls = new List<string>();

            foreach (var item in uiQueryMapResult.SelectedItems) ls.Add(((Data.OperationMapQuery)item).sm_hash);
            SetClipboard(JsonConvert.SerializeObject(ls));
        }
        private void func_menuMapHash(object sender, RoutedEventArgs e) {
            var index = uiQueryMapResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(mapList[index].sm_hash);
        }
        private void func_menuMapName(object sender, RoutedEventArgs e) {
            var index = uiQueryMapResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(mapList[index].sm_name);
        }
        private void func_menuMapI18N(object sender, RoutedEventArgs e) {
            var index = uiQueryMapResult.SelectedIndex;
            if (index == -1) return;
            SetClipboard(mapList[index].sm_i18n);
        }



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
        private void func_addUser(object sender, RoutedEventArgs e) {
            var cachePriority = SM_Priority.None;
            if (uiAddUser_Priority_user.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.User;
            if (uiAddUser_Priority_live.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.Live;
            if (uiAddUser_Priority_speedrun.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.Speedrun;
            if (uiAddUser_Priority_admin.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.Admin;

            var status = SharedModule.smmcore.OperationUser_Add(new SMMLib.Data.SMMInputBuilder.UserAddBuilder(
                uiAddUser_Name.Text,
                uiAddUser_Password.Text,
                cachePriority));
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_deleteUser(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationUser_Delete(JsonConvert.DeserializeObject<List<string>>(uiDeleteUser_Name.Text));
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_updateUser(object sender, RoutedEventArgs e) {
            var cachePriority = SM_Priority.None;
            if (uiUpdateUser_Priority_user.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.User;
            if (uiUpdateUser_Priority_live.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.Live;
            if (uiUpdateUser_Priority_speedrun.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.Speedrun;
            if (uiUpdateUser_Priority_admin.IsChecked.UniformBoolean()) cachePriority |= SM_Priority.Admin;

            var status = SharedModule.smmcore.OperationUser_Update(JsonConvert.DeserializeObject<List<string>>(uiUpdateUser_Name.Text),
                new SMMLib.Data.SMMInputBuilder.UserUpdateFilter(
                    uiUpdateUser_WhetherPassword.IsChecked.UniformBoolean(),
                    uiUpdateUser_WhetherPriority.IsChecked.UniformBoolean(),
                    uiUpdateUser_WhetherExpireOn.IsChecked.UniformBoolean(),

                    uiUpdateUser_Password.Text,
                    cachePriority,
                    uiUpdateUser_ExpireOn.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC)
                    ));
            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
        private void func_addCompetition(object sender, RoutedEventArgs e) {
            var (status, inserID) = SharedModule.smmcore.OperationCompetition_Add(new SMMLib.Data.SMMInputBuilder.CompetitionAddBuilder(
                uiAddCompetition_StartDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiAddCompetition_EndDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiAddCompetition_JudgeEndDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                JsonConvert.DeserializeObject<List<string>>(uiAddCompetition_Participant.Text)));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished" + Environment.NewLine + $"New competition ID is: {inserID}", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_deleteCompetition(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationCompetition_Delete(JsonConvert.DeserializeObject<List<long>>(uiDeleteCompetition_ID.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_updateCompetition(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationCompetition_Update(long.Parse(uiUpdateCompetition_ID.Text),
                new SMMLib.Data.SMMInputBuilder.CompetitionUpdateFilter(
                    uiUpdateCompetition_WhetherResult.IsChecked.UniformBoolean(),
                    uiUpdateCompetition_WhetherMap.IsChecked.UniformBoolean(),
                    uiUpdateCompetition_WhetherBanMap.IsChecked.UniformBoolean(),
                    uiUpdateCompetition_WhetherWinner.IsChecked.UniformBoolean(),

                    uiUpdateCompetition_Result.Text,
                    uiUpdateCompetition_Map.Text,
                    uiUpdateCompetition_BanMap.Text,
                    uiUpdateCompetition_Winner.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void func_queryRecord(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationRecord_Query(new SMMLib.Data.SMMInputBuilder.RecordQueryFilter(
                uiQueryRecord_WhetherInstallOn.IsChecked.UniformBoolean(),
                uiQueryRecord_WhetherName.IsChecked.UniformBoolean(),
                uiQueryRecord_WhetherStartDate.IsChecked.UniformBoolean(),
                uiQueryRecord_WhetherEndDate.IsChecked.UniformBoolean(),
                uiQueryRecord_WhetherScore.IsChecked.UniformBoolean(),
                uiQueryRecord_WhetherSRTime.IsChecked.UniformBoolean(),
                uiQueryRecord_WhetherMap.IsChecked.UniformBoolean(),

                uiQueryRecord_InstallOn.Text.ConvertToInt(),
                uiQueryRecord_Name.Text,
                uiQueryRecord_StartDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiQueryRecord_EndDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiQueryRecord_Score.Text.ConvertToInt(),
                uiQueryRecord_SRTime.Text.ConvertToInt(),
                uiQueryRecord_Map.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            recordList.Clear();
            foreach (var item in data)
                recordList.Add(new Data.OperationRecordQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void func_queryTournament(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationTournament_Query(new SMMLib.Data.SMMInputBuilder.TournamentQueryFilter(
                uiQueryTournament_WhetherName.IsChecked.UniformBoolean(),
                uiQueryTournament_WhetherVagueName.IsChecked.UniformBoolean() ? $"%{uiQueryTournament_Name.Text}%" : uiQueryTournament_Name.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            tournamentList.Clear();
            foreach (var item in data)
                tournamentList.Add(new Data.OperationTournamentQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_addTournament(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationTournament_Add(new SMMLib.Data.SMMInputBuilder.TournamentAddBuilder(
                uiAddTournament_StartDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiAddTournament_EndDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiAddTournament_Name.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_deleteTournament(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationTournament_Delete(uiDeleteTournament_Name.Text);

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_updateTournament(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationTournament_Update(uiUpdateTournament_Name.Text, new SMMLib.Data.SMMInputBuilder.TournamentUpdateFilter(
                uiUpdateTournament_WhetherStartDate.IsChecked.UniformBoolean(),
                uiUpdateTournament_WhetherEndDate.IsChecked.UniformBoolean(),
                uiUpdateTournament_WhetherSchedule.IsChecked.UniformBoolean(),

                uiUpdateTournament_StartDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiUpdateTournament_EndDate.GetDatetimePickerData().ConvertToTimestamp(WhetherAssumeUTC),
                uiUpdateTournament_Schedule.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void func_queryRegistry(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationRegistry_Query(new SMMLib.Data.SMMInputBuilder.RegistryQueryFilter(
                uiQueryRegistry_WhetherName.IsChecked.UniformBoolean(),
                uiQueryRegistry_WhetherTournament.IsChecked.UniformBoolean(),

                uiQueryRegistry_WhetherVagueName.IsChecked.UniformBoolean() ? $"%{uiQueryRegistry_Name.Text}%" : uiQueryRegistry_Name.Text,
                uiQueryRegistry_Tournament.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            registryList.Clear();
            foreach (var item in data)
                registryList.Add(new Data.OperationRegistryQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_addRegistry(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationRegistry_Add(new SMMLib.Data.SMMInputBuilder.RegistryAddDeleteBuilder(
                uiAddRegistry_User.Text,
                uiAddRegistry_Tournament.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_deleteRegistry(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationRegistry_Delete(new SMMLib.Data.SMMInputBuilder.RegistryAddDeleteBuilder(
                uiDeleteRegistry_Name.Text,
                uiDeleteRegistry_Tournament.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void func_queryMapPool(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationMapPool_Query(new SMMLib.Data.SMMInputBuilder.MapPoolQueryFilter(
                uiQueryMapPool_WhetherHash.IsChecked.UniformBoolean(),
                uiQueryMapPool_WhetherTournament.IsChecked.UniformBoolean(),

                uiQueryMapPool_Hash.Text,
                uiQueryMapPool_Tournament.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mapPoolList.Clear();
            foreach (var item in data)
                mapPoolList.Add(new Data.OperationMapPoolQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_addMapPool(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationMapPool_Add(new SMMLib.Data.SMMInputBuilder.MapPoolAddDeleteBuilder(
                uiAddMapPool_Hash.Text,
                uiAddMapPool_Tournament.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_deleteMapPool(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationMapPool_Delete(new SMMLib.Data.SMMInputBuilder.MapPoolAddDeleteBuilder(
                uiDeleteMapPool_Hash.Text,
                uiDeleteMapPool_Tournament.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void func_queryMap(object sender, RoutedEventArgs e) {
            var (status, data) = SharedModule.smmcore.OperationMap_Query(new SMMLib.Data.SMMInputBuilder.MapQueryFilter(
                uiQueryMap_WhetherName.IsChecked.UniformBoolean(),
                uiQueryMap_WhetherI18N.IsChecked.UniformBoolean(),
                uiQueryMap_WhetherHash.IsChecked.UniformBoolean(),

                uiQueryMap_Name.Text,
                uiQueryMap_I18N.Text,
                uiQueryMap_Hash.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mapList.Clear();
            foreach (var item in data)
                mapList.Add(new Data.OperationMapQuery(item));
            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_addMap(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationMap_Add(new SMMLib.Data.SMMInputBuilder.MapAddBuilder(
                uiAddMap_Name.Text,
                uiAddMap_I18N.Text,
                uiAddMap_Hash.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void func_deleteMap(object sender, RoutedEventArgs e) {
            var status = SharedModule.smmcore.OperationMap_Delete(JsonConvert.DeserializeObject<List<string>>(uiDeleteMap_Hash.Text));

            if (!status.IsSuccess) {
                MessageBox.Show("Error: " + status.Description, "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Operation finished", "Mahoushoujo", MessageBoxButton.OK, MessageBoxImage.Information);
        }





        #endregion

    }
}
