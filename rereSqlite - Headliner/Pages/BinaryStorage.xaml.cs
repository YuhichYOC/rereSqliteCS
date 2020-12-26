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
using System.Linq;
using System.Windows;
using System.Windows.Media;
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
            TagInput.Items.Clear();
            var add = new Data.TagMaster().Query(appBehind)
                .Aggregate(
                    new List<KeyValuePair<string, string>>(),
                    (ret, row) => {
                        ret.Add(new KeyValuePair<string, string>(row[0].ToString(), row[0].ToString()));
                        return ret;
                    });
            add.Insert(0, new KeyValuePair<string, string>(@"", @""));
            add.ForEach(item => { TagInput.Items.Add(item); });
        }

        private void PerformSelect() {
            Fill();
        }

        private void Fill() {
            CardList.Children.Clear();
            var rows = new Data.BinaryStorage().Query(appBehind, KeyInput.Text, TagInput.SelectedValue);
            var tagCandidates = new Data.TagMaster().Query(appBehind).Select(row => row[0]).ToList();
            var margin = new Thickness(2);
            var keyHit = false;
            foreach (var row in rows.Where(row => null != row[0] && null != row[3] && (long) row[3] == (long) row[4])) {
                AddChild(
                    row[0].ToString(),
                    row[1].ToString(),
                    true,
                    tagCandidates,
                    rows.Where(r => row[0].ToString().Equals(r[0].ToString()))
                        .Select(r => r[2])
                        .ToList(),
                    margin
                );
                if (KeyInput.Text.Equals(row[0].ToString())) keyHit = true;
            }

            if (keyHit || string.IsNullOrEmpty(KeyInput.Text)) return;
            AddChild(KeyInput.Text, @"", false, tagCandidates, new List<object>(), margin);
        }

        private void AddChild(
            string key,
            string fileName,
            bool hasBinaryInDB,
            List<object> tagCandidates,
            List<object> tags,
            Thickness margin
        ) {
            var add = new BinaryCard {
                AppBehind = appBehind,
                Key = key,
                FileName = fileName,
                HasBinaryInDb = hasBinaryInDB,
                Margin = margin
            };
            add.FillTagInput(tagCandidates, tags);
            CardList.Children.Add(add);
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