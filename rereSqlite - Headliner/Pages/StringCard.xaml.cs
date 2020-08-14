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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public partial class StringCard : UserControl {
    private AppBehind appBehind;

    private string originalValue;

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public string Key { get; set; }

    public string Value { get; set; }

    public string OriginalValue {
        get => originalValue;
        set {
            originalValue = value;
            insertButton.IsEnabled = @"".Equals(value);
        }
    }

    public StringCard() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        DataContext = this;
        originalValue = @"";
        insertButton.IsEnabled = false;
        updateButton.IsEnabled = false;
    }

    private void PerformInsert() {
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString = @" INSERT INTO STRING_STORAGE ( KEY, VALUE ) VALUES ( @key, @value ) "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        command.Parameters.AddWithValue(@"@value", Value);
        accessor.Execute(command);
        accessor.Close();
        insertButton.IsEnabled = false;
    }

    private void PerformUpdate() {
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString = @" UPDATE STRING_STORAGE SET VALUE = @value WHERE KEY = @key "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        command.Parameters.AddWithValue(@"@value", Value);
        accessor.Execute(command);
        accessor.Close();
        OriginalValue = Value;
        updateButton.IsEnabled = false;
    }

    private void ValueInput_Change(object sender, RoutedEventArgs e) {
        Value = valueInput.Text;
        updateButton.IsEnabled = !(@"".Equals(OriginalValue) || OriginalValue.Equals(Value));
    }

    private void Insert_Click(object sender, RoutedEventArgs e) {
        try {
            PerformInsert();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Update_Click(object sender, RoutedEventArgs e) {
        try {
            PerformUpdate();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }
}