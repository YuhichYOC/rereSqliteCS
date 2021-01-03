/*
*
* TagInputList.cs
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
using System.Windows.Controls;

namespace rereSqlite___Headliner.UserControls {
    public partial class TagInputList {
        private const int MaxTagCount = 5;

        private List<object> candidates;

        private List<string> originalValues;

        public TagInputList() {
            InitializeComponent();
            candidates = new List<object>();
            originalValues = new List<string>();
        }

        public Action TagChanged { get; set; }

        public void SetCandidates(List<object> arg) {
            candidates = arg;
        }

        public void SetTags(List<object> arg) {
            if (MaxTagCount < arg.Count) throw new ArgumentException(@"There are too many tags.");
            originalValues = new List<string>();
            arg.ForEach(t => {
                if (null == t || DBNull.Value == t) return;
                originalValues.Add(t.ToString());
                AddChild(t.ToString());
            });
            originalValues.Sort();
            if (MaxTagCount > arg.Count) AddChild(@"");
        }

        public bool AnyTagChanged() {
            var now = GetTags();
            if (originalValues.Count != now.Count) return true;
            now.Sort();
            return originalValues.Where((t, i) => !t.Equals(now[i])).Any();
        }

        public List<object> GetTags() {
            var ret = new List<object>();
            for (var i = 0; Tags.Children.Count > i; ++i) {
                if (string.IsNullOrEmpty(GetValueAt(i))) continue;
                ret.Add(GetValueAt(i));
            }

            return ret;
        }

        public void Refresh() {
            originalValues.Clear();
            GetTags().ForEach(t => originalValues.Add(t.ToString()));
        }

        private void RemoveDuplicate() {
            for (var i = 0; Tags.Children.Count > i; ++i)
            for (var j = Tags.Children.Count - 1; i < j; --j)
                if (GetValueAt(i).Equals(GetValueAt(j)))
                    Tags.Children.RemoveAt(j);
        }

        private void RemoveBlank() {
            for (var i = Tags.Children.Count - 1; 0 <= i; --i)
                if (string.IsNullOrEmpty(GetValueAt(i)))
                    Tags.Children.RemoveAt(i);
        }

        private void AddChild(string initialValue) {
            if (MaxTagCount <= Tags.Children.Count) return;
            var add = new ComboBox();
            add.Items.Add(new KeyValuePair<string, string>(@"", @""));
            candidates.ForEach(c => { add.Items.Add(new KeyValuePair<string, string>(c.ToString(), c.ToString())); });
            add.DisplayMemberPath = @"Value";
            add.SelectedValuePath = @"Key";
            add.SelectedValue = initialValue;
            add.SelectionChanged += Selected;
            add.Margin = new Thickness(2);
            Tags.Children.Add(add);
        }

        private string GetValueAt(int i) {
            if (Tags.Children.Count - 1 < i) throw new IndexOutOfRangeException();
            return null == ((ComboBox) Tags.Children[i]).SelectedValue
                ? @""
                : (string) ((ComboBox) Tags.Children[i]).SelectedValue;
        }

        #region -- Event Handlers --

        private void Selected(object sender, RoutedEventArgs e) {
            try {
                RemoveDuplicate();
                RemoveBlank();
                AddChild(@"");
                TagChanged?.Invoke();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        #endregion
    }
}