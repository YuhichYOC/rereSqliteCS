/*
*
* TagMaster.cs
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
using rereSqlite___Headliner.UserControls;

namespace rereSqlite___Headliner.Pages {
    public partial class TagMaster {
        private AppBehind appBehind;

        public TagMaster() {
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
            DataContext = appBehind;
        }

        private void PerformSelect() {
            FillCardList(new Data.TagMaster().Query(appBehind, TagInput.Text));
        }

        private void FillCardList(List<List<object>> rows) {
            CardList.Children.Clear();
            rows.ForEach(row => { AddCard(row[0].ToString(), row[0].ToString(), new Thickness(0, 2, 0, 0)); });
            AddCard(@"", @"", new Thickness(0, 2, 0, 0));
        }

        private void AddCard(string newTag, string oldTag, Thickness margin) {
            CardList.Children.Add(new TagCard {
                AppBehind = appBehind,
                NewTag = newTag,
                OldTag = oldTag,
                Margin = margin
            });
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