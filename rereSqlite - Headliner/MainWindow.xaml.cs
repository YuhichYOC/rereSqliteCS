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

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private static LogSpooler Logger { get; } = new LogSpooler();

    private static AppBehind AppBehind { get; } = new AppBehind();

    public MainWindow() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        Height = AppBehind.WindowHeight;
        Width = AppBehind.WindowWidth;
        FontFamily = new FontFamily(AppBehind.FontFamily);
        FontSize = AppBehind.FontSize;
        pages.AppBehind = AppBehind;
        pages.EnableRemovePage = false;
        pages.AddPage(@"ファイルを開く", new FileBrowse {AppBehind = AppBehind});
        pages.AddPage(@"テーブル一覧", new TableList {AppBehind = AppBehind});
        pages.AddPage(@"クエリ実行", new QueryStringInput {AppBehind = AppBehind});
        pages.AddPage(@"問い合わせ結果", new QueryResultViewList {AppBehind = AppBehind});
        pages.AddPage(@"文字列データ", new StringStorage {AppBehind = AppBehind});
        pages.AddPage(@"バイナリデータ", new BinaryStorage {AppBehind = AppBehind});
        pages.AddPage(@"Informations", new RunningInformations {AppBehind = AppBehind});
        AppBehind.AppendError += Logger.AppendError;
        AppBehind.AppendError += ((RunningInformations) pages.GetPage(@"Informations")).AppendInfo;
        AppBehind.Reload = ((TableList) pages.GetPage(@"テーブル一覧")).FillTableList;
        AppBehind.SetQueryString = ((QueryStringInput) pages.GetPage(@"クエリ実行")).SetQueryString;
        AppBehind.AddPage = ((QueryResultViewList) pages.GetPage(@"問い合わせ結果")).AddPage;
        AppBehind.StringStorageSetUp = ((StringStorage) pages.GetPage(@"文字列データ")).SetUp;
        AppBehind.BinaryStorageSetUp = ((BinaryStorage) pages.GetPage(@"バイナリデータ")).SetUp;
        Logger.SafeTicks = -1;
        Logger.Start();
        pages.SwitchPage(0);
    }

    private void Window_Close(object sender, EventArgs e) {
        Logger.Dispose();
    }
}