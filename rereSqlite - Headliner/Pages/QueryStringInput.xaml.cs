/*
*
* QueryStringInput.cs
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
using rereSqlite___Headliner.Accessor;

namespace rereSqlite___Headliner.Pages {
    public partial class QueryStringInput {
        private QueryChunk qc;

        public QueryStringInput() {
            InitializeComponent();
        }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            ExecuteButton.IsEnabled = true;
            BeginButton.IsEnabled = true;
            CommitButton.IsEnabled = false;
            RollbackButton.IsEnabled = false;
        }

        public void SetQueryString(string arg) {
            QueryInput.Text = arg;
        }

        private void PerformExecute() {
            var openNew = !(null != qc && qc.TransactionAlreadyBegun);
            if (openNew) {
                qc = new QueryChunk();
                qc.Open();
            }

            qc.AddCommand(QueryInput.Text);
            qc.Execute();
            if (openNew) qc.Close();
        }

        private void PerformBegin() {
            qc?.Close();
            qc = new QueryChunk();
            qc.Begin();
            BeginButton.IsEnabled = false;
            CommitButton.IsEnabled = true;
            RollbackButton.IsEnabled = true;
        }

        private void PerformCommit() {
            qc.Commit();
            qc.Close();
            BeginButton.IsEnabled = true;
            CommitButton.IsEnabled = false;
            RollbackButton.IsEnabled = false;
        }

        private void PerformRollback() {
            qc.Rollback();
            qc.Close();
            BeginButton.IsEnabled = true;
            CommitButton.IsEnabled = false;
            RollbackButton.IsEnabled = false;
        }

        #region -- Event Handlers --

        private void Execute_Click(object sender, RoutedEventArgs e) {
            try {
                PerformExecute();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        private void Begin_Click(object sender, RoutedEventArgs e) {
            try {
                PerformBegin();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        private void Commit_Click(object sender, RoutedEventArgs e) {
            try {
                PerformCommit();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        private void Rollback_Click(object sender, RoutedEventArgs e) {
            try {
                PerformRollback();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        #endregion
    }
}