/*
*
* StringStorage.cs
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

public partial class StringStorage : Page {
    private AppBehind appBehind;

    public AppBehind AppBehind {
        private get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public StringStorage() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        DataContext = AppBehind;
    }

    public void SetUp() {
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString =
                @" SELECT COUNT(NAME) AS COUNT_TABLES FROM sqlite_master WHERE TYPE = 'table' AND NAME = 'STRING_STORAGE' "
        };
        accessor.Open();
        if (0 < (long) accessor.ExecuteScalar(accessor.CreateCommand())) {
            accessor.Close();
            return;
        }

        accessor.QueryString = @" CREATE TABLE STRING_STORAGE ( KEY TEXT, VALUE TEXT, PRIMARY KEY (KEY) ) ";
        accessor.Execute(accessor.CreateCommand());
        accessor.Close();
    }

    private void PerformSelect() {
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString = @" SELECT KEY, VALUE FROM STRING_STORAGE WHERE KEY LIKE @key || '%' "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", keyInput.Text);
        accessor.Execute(command);
        accessor.Close();
        FillCardList(accessor.QueryResult);
    }

    private void FillCardList(List<List<object>> rows) {
        cardList.Children.Clear();
        var keyHit = false;
        rows.ForEach(row => {
            cardList.Children.Add(new StringCard {
                AppBehind = AppBehind, Key = row[0].ToString(), Value = row[1].ToString(),
                OriginalValue = row[1].ToString(), Margin = new Thickness(0, 2, 0, 2)
            });
            if (keyInput.Text.Equals(row[0].ToString())) keyHit = true;
        });
        if (keyHit) return;
        cardList.Children.Add(new StringCard {
            AppBehind = AppBehind, Key = keyInput.Text, Value = @"", OriginalValue = @"",
            Margin = new Thickness(0, 2, 0, 2)
        });
    }

    private void KeyInput_Change(object sender, RoutedEventArgs e) {
        try {
            PerformSelect();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}