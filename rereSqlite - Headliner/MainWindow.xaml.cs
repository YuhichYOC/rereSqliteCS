/*
*
* MainWindow.cs
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
using rereSqlite___Headliner.Pages;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Prepare();
    }

    private static LogSpooler Logger { get; } = new LogSpooler();

    private static AppBehind AppBehind { get; } = new AppBehind();

    private void Prepare() {
        Height = AppBehind.WindowHeight;
        Width = AppBehind.WindowWidth;
        FontFamily = new FontFamily(AppBehind.FontFamily);
        FontSize = AppBehind.FontSize;
        Pages.AppBehind = AppBehind;
        Pages.AddPage(@"ファイルを開く", new FileBrowse {AppBehind = AppBehind});
        Pages.AddPage(@"テーブル一覧", new TableList {AppBehind = AppBehind});
        Pages.AddPage(@"クエリ実行", new QueryStringInput {AppBehind = AppBehind});
        Pages.AddPage(@"問い合わせ結果", new QueryResultViewList {AppBehind = AppBehind});
        Pages.AddPage(@"文字列データ", new StringStorage {AppBehind = AppBehind});
        Pages.AddPage(@"バイナリデータ", new BinaryStorage {AppBehind = AppBehind});
        Pages.AddPage(@"タグ", new TagMaster {AppBehind = AppBehind});
        Pages.AddPage(@"クローン", new Clone {AppBehind = AppBehind});
        Pages.AddPage(@"Informations", new RunningInformations {AppBehind = AppBehind});
        AppBehind.AppendError += Logger.AppendError;
        AppBehind.AppendError += ((RunningInformations) Pages.GetPage(@"Informations")).AppendInfo;
        AppBehind.AppendInfo += Logger.AppendInfo;
        AppBehind.AppendInfo += ((RunningInformations) Pages.GetPage(@"Informations")).AppendInfo;
        AppBehind.Reload = ((TableList) Pages.GetPage(@"テーブル一覧")).FillTableList;
        AppBehind.Reload += ((BinaryStorage) Pages.GetPage(@"バイナリデータ")).FillTagInput;
        AppBehind.Reload += ((StringStorage) Pages.GetPage(@"文字列データ")).FillTagInput;
        AppBehind.SetQueryString = ((QueryStringInput) Pages.GetPage(@"クエリ実行")).SetQueryString;
        AppBehind.AddPage = ((QueryResultViewList) Pages.GetPage(@"問い合わせ結果")).AddPage;
        Logger.SafeTicks = -1;
        Logger.Start();
        Pages.SwitchPage(0);
    }

    private void Window_Close(object sender, EventArgs e) {
        Logger.Dispose();
    }
}