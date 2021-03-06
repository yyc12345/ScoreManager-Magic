﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ScoreManager_Magic.Core {

    public class WindowsRegistryHelper {

        public WindowsRegistryHelper() {
            var operKey = tryOpenAndInitRegistry();

            FullScreen = Convert.ToBoolean((int)operKey.GetValue("Fullscreen"));

            if (operKey.GetValueKind("TargetDir") == RegistryValueKind.String) TargetDir = operKey.GetValue("TargetDir").ToString();
            else {
                //REG_MULTI_SZ process
                var cache = (string[])operKey.GetValue("TargetDir");
                TargetDir = cache[0];
            }

            Language = (BallanceLanguage)(int)operKey.GetValue("Language");

            SetupCommand = operKey.GetValue("SetupCommand").ToString();

            var (width, height) = decodeResolutionValue(uint.Parse(operKey.GetValue("VideoMode").ToString()));
            ResolutionWidth = width;
            ResolutionHeight = height;

            //backup values
            BackupValues();

            operKey.Dispose();
        }

        public bool FullScreen { get; set; }
        public string TargetDir { get; set; }
        public BallanceLanguage Language { get; set; }
        public string SetupCommand { get; set; }
        public ushort ResolutionWidth { get; set; }
        public ushort ResolutionHeight { get; set; }

        private bool backups_FullScreen;
        private string backups_TargetDir;
        private BallanceLanguage backups_Language;
        private string backups_SetupCommand;
        private ushort backups_ResolutionWidth;
        private ushort backups_ResolutionHeight;

        private RegistryKey tryOpenAndInitRegistry() {
            var rootKey = Registry.LocalMachine;
            RegistryKey locateKey;
            if (Environment.Is64BitOperatingSystem) locateKey = rootKey.OpenSubKey(@"SOFTWARE\Wow6432Node\ballance\Settings", true);
            else locateKey = rootKey.OpenSubKey(@"SOFTWARE\ballance\Settings", true);

            //no subkey
            if (locateKey is null) {
                if (Environment.Is64BitOperatingSystem) {
                    rootKey.CreateSubKey(@"SOFTWARE\Wow6432Node\ballance\Settings");
                    locateKey = rootKey.OpenSubKey(@"SOFTWARE\Wow6432Node\ballance\Settings", true);
                } else {
                    rootKey.CreateSubKey(@"SOFTWARE\ballance\Settings");
                    locateKey = rootKey.OpenSubKey(@"SOFTWARE\ballance\Settings", true);
                }
            }

            if (locateKey.GetValue("Fullscreen") is null) locateKey.SetValue("FullScreen", 1, RegistryValueKind.DWord);
            if (locateKey.GetValue("TargetDir") is null) locateKey.SetValue("TargetDir", "", RegistryValueKind.String);
            if (locateKey.GetValue("Language") is null) locateKey.SetValue("Language", 1, RegistryValueKind.DWord);
            if (locateKey.GetValue("SetupCommand") is null) locateKey.SetValue("SetupCommand", "", RegistryValueKind.String);
            if (locateKey.GetValue("VideoMode") is null) locateKey.SetValue("VideoMode", computeResolutionValue(800, 600), RegistryValueKind.DWord);

            return locateKey;
        }

        private uint computeResolutionValue(ushort width, ushort height) {
            var str = width.ToString("X4") + height.ToString("X4");
            return uint.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        private (ushort width, ushort height) decodeResolutionValue(uint encodedNum) {
            var str = encodedNum.ToString("X8");
            return (ushort.Parse(str.Substring(0, 4), System.Globalization.NumberStyles.HexNumber),
                ushort.Parse(str.Substring(4, 4), System.Globalization.NumberStyles.HexNumber));
        }

        private void BackupValues() {
            backups_FullScreen = FullScreen;
            backups_TargetDir = TargetDir;
            backups_Language = Language;
            backups_SetupCommand = SetupCommand;
            backups_ResolutionWidth = ResolutionWidth;
            backups_ResolutionHeight = ResolutionHeight;
        }

        public void RecoverValues() {
            FullScreen = backups_FullScreen;
            TargetDir = backups_TargetDir;
            Language = backups_Language;
            SetupCommand = backups_SetupCommand;
            ResolutionWidth = backups_ResolutionWidth;
            ResolutionHeight = backups_ResolutionHeight;

            ForceWrite();
        }

        public void ForceWrite() {
            var operKey = tryOpenAndInitRegistry();

            operKey.SetValue("Fullscreen", Convert.ToInt32(FullScreen), RegistryValueKind.DWord);
            operKey.SetValue("TargetDir", TargetDir, RegistryValueKind.String);
            operKey.SetValue("Language", (int)Language, RegistryValueKind.DWord);
            operKey.SetValue("SetupCommand", SetupCommand, RegistryValueKind.String);
            operKey.SetValue("VideoMode", computeResolutionValue(ResolutionWidth, ResolutionHeight), RegistryValueKind.DWord);

            operKey.Dispose();
        }

    }

    public enum BallanceLanguage : int {
        Germany = 0,
        EnglishOrChinese = 1,
        Spanish = 2,
        Italian = 3,
        French = 4
    }

}
