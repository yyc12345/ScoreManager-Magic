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
using System.Windows.Threading;
using BTLD.UIControls;

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
                //uiBg.Play();
            }

            globalRnd = new Random();

            currentPage = MonitorPage.Welcome;
            videoIsPlaying = false;

            userItemsCache = new List<UserItem>();
            groupingItemsCache = new List<UserItem>();
            mapItemsCache = new List<MapItem>();
            mapPickerItemsCache = new List<MapItem>();
            competitionItemsCache = new List<CompetitionItem>();

            remainGroupingUsers = new Dictionary<string, int>();
            remainPickMaps = new List<string>();
        }

        #region public interface
        Random globalRnd;

        MonitorPage currentPage;
        bool videoIsPlaying;
        DispatcherTimer dispatcherTimer;
        int timerCounter;

        List<UserItem> userItemsCache;
        List<UserItem> groupingItemsCache;
        List<MapItem> mapItemsCache;
        List<MapItem> mapPickerItemsCache;
        List<CompetitionItem> competitionItemsCache;

        Dictionary<string, int> remainGroupingUsers;
        DispatcherTimer groupingScrollTimer;
        List<string> remainPickMaps;
        DispatcherTimer mapPickerScrollTimer;

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
            //set target and restore z index
            switch (currentPage) {
                case MonitorPage.Welcome:
                    Storyboard.SetTarget(animationClose, this.uiPageWelcome);
                    Panel.SetZIndex(this.uiPageWelcome, -1);
                    break;
                case MonitorPage.Timer:
                    Storyboard.SetTarget(animationClose, this.uiPageTimer);
                    Panel.SetZIndex(this.uiPageTimer, -1);
                    break;
                case MonitorPage.Participant:
                    Storyboard.SetTarget(animationClose, this.uiPageParticipant);
                    Panel.SetZIndex(this.uiPageParticipant, -1);
                    break;
                case MonitorPage.MapPool:
                    Storyboard.SetTarget(animationClose, this.uiPageMapPool);
                    Panel.SetZIndex(this.uiPageMapPool, -1);
                    break;
                case MonitorPage.Grouping:
                    Storyboard.SetTarget(animationClose, this.uiPageGrouping);
                    Panel.SetZIndex(this.uiPageGrouping, -1);
                    break;
                case MonitorPage.MapPicker:
                    Storyboard.SetTarget(animationClose, this.uiPageMapPicker);
                    Panel.SetZIndex(this.uiPageMapPicker, -1);
                    break;
                case MonitorPage.Competition:
                    Storyboard.SetTarget(animationClose, this.uiPageCompetition);
                    Panel.SetZIndex(this.uiPageCompetition, -1);
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
                    Panel.SetZIndex(this.uiPageWelcome, 1);
                    break;
                case MonitorPage.Timer:
                    Storyboard.SetTarget(animationShow, this.uiPageTimer);
                    Panel.SetZIndex(this.uiPageTimer, 1);
                    break;
                case MonitorPage.Participant:
                    Storyboard.SetTarget(animationShow, this.uiPageParticipant);
                    Panel.SetZIndex(this.uiPageParticipant, 1);
                    break;
                case MonitorPage.MapPool:
                    Storyboard.SetTarget(animationShow, this.uiPageMapPool);
                    Panel.SetZIndex(this.uiPageMapPool, 1);
                    break;
                case MonitorPage.Grouping:
                    Storyboard.SetTarget(animationShow, this.uiPageGrouping);
                    Panel.SetZIndex(this.uiPageGrouping, 1);
                    break;
                case MonitorPage.MapPicker:
                    Storyboard.SetTarget(animationShow, this.uiPageMapPicker);
                    Panel.SetZIndex(this.uiPageMapPicker, 1);
                    break;
                case MonitorPage.Competition:
                    Storyboard.SetTarget(animationShow, this.uiPageCompetition);
                    Panel.SetZIndex(this.uiPageCompetition, 1);
                    break;
                default:
                    break;
            }
            Storyboard.SetTargetProperty(animationShow, new PropertyPath("Opacity"));
            sb.Children.Add(animationShow);

            sb.Begin();
            currentPage = pages;
        }

        public void Interface_VideoControl(bool isPlaying) {
            if (videoIsPlaying == isPlaying) return;
            if (isPlaying) uiBg.Play();
            else uiBg.Pause();

            videoIsPlaying = isPlaying;
        }

        public void Interface_SetTimer(int second) {
            if (!(dispatcherTimer is null)) dispatcherTimer.Stop();
            timerCounter = second;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (sender, e) => {
                timerCounter--;
                uiTimer.Text = $"{(timerCounter / 60).ToString().PadLeft(2, '0')} : {(timerCounter % 60).ToString().PadLeft(2, '0')}";
                if (timerCounter == 0) dispatcherTimer.Stop();
            };
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        public void Interface_SetParticipant(List<string> names) {
            var cacheSize = userItemsCache.Count;
            for (int i = 0; i < names.Count - cacheSize; i++) {
                var cache = new UserItem();
                cache.Margin = new Thickness(10);
                userItemsCache.Add(cache);
            }

            uiParticipantList.Children.Clear();
            for (int i = 0; i < names.Count; i++) {
                userItemsCache[i].ApplyUsername(names[i]);
                userItemsCache[i].ApplyUserAvatar(names[i]);
                userItemsCache[i].ApplyEnable(true);
                uiParticipantList.Children.Add(userItemsCache[i]);
            }
        }

        public void Interface_SetMapPool(List<string> maps) {
            var cacheSize = mapItemsCache.Count;
            for (int i = 0; i < maps.Count - cacheSize; i++) {
                var cache = new MapItem();
                cache.Margin = new Thickness(10);
                mapItemsCache.Add(cache);
            }

            uiMapPoolList.Children.Clear();
            for (int i = 0; i < maps.Count; i++) {
                mapItemsCache[i].ApplyMapName(maps[i]);
                mapItemsCache[i].ApplyMapImage(maps[i]);
                mapItemsCache[i].ApplyEnable(true);
                uiMapPoolList.Children.Add(mapItemsCache[i]);
            }
        }

        public void Interface_SetGrouping(List<string> names) {
            var cacheSize = groupingItemsCache.Count;
            for (int i = 0; i < names.Count - cacheSize; i++) {
                var cache = new UserItem();
                cache.Margin = new Thickness(10);
                groupingItemsCache.Add(cache);
            }

            uiUserPickerList.Children.Clear();
            remainGroupingUsers.Clear();
            for (int i = 0; i < names.Count; i++) {
                //add ui
                groupingItemsCache[i].ApplyUsername(names[i]);
                groupingItemsCache[i].ApplyUserAvatar(names[i]);
                groupingItemsCache[i].ApplyEnable(true);
                uiUserPickerList.Children.Add(groupingItemsCache[i]);

                //apply dict
                remainGroupingUsers.Add(names[i], i);
            }

            //reset ui
            uiUserPicker.ApplyUsername("");
            uiUserPicker.ApplyUserAvatar("");
        }

        public string Interface_PickGrouping() {
            if (remainGroupingUsers.Count == 0) return "";

            var len = remainGroupingUsers.Count;
            var rnd = globalRnd.Next(0, len);
            var animationGrouping = remainGroupingUsers.Keys.ToArray();
            var picked = animationGrouping[rnd];
            var disabledItem = remainGroupingUsers[picked];
            var animationGroupingCounter = globalRnd.Next(20, 40);

            //rm selected item
            remainGroupingUsers.Remove(picked);

            //set ui
            uiUserPicker.ApplyUsername("");
            uiUserPicker.ApplyUserAvatar("");

            if (!(groupingScrollTimer is null)) groupingScrollTimer.Stop();
            groupingScrollTimer = new DispatcherTimer();
            groupingScrollTimer.Interval = TimeSpan.FromMilliseconds(50);
            groupingScrollTimer.Tick += (sender, e) => {
                if (animationGroupingCounter == 0) {
                    groupingScrollTimer.Stop();
                    uiUserPicker.ApplyUsername(picked);
                    uiUserPicker.ApplyUserAvatar(picked);

                    groupingItemsCache[disabledItem].ApplyEnable(false);
                } else {
                    uiUserPicker.ApplyUsername(animationGrouping[globalRnd.Next(0, len)]);
                    animationGroupingCounter--;
                }
            };
            groupingScrollTimer.Start();

            return picked;
        }

        public void Interface_SetMapPicker(List<string> maps, List<string> banMaps) {
            var cacheSize = mapPickerItemsCache.Count;
            var lenMaps = maps.Count;
            var lenBanMaps = banMaps.Count;
            for (int i = 0; i < lenMaps + lenBanMaps - cacheSize; i++) {
                var cache = new MapItem();
                cache.Margin = new Thickness(10);
                mapPickerItemsCache.Add(cache);
            }

            uiMapPickerList.Children.Clear();
            remainPickMaps.Clear();
            for (int i = 0; i < lenBanMaps; i++) {
                //add ui
                mapPickerItemsCache[i].ApplyMapName(banMaps[i]);
                mapPickerItemsCache[i].ApplyMapImage(banMaps[i]);
                mapPickerItemsCache[i].ApplyEnable(false);
                uiMapPickerList.Children.Add(mapPickerItemsCache[i]);
            }
            for (int i = 0; i < lenMaps; i++) {
                //add ui
                mapPickerItemsCache[i + lenBanMaps].ApplyMapName(maps[i]);
                mapPickerItemsCache[i + lenBanMaps].ApplyMapImage(maps[i]);
                mapPickerItemsCache[i + lenBanMaps].ApplyEnable(true);
                uiMapPickerList.Children.Add(mapPickerItemsCache[i + lenBanMaps]);

                //reg
                remainPickMaps.Add(maps[i]);
            }

            //reset ui
            uiMapPicker.ApplyMapName("");
            uiMapPicker.ApplyMapImage("");

        }

        public string Interface_PickMap() {
            if (remainPickMaps.Count == 0) return "";

            var len = remainPickMaps.Count;
            var rnd = globalRnd.Next(0, len);
            var picked = remainPickMaps[rnd];
            var counter = globalRnd.Next(20, 40);

            //set ui
            uiMapPicker.ApplyMapName("");
            uiMapPicker.ApplyMapImage("");

            if (!(mapPickerScrollTimer is null)) mapPickerScrollTimer.Stop();
            mapPickerScrollTimer = new DispatcherTimer();
            mapPickerScrollTimer.Interval = TimeSpan.FromMilliseconds(50);
            mapPickerScrollTimer.Tick += (sender, e) => {
                if (counter == 0) {
                    mapPickerScrollTimer.Stop();
                    uiMapPicker.ApplyMapName(picked);
                    uiMapPicker.ApplyMapImage(picked);
                } else {
                    uiMapPicker.ApplyMapName(remainPickMaps[globalRnd.Next(0, len)]);
                    counter--;
                }
            };
            mapPickerScrollTimer.Start();

            return picked;
        }

        public void Interface_SetCompetition(List<string> past, List<string> now) {
            var cacheSize = competitionItemsCache.Count;
            var lenPast = past.Count;
            var lenNow = now.Count;
            for (int i =0;i<lenPast+ lenNow - cacheSize;i++) {
                var cache = new CompetitionItem();
                cache.Margin = new Thickness(10);
                competitionItemsCache.Add(cache);
            }

            uiCompetitionPastList.Children.Clear();
            uiCompetitionNowList.Children.Clear();
            for (int i =0;i<lenPast;i++) {
                var cache = past[i].Split('/');
                competitionItemsCache[i].ApplyData(cache[0], cache[1]);
                uiCompetitionPastList.Children.Add(competitionItemsCache[i]);
            }
            for (int i =0;i<lenNow;i++) {
                var cache = now[i].Split('/');
                competitionItemsCache[i + lenPast].ApplyData(cache[0], cache[1]);
                uiCompetitionNowList.Children.Add(competitionItemsCache[i + lenPast]);
            }
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
