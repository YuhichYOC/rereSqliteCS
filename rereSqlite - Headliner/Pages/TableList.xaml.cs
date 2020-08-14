﻿/*
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public partial class TableList : Page {
    private AppBehind appBehind;

    private Operator tableListOperator;

    public AppBehind AppBehind {
        private get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public TableList() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        tableListOperator = new Operator();
        tableListOperator.Prepare(tableList);
        tableListOperator.AddColumn(@"TableName", @"テーブル");
        tableListOperator.CreateColumns();
    }

    public void FillTableList() {
        if (@"".Equals(AppBehind.DBFilePath)) return;
        FillTableList(QueryTables());
    }

    private List<List<object>> QueryTables() {
        var accessor = new SqliteAccessor
            {DataSource = AppBehind.DBFilePath, Password = AppBehind.Password};
        accessor.Open();
        if (@"".Equals(filterInput.Text.Trim())) {
            accessor.QueryString = @" SELECT NAME FROM sqlite_master WHERE TYPE = 'table' ORDER BY NAME ";
            accessor.Execute(accessor.CreateCommand());
        }
        else {
            accessor.QueryString =
                @" SELECT NAME FROM sqlite_master WHERE TYPE = 'table' AND NAME LIKE '%' || @filter || '%' ORDER BY NAME ";
            var com = accessor.CreateCommand();
            com.Parameters.AddWithValue(@"@filter", filterInput.Text);
            accessor.Execute(com);
        }

        accessor.Close();
        return accessor.QueryResult;
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
        if (@"".Equals(tableName)) return;
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString = @" PRAGMA table_info('" + tableName + "') "
        };
        accessor.Open();
        accessor.Execute(accessor.CreateCommand());
        if (0 == accessor.QueryResult.Count) return;
        var query = @" SELECT " + '\n';
        for (var i = 0; accessor.QueryResult.Count > i; ++i) {
            query += 0 == i ? @"     " : @"   , ";
            query += accessor.QueryResult[i][1] + @" " + '\n';
        }

        query += @" FROM " + '\n';
        query += @"     " + tableName + @" " + '\n';
        accessor.QueryString = query;
        accessor.Execute(accessor.CreateCommand());
        accessor.Close();
        AppBehind.SetQueryString(query);
        AppBehind.AddPage(accessor);
    }

    private void Reload_AnyEvent(object sender, RoutedEventArgs e) {
        try {
            FillTableList();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void TableList_DoubleClick(object sender, MouseButtonEventArgs e) {
        try {
            PerformQueryWholeTable((e.OriginalSource as TextBlock)?.Text);
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}