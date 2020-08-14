/*
*
* FileBrowse.cs
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

public partial class FileBrowse : Page {
    private AppBehind appBehind;

    public AppBehind AppBehind {
        private get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public FileBrowse() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        DataContext = AppBehind;
    }

    private void Browse_Click(object sender, RoutedEventArgs e) {
        try {
            var of = new OFWindow();
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            AppBehind.DBFilePath = of.SelectedPath;
            filePathOutput.Content = AppBehind.DBFilePath;
            AppBehind.Password = passwordInput.Text;
            AppBehind.Reload();
            AppBehind.StringStorageSetUp();
            AppBehind.BinaryStorageSetUp();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void NewFile_Click(object sender, RoutedEventArgs e) {
        try {
            var of = new OFWindow();
            of.CreateNewFile();
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            AppBehind.DBFilePath = of.SelectedPath;
            filePathOutput.Content = AppBehind.DBFilePath;
            AppBehind.Password = passwordInput.Text;
            var accessor = @"".Equals(AppBehind.Password)
                ? new SqliteAccessor {DataSource = AppBehind.DBFilePath}
                : new SqliteAccessor {DataSource = AppBehind.DBFilePath, Password = AppBehind.Password};
            accessor.Open();
            accessor.Close();
            AppBehind.Reload();
            AppBehind.StringStorageSetUp();
            AppBehind.BinaryStorageSetUp();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}