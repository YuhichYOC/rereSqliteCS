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
        private string fileFullPath;

        private string fileNameString;

        private bool hasBinaryInDB;

        private string keyString;

        public BinaryCard() {
            InitializeComponent();
        }

        public string Key {
            get => keyString;
            set {
                keyString = value;
                KeyOutput.Content = keyString;
            }
        }

        public string FileName {
            get => fileNameString;
            set {
                fileNameString = value;
                FileNameOutput.Content = fileNameString;
            }
        }

        public bool HasBinaryInDb {
            get => hasBinaryInDB;
            set {
                hasBinaryInDB = value;
                RetrieveFileButton.IsEnabled = hasBinaryInDB;
            }
        }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            TagInputList.TagChanged += TagChanged;
            fileFullPath = @"";
            SelectFileButton.IsEnabled = true;
            RetrieveFileButton.IsEnabled = hasBinaryInDB;
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
            var of = new OFWindow(false) {
                AppendErrorDelegate = AppBehind.Get.AppendError,
                RowHeightPlus = AppBehind.Get.DataGridRowHeightPlus,
                OpenButtonCaption = AppBehind.Get.OFWindowCaptions.SelectPathButton,
                NewFileButtonCaption = AppBehind.Get.OFWindowCaptions.SelectPathButton,
                FileNameColumnCaption = AppBehind.Get.OFWindowCaptions.FileNameColumn,
                OverwriteDialogTitle = AppBehind.Get.OFWindowCaptions.OverwriteDialogTitle,
                OverwriteMessage = AppBehind.Get.OFWindowCaptions.OverwriteMessage,
                Title = AppBehind.Get.OFWindowCaptions.SelectFileTitle
            };
            of.Init();
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            fileFullPath = of.SelectedPath;
            FileName = Path.GetFileName(fileFullPath);
            FileNameOutput.Content = FileName;
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void PerformRetrieveFile() {
            var of = new OFWindow(true) {
                AppendErrorDelegate = AppBehind.Get.AppendError,
                RowHeightPlus = AppBehind.Get.DataGridRowHeightPlus,
                OpenButtonCaption = AppBehind.Get.OFWindowCaptions.SaveFileButton,
                NewFileButtonCaption = AppBehind.Get.OFWindowCaptions.SaveFileButton,
                FileNameColumnCaption = AppBehind.Get.OFWindowCaptions.FileNameColumn,
                OverwriteDialogTitle = AppBehind.Get.OFWindowCaptions.OverwriteDialogTitle,
                OverwriteMessage = AppBehind.Get.OFWindowCaptions.OverwriteMessage,
                Title = AppBehind.Get.OFWindowCaptions.RetrieveFileTitle
            };
            of.Init();
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            using var outputStream = new FileStream(of.SelectedPath, FileMode.Create, FileAccess.Write);
            BinaryStorage.Retrieve(Key, outputStream);
        }

        private void InsertFile() {
            BinaryStorage.Register(true, Key, fileFullPath, FileName, TagInputList.GetTags());
            HasBinaryInDb = true;
            TagInputList.Refresh();
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void UpdateFile() {
            BinaryStorage.Register(false, Key, fileFullPath, FileName, TagInputList.GetTags());
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

        #region -- Event Handlers --

        private void SelectFile_Click(object sender, RoutedEventArgs e) {
            try {
                PerformSelectFile();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        private void RetrieveFile_Click(object sender, RoutedEventArgs e) {
            try {
                PerformRetrieveFile();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            try {
                PerformRegister();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        #endregion
    }
}