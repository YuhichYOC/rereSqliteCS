/*
*
* TagCard.cs
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
using System.Windows;
using System.Windows.Media;
using rereSqlite___Headliner.Data;

namespace rereSqlite___Headliner.UserControls {
    public partial class TagCard {
        private string newTag;

        public TagCard() {
            InitializeComponent();
        }

        public string NewTag {
            get => newTag;
            set {
                newTag = value;
                TagInput.Text = newTag;
            }
        }

        public string OldTag { get; set; }

        private bool ValueChanged() {
            if (string.IsNullOrEmpty(NewTag)) return false;
            if (string.IsNullOrEmpty(OldTag)) return true;
            return !OldTag.Equals(NewTag);
        }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            RegisterButton.IsEnabled = false;
        }

        private void Insert() {
            TagMaster.Register(true, NewTag, OldTag);
            OldTag = NewTag;
            RegisterButton.IsEnabled = ValueChanged();
        }

        private void Update() {
            TagMaster.Register(false, NewTag, OldTag);
            OldTag = NewTag;
            RegisterButton.IsEnabled = ValueChanged();
        }

        private void PerformRegister() {
            if (string.IsNullOrEmpty(OldTag))
                Insert();
            else
                Update();
            AppBehind.Get.Reload();
        }

        #region -- Event Handlers --

        private void TagInput_Change(object sender, RoutedEventArgs e) {
            NewTag = TagInput.Text;
            RegisterButton.IsEnabled = ValueChanged();
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