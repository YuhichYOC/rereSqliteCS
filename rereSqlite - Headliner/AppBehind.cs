/*
*
* AppBehind.cs
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

public class AppBehind {
    public delegate void AppendErrorDelegate(string message, Exception ex);

    public delegate void AppendInfoDelegate(string message);

    public delegate void ReloadDelegate();

    public delegate void SetQueryStringDelegate(string query);

    public delegate void AddPageDelegate(SqliteAccessor accessor);

    public delegate void StringStorageSetUpDelegate();

    public delegate void BinaryStorageSetUpDelegate();

    public int WindowHeight { get; }

    public int WindowWidth { get; }

    public string FontFamily { get; }

    public double FontSize { get; }

    public double DataGridRowHeightPlus { get; }

    public string DBFilePath { get; set; }

    public string Password { get; set; }

    public AppendErrorDelegate AppendError { get; set; }

    public AppendInfoDelegate AppendInfo { get; set; }

    public ReloadDelegate Reload { get; set; }

    public SetQueryStringDelegate SetQueryString { get; set; }

    public AddPageDelegate AddPage { get; set; }

    public StringStorageSetUpDelegate StringStorageSetUp { get; set; }

    public BinaryStorageSetUpDelegate BinaryStorageSetUp { get; set; }

    public AppBehind() {
        var r = new XReader {Directory = @".", FileName = @"Setting.config"};
        r.Parse();
        WindowHeight = int.Parse(r.Node.Find(@"SettingDef").Find(@"Window").Find(@"Height").NodeValue);
        WindowWidth = int.Parse(r.Node.Find(@"SettingDef").Find(@"Window").Find(@"Width").NodeValue);
        FontFamily = r.Node.Find(@"SettingDef").Find(@"Window").Find(@"FontFamily").NodeValue;
        FontSize = double.Parse(r.Node.Find(@"SettingDef").Find(@"Window").Find(@"FontSize").NodeValue);
        DataGridRowHeightPlus =
            double.Parse(r.Node.Find(@"SettingDef").Find(@"DataGrid").Find(@"RowHeightPlus").NodeValue);
    }
}