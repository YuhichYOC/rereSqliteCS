/*
*
* BinaryCard.cs
*
* Copyright 2020 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using rereSqlite___Headliner.Data;

namespace rereSqlite___Headliner.UserControls {
    public partial class BinaryCard {
        private AppBehind appBehind;

        private string fileFullPath;

        private bool hasBinaryInDB;

        public BinaryCard() {
            InitializeComponent();
            Prepare();
        }

        public AppBehind AppBehind {
            set {
                appBehind = value;
                FontFamily = new FontFamily(appBehind.FontFamily);
                FontSize = appBehind.FontSize;
                TagInputList.AppBehind = appBehind;
                TagInputList.TagChanged += TagChanged;
            }
        }

        public string Key { get; set; }

        public string FileName { get; set; }

        public bool HasBinaryInDb {
            get => hasBinaryInDB;
            set {
                hasBinaryInDB = value;
                RetrieveFileButton.IsEnabled = hasBinaryInDB;
            }
        }

        private void Prepare() {
            DataContext = this;
            fileFullPath = @"";
            SelectFileButton.IsEnabled = true;
            RetrieveFileButton.IsEnabled = false;
            RegisterButton.IsEnabled = false;
        }

        public void FillTagInput(List<object> candidates, List<object> tags) {
            TagInputList.SetCandidates(candidates);
            TagInputList.SetTags(tags);
        }

        private bool AnyValueChanged() {
            if (!string.IsNullOrEmpty(fileFullPath)) return true;
            return hasBinaryInDB && TagInputList.AnyTagChanged();
        }

        private void PerformSelectFile() {
            var of = new OFWindow {AppBehind = appBehind};
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            fileFullPath = of.SelectedPath;
            FileName = Path.GetFileName(fileFullPath);
            FileNameOutput.Content = FileName;
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void PerformRetrieveFile() {
            var of = new OFWindow {AppBehind = appBehind};
            of.CreateNewFile();
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            using var outputStream = new FileStream(of.SelectedPath, FileMode.Create, FileAccess.Write);
            BinaryStorage.Retrieve(appBehind, Key, outputStream);
        }

        private void InsertFile() {
            BinaryStorage.Register(true, appBehind, Key, fileFullPath, FileName, TagInputList.GetTags());
            HasBinaryInDb = true;
            TagInputList.Refresh();
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void UpdateFile() {
            BinaryStorage.Register(false, appBehind, Key, fileFullPath, FileName, TagInputList.GetTags());
            HasBinaryInDb = true;
            TagInputList.Refresh();
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void PerformRegister() {
            if (HasBinaryInDb)
                UpdateFile();
            else
                InsertFile();
        }

        private void TagChanged() {
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e) {
            try {
                PerformSelectFile();
            }
            catch (Exception ex) {
                appBehind.AppendError(ex.Message, ex);
            }
        }

        private void RetrieveFile_Click(object sender, RoutedEventArgs e) {
            try {
                PerformRetrieveFile();
            }
            catch (Exception ex) {
                appBehind.AppendError(ex.Message, ex);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            try {
                PerformRegister();
            }
            catch (Exception ex) {
                appBehind.AppendError(ex.Message, ex);
            }
        }
    }
}