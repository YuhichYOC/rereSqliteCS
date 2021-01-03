/*
*
* TableList.cs
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
using System.Windows.Input;
using System.Windows.Media;
using rereSqlite___Headliner.Data;

namespace rereSqlite___Headliner.Pages {
    public partial class TableList {
        private Operator tableListOperator;

        public TableList() {
            InitializeComponent();
        }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            tableListOperator = new Operator();
            tableListOperator.Prepare(Tables);
            tableListOperator.AddColumn(@"TableName", AppBehind.Get.TableListCaptions.TableNameColumn);
            tableListOperator.CreateColumns();
            Tables.RowHeight = AppBehind.Get.FontSize + AppBehind.Get.DataGridRowHeightPlus;
        }

        public void FillTableList() {
            if (@"".Equals(AppBehind.Get.DBFilePath)) return;
            FillTableList(QueryTables());
        }

        private List<List<object>> QueryTables() {
            return string.IsNullOrEmpty(FilterInput.Text.Trim())
                ? new Schema().Query()
                : new Schema().Query(FilterInput.Text.Trim());
        }

        private void FillTableList(List<List<object>> queryResult) {
            tableListOperator.Blank();
            queryResult.ForEach(row => {
                var addRow = new RowEntity();
                addRow.TrySetMember(@"TableName", row[0].ToString());
                tableListOperator.AddRow(addRow);
            });

            tableListOperator.Refresh();
        }

        private void PerformQueryWholeTable(string tableName) {
            if (string.IsNullOrEmpty(tableName)) return;
            var tableInfo = new Schema().QueryTableInfo(tableName);
            if (0 == tableInfo.Count) return;
            var query = @" SELECT " + '\n';
            query += tableInfo
                .Select((row, index) => new {index, columnName = row[1]})
                .Aggregate(@"",
                    (ret, item) =>
                        0 == item.index
                            ? ret + @"     " + item.columnName + @" " + '\n'
                            : ret + @"   , " + item.columnName + @" " + '\n');

            query += @" FROM " + '\n';
            query += @"     " + tableName + @" " + '\n';
            var accessor = new SqliteAccessor {
                DataSource = AppBehind.Get.DBFilePath,
                Password = AppBehind.Get.Password,
                QueryString = query
            };
            accessor.Open();
            accessor.Execute(accessor.CreateCommand());
            accessor.Close();
            AppBehind.Get.SetQueryString(query);
            AppBehind.Get.AddPage(accessor);
        }

        #region -- Event Handlers --

        private void Reload_AnyEvent(object sender, RoutedEventArgs e) {
            try {
                FillTableList();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        private void TableList_DoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                PerformQueryWholeTable((e.OriginalSource as TextBlock)?.Text);
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        #endregion
    }
}