using SMMLib.Data;
using SMMLib.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                    if (ui_log_logList.Items.Count == 100) ui_log_logList.Items.RemoveAt(0);
                    ui_log_logList.Items.Add(obj);
                    ui_log_logList.ScrollIntoView(ui_log_logList.Items[ui_log_logList.Items.Count - 1]);
                }));
            };

            SharedModule.NewSubmitCallback += async (hash, bsm) => {
                var (status, data) = await TaskEx.Run(() => {
                    return SharedModule.smm.GetMapName(new List<string>() { hash });
                });

                if (status.IsSuccess) {
                    if (data.Count != 0) hash = data[0].sm_name;
                }

                this.Dispatcher.Invoke(new Action(() => {
                    this.bsmList.Add(new Data.ObservableBsmData(bsm, hash, DateTime.Now));
                }));
            };

            for (int i = 1; i <= 13; i++)
                ui_game_installMapLevel.Items.Add(i);
            ui_game_installMapLevel.SelectedIndex = 0;
            ui_game_languageList.SelectedIndex = 0;

            //bind ui
            ui_competition_datagrid.DataContext = competitionList;
            ui_tournament_datagrid.DataContext = tournamentList;
            ui_submit_datagrid.DataContext = bsmList;

            //set user page
            ui_user_name.Text = SharedModule.smm.Username;

            var sm_priority = SharedModule.smm.Priority;
            List<string> cachePri = new List<string>();
            if ((sm_priority & SM_Priority.User) == SM_Priority.User) cachePri.Add(SM_Priority.User.ToString());
            if ((sm_priority & SM_Priority.Live) == SM_Priority.Live) cachePri.Add(SM_Priority.Live.ToString());
            if ((sm_priority & SM_Priority.Speedrun) == SM_Priority.Speedrun) cachePri.Add(SM_Priority.Speedrun.ToString());
            if ((sm_priority & SM_Priority.Admin) == SM_Priority.Admin) cachePri.Add(SM_Priority.Admin.ToString());
            if (cachePri.Count == 0) ui_user_priority.Text = "None";
            else ui_user_priority.Text = string.Join(", ", cachePri);

            //setup related data
            ui_game_currentBallancePath.Text = SharedModule.configManager.Configuration["BallancePath"];

            //setup dialog
            gamePathDialog.Filter = "Database.tdb文件|Database.tdb";
            mapSelectorDialog.Filter = "地图文件|*.nmo";
            gamePathDialog.Multiselect = false;
            mapSelectorDialog.Multiselect = false;
        }

        ObservableCollection<Data.ObservableGetFutureCompetition> competitionList = new ObservableCollection<Data.ObservableGetFutureCompetition>();
        ObservableCollection<Data.ObservableGetTournament> tournamentList = new ObservableCollection<Data.ObservableGetTournament>();
        ObservableCollection<Data.ObservableBsmData> bsmList = new ObservableCollection<Data.ObservableBsmData>();

        Microsoft.Win32.OpenFileDialog gamePathDialog = new Microsoft.Win32.OpenFileDialog();
        Microsoft.Win32.OpenFileDialog mapSelectorDialog = new Microsoft.Win32.OpenFileDialog();

        #region misc method

        public void FreezeUI(bool isFreeze) {
                ui_user_changePassword.IsEnabled = !isFreeze;

                ui_game_changeBallancePath.IsEnabled = !isFreeze;
                ui_game_installMap.IsEnabled = !isFreeze;
                ui_game_cleanHighscore.IsEnabled = !isFreeze;
                ui_game_openLevels.IsEnabled = !isFreeze;
                ui_game_changeBallanceLanguage.IsEnabled = !isFreeze;

                ui_competition_refresh.IsEnabled = !isFreeze;
                ui_competition_battle.IsEnabled = !isFreeze;

                ui_tournament_refresh.IsEnabled = !isFreeze;
                ui_tournament_registry.IsEnabled = !isFreeze;
        }

        #endregion

        #region ballance oper method

        private void func_gameChangePath(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            if (!gamePathDialog.ShowDialog().UniformBoolean()) {
                FreezeUI(false);
                return;
            }

            if (gamePathDialog.FileName != "") {
                var path = new FilePathBuilder(gamePathDialog.FileName).Backtracking().Path;
                SharedModule.configManager.Configuration["BallancePath"] = path;
                SharedModule.configManager.Save();

                ui_game_currentBallancePath.Text = path;

                MessageBox.Show($"已将游戏目录修改为：{path}", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
            } else MessageBox.Show("修改失败，因为没有选中文件", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);

            FreezeUI(false);
        }

        private void func_gameInstallMap(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            if (!mapSelectorDialog.ShowDialog().UniformBoolean()) {
                FreezeUI(false);
                return;
            }

            if (mapSelectorDialog.FileName != "") {
                var mapFile = mapSelectorDialog.FileName;
                var replaceFile = new FilePathBuilder(SharedModule.configManager.Configuration["BallancePath"]).Enter("3D Entities").Enter("Level")
                    .Enter($"Level_{(ui_game_installMapLevel.SelectedIndex + 1).ToString().PadLeft(2, '0')}.NMO").Path;

                if (System.IO.File.Exists(mapFile) && System.IO.File.Exists(replaceFile)) {
                    System.IO.File.Copy(mapFile, replaceFile, true);
                    MessageBox.Show("已安装地图", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                } else MessageBox.Show("修改失败，因为文件不存在，可能是Ballance目录设置错误所致", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);

            } else MessageBox.Show("修改失败，因为没有选中文件", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);

            FreezeUI(false);
        }

        private void func_gameCleanHighscore(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            var tdb = new FilePathBuilder(SharedModule.configManager.Configuration["BallancePath"]).Enter("Database.tdb").Path;

            try {
                //get data
                var cache = Core.DatabasetdbChanger.ReadDB(tdb);
                foreach (var item in cache.HighScores) {
                    foreach (var item2 in item.Play) {
                        item2.Player = "-";
                        item2.Points = -1;
                    }
                }

                //rewrite
                Core.DatabasetdbChanger.SaveDB(cache, tdb);
                MessageBox.Show("清理完成", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ee) {
                MessageBox.Show("修改失败，可能是Ballance目录设置错误所致。错误如下：" + ee.Message, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FreezeUI(false);
        }

        private void func_gameOpenLevels(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            var tdb = new FilePathBuilder(SharedModule.configManager.Configuration["BallancePath"]).Enter("Database.tdb").Path;

            try {
                //get data
                var cache = Core.DatabasetdbChanger.ReadDB(tdb);
                for (int i = 0; i < cache.Settings.LevelOpened.Length; i++)
                    cache.Settings.LevelOpened[i] = true;

                //rewrite
                Core.DatabasetdbChanger.SaveDB(cache, tdb);
                MessageBox.Show("开启完成", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ee) {
                MessageBox.Show("修改失败，可能是Ballance目录设置错误所致。错误如下：" + ee.Message, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FreezeUI(false);
        }

        private void func_gameChangeLanguage(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            SharedModule.registryHelper.Language = (Core.BallanceLanguage)ui_game_languageList.SelectedIndex;
            MessageBox.Show("修改完成", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);

            FreezeUI(false);
        }

        #endregion

        #region net oper method

        private async void func_competitionRefresh(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            var (status, data) = await TaskEx.Run(() => {
                return SharedModule.smm.GetFutureCompetition();
            });

            if (!status.IsSuccess) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("刷新失败，由于以下错误：" + status.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    FreezeUI(false);
                }));
                return;
            }

            if (data.Count != 0) {
                //get map message
                var neededMaps = new List<string>();
                foreach (var item in data) {
                    if (item.sm_map != "" && (!neededMaps.Contains(item.sm_map)))
                        neededMaps.Add(item.sm_map);
                }

                var (status2, data2) = await TaskEx.Run(() => {
                    return SharedModule.smm.GetMapName(neededMaps);
                });

                if (!status2.IsSuccess) {
                    this.Dispatcher.Invoke(new Action(() => {
                        MessageBox.Show("刷新失败，由于以下错误：" + status2.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                    return;
                }

                //build map sheet
                var mapSheet = new Dictionary<string, string>();
                foreach (var item in data2)
                    mapSheet.Add(item.sm_hash, item.sm_name);

                //combine data
                this.Dispatcher.Invoke(new Action(() => {
                    competitionList.Clear();
                    foreach (var item in data) {
                        var cache = new Data.ObservableGetFutureCompetition(item);
                        if (mapSheet.ContainsKey(item.sm_map)) cache.conv_map = mapSheet[item.sm_map];
                        else cache.conv_map = "未知地图";
                        competitionList.Add(cache);
                    }

                    MessageBox.Show("刷新成功", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                    FreezeUI(false);
                }));
            } else {
                //no data
                this.Dispatcher.Invoke(new Action(() => {
                    competitionList.Clear();
                    MessageBox.Show("刷新成功", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                    FreezeUI(false);
                }));
            }

        }
        private void func_competitionBattle(object sender, RoutedEventArgs e) {
            var index = ui_competition_datagrid.SelectedIndex;
            if (index == -1) return;

            var data = competitionList[index];
            if (data.sm_map == "") {
                MessageBox.Show("此比赛尚未决定地图，无法选中此比赛", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SharedModule.Raise_SelectCompetitionCallback(data.sm_map, data.conv_map, data.sm_cdk == "" ? "无" : data.sm_cdk);
            MessageBox.Show("已选中此比赛", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private async void func_tournamentRefresh(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            var (status, data) = await TaskEx.Run(() => {
                return SharedModule.smm.GetTournament();
            });

            if (!status.IsSuccess) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("刷新失败，由于以下错误：" + status.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    FreezeUI(false);
                }));
                return;
            }

            this.Dispatcher.Invoke(new Action(() => {
                tournamentList.Clear();

                foreach (var item in data)
                    tournamentList.Add(new Data.ObservableGetTournament(item));
                MessageBox.Show("刷新成功", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                FreezeUI(false);
            }));

        }
        private async void func_tournamentRegistry(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            var index = ui_tournament_datagrid.SelectedIndex;
            if (index == -1) return;
            var sm_tournament = tournamentList[index].sm_tournament;

            var status = await TaskEx.Run(() => {
                return SharedModule.smm.RegisterTournament(sm_tournament);
            });

            if (!status.IsSuccess) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("注册失败，由于以下错误：" + status.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    FreezeUI(false);
                }));
                return;
            }

            this.Dispatcher.Invoke(new Action(() => {
                MessageBox.Show("注册成功", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                FreezeUI(false);
            }));
        }


        private async void func_userChangePassword(object sender, RoutedEventArgs e) {
            FreezeUI(true);

            var newPass = ui_user_newPassword.Password;
            if (newPass == "") {
                MessageBox.Show("新密码不能为空", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var status = await TaskEx.Run(() => {
                return SharedModule.smm.ChangePassword(newPass);
            });

            if (!status.IsSuccess) {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show("修改密码失败，由于以下错误：" + status.Description, "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Error);
                    FreezeUI(false);
                }));
                return;
            }

            this.Dispatcher.Invoke(new Action(() => {
                MessageBox.Show("密码修改成功，将在下次登陆时应用", "ScoreManager-Magic", MessageBoxButton.OK, MessageBoxImage.Information);
                FreezeUI(false);
            }));
        }



        #endregion

    }
}
