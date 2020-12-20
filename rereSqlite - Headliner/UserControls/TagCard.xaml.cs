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
        private AppBehind appBehind;

        public TagCard() {
            InitializeComponent();
            Prepare();
        }

        public AppBehind AppBehind {
            set {
                appBehind = value;
                FontFamily = new FontFamily(appBehind.FontFamily);
                FontSize = appBehind.FontSize;
            }
        }

        public string NewTag { get; set; }

        public string OldTag { get; set; }

        private void Prepare() {
            DataContext = this;
            RegisterButton.IsEnabled = false;
        }

        private void Insert() {
            TagMaster.Register(true, appBehind, NewTag, OldTag);
            RegisterButton.IsEnabled = false;
        }

        private void Update() {
            TagMaster.Register(false, appBehind, NewTag, OldTag);
            RegisterButton.IsEnabled = false;
        }

        private void PerformRegister() {
            if (string.IsNullOrEmpty(OldTag))
                Insert();
            else
                Update();
        }

        private void TagInput_Change(object sender, RoutedEventArgs e) {
            NewTag = TagInput.Text;
            RegisterButton.IsEnabled = !OldTag.Equals(NewTag);
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