/*
*
* StringCard.cs
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

namespace rereSqlite___Headliner.UserControls {
    public partial class StringCard {
        private string keyString;

        private string valueString;

        public StringCard() {
            InitializeComponent();
        }

        public string Key {
            get => keyString;
            set {
                keyString = value;
                KeyOutput.Content = keyString;
            }
        }

        public string Value {
            get => valueString;
            set {
                valueString = value;
                ValueInput.Text = valueString;
            }
        }

        public string OriginalValue { get; set; }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            TagInputList.TagChanged += TagChanged;
            RegisterButton.IsEnabled = false;
        }

        public void FillTagInput(List<object> candidates, List<object> tags) {
            TagInputList.SetCandidates(candidates);
            TagInputList.SetTags(tags);
        }

        private bool AnyValueChanged() {
            if (string.IsNullOrEmpty(Value)) return false;
            return null != OriginalValue && !OriginalValue.Equals(Value) || TagInputList.AnyTagChanged();
        }

        private void Insert() {
            StringStorage.Register(true, Key, Value, TagInputList.GetTags());
            OriginalValue = Value;
            TagInputList.Refresh();
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void Update() {
            StringStorage.Register(false, Key, Value, TagInputList.GetTags());
            OriginalValue = Value;
            TagInputList.Refresh();
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        private void PerformRegister() {
            if (string.IsNullOrEmpty(OriginalValue))
                Insert();
            else
                Update();
        }

        private void TagChanged() {
            RegisterButton.IsEnabled = AnyValueChanged();
        }

        #region -- Event Handlers --

        private void ValueInput_Change(object sender, RoutedEventArgs e) {
            Value = ValueInput.Text;
            RegisterButton.IsEnabled = AnyValueChanged();
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