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
using System.Windows.Media;
using rereSqlite___Headliner.Pages;

namespace rereSqlite___Headliner {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
            Prepare();
        }

        private static LogSpooler Logger { get; } = new LogSpooler();

        private void Prepare() {
            Height = AppBehind.Get.WindowHeight;
            Width = AppBehind.Get.WindowWidth;
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            Pager.AppendErrorDelegate += AppBehind.Get.AppendError;
            Pager.FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            Pager.FontSize = AppBehind.Get.FontSize;
            Pager.EnableRemovePage = false;
            Pager.Init();
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabFileBrowse, new FileBrowse());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabTableList, new TableList());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabQueryStringInput, new QueryStringInput());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabQueryResultViewList, new QueryResultViewList());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabStringStorage, new StringStorage());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabBinaryStorage, new BinaryStorage());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabTagMaster, new TagMaster());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabClone, new Clone());
            Pager.AddPage(AppBehind.Get.MainWindowCaptions.TabRunningInformation, new RunningInformations());
            AppBehind.Get.AppendError
                += Logger.AppendError;
            AppBehind.Get.AppendError
                += ((RunningInformations) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabRunningInformation))
                .AppendInfo;
            AppBehind.Get.AppendInfo
                += Logger.AppendInfo;
            AppBehind.Get.AppendInfo
                += ((RunningInformations) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabRunningInformation))
                .AppendInfo;
            AppBehind.Get.Reload
                += ((TableList) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabTableList)).FillTableList;
            AppBehind.Get.Reload
                += AppBehind.Get.ReloadTags;
            AppBehind.Get.SetQueryString
                += ((QueryStringInput) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabQueryStringInput))
                .SetQueryString;
            AppBehind.Get.AddPage
                += ((QueryResultViewList) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabQueryResultViewList))
                .AddPage;
            ((FileBrowse) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabFileBrowse)).Init();
            ((TableList) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabTableList)).Init();
            ((QueryStringInput) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabQueryStringInput)).Init();
            ((QueryResultViewList) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabQueryResultViewList)).Init();
            ((StringStorage) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabStringStorage)).Init();
            ((BinaryStorage) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabBinaryStorage)).Init();
            ((TagMaster) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabTagMaster)).Init();
            ((Clone) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabClone)).Init();
            ((RunningInformations) Pager.GetPage(AppBehind.Get.MainWindowCaptions.TabRunningInformation)).Init();
            Logger.SafeTicks = -1;
            Logger.Start();
            Pager.SwitchPage(0);
        }

        private void Window_Close(object sender, EventArgs e) {
            Logger.Dispose();
        }
    }
}