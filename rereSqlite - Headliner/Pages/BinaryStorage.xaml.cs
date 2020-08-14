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
using System.Windows.Controls;
using System.Windows.Media;

public partial class BinaryStorage : Page {
    private AppBehind appBehind;

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public BinaryStorage() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        DataContext = appBehind;
    }

    public void SetUp() {
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString =
                @" SELECT COUNT(NAME) AS COUNT_TABLES FROM sqlite_master WHERE TYPE = 'table' AND NAME = 'BINARY_STORAGE' "
        };
        accessor.Open();
        if (0 < (long) accessor.ExecuteScalar(accessor.CreateCommand())) {
            accessor.Close();
            return;
        }

        accessor.QueryString =
            @" CREATE TABLE BINARY_STORAGE ( KEY TEXT, FILE_NAME TEXT, VALUE BLOB, PRIMARY KEY (KEY) ) ";
        accessor.Execute(accessor.CreateCommand());
        accessor.Close();
    }

    private void PerformSelect() {
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString = @" SELECT KEY, FILE_NAME FROM BINARY_STORAGE WHERE KEY LIKE @key || '%' "
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
            cardList.Children.Add(new BinaryCard {
                AppBehind = appBehind, Key = row[0].ToString(), FileName = row[1].ToString(), HasBinaryInDb = true,
                HasBeenAnyOperation = false, Margin = new Thickness(0, 2, 0, 0)
            });
            if (keyInput.Text.Equals(row[0].ToString())) keyHit = true;
        });
        if (keyHit) return;
        cardList.Children.Add(new BinaryCard {
            AppBehind = appBehind, Key = keyInput.Text, FileName = @"", HasBinaryInDb = false,
            HasBeenAnyOperation = false, Margin = new Thickness(0, 2, 0, 0)
        });
    }

    private void KeyInput_Change(object sender, RoutedEventArgs e) {
        try {
            PerformSelect();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }
}