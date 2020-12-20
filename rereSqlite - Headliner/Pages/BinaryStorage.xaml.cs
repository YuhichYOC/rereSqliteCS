/*
*
* BinaryStorage.cs
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
using System.Windows;
using System.Windows.Media;
using rereSqlite___Headliner.Data;
using rereSqlite___Headliner.UserControls;

namespace rereSqlite___Headliner.Pages {
    public partial class BinaryStorage {
        private AppBehind appBehind;

        public BinaryStorage() {
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

        private void Prepare() {
            DataContext = this;
        }

        public void FillTagInput() {
            TagInput.Items.Add(new KeyValuePair<string, string>(@"", @""));
            new Data.TagMaster().Query(appBehind).ForEach(row => {
                TagInput.Items.Add(new KeyValuePair<string, string>(row[0].ToString(), row[0].ToString()));
            });
        }

        private void PerformSelect() {
            FillCardList(new Data.BinaryStorage().Query(appBehind, KeyInput.Text, TagInput.SelectedValue));
        }

        private void FillCardList(List<List<object>> rows) {
            CardList.Children.Clear();
            var keyHit = false;
            rows.ForEach(row => {
                AddCard(row[0].ToString(), row[1].ToString(), true, new Thickness(0, 2, 0, 0));
                if (KeyInput.Text.Equals(row[0].ToString())) keyHit = true;
            });
            if (keyHit) return;
            if (string.IsNullOrEmpty(KeyInput.Text)) return;
            AddCard(KeyInput.Text, @"", false, new Thickness(0, 2, 0, 0));
        }

        private void AddCard(string key, string fileName, bool hasBinaryInDB, Thickness margin) {
            CardList.Children.Add(new BinaryCard {
                AppBehind = appBehind,
                Key = key,
                FileName = fileName,
                HasBinaryInDb = hasBinaryInDB,
                Margin = margin
            });
            ((BinaryCard) CardList.Children[^1]).FillTagInput(new BinaryTags().Query(appBehind, key));
        }

        private void Search_Click(object sender, RoutedEventArgs e) {
            try {
                PerformSelect();
            }
            catch (Exception ex) {
                appBehind.AppendError(ex.Message, ex);
            }
        }
    }
}