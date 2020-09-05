/*
*
* Clone.cs
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

public partial class Clone : Page {
    private AppBehind appBehind;

    private string dataSourceTo;

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public Clone() {
        InitializeComponent();
    }

    private void PerformClone() {
        if (null == dataSourceTo || @"".Equals(dataSourceTo)) return;
        var cloner = new SqliteCloner {
            DataSourceFrom = appBehind.DBFilePath, PasswordFrom = appBehind.Password, DataSourceTo = dataSourceTo,
            PasswordTo = passwordInput.Text
        };
        cloner.run();
    }

    private void SelectFile_Click(object sender, RoutedEventArgs e) {
        try {
            var of = new OFWindow {AppBehind = appBehind};
            of.CreateNewFile();
            of.ShowDialog();
            if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
            filePathOutput.Content = of.SelectedPath;
            dataSourceTo = of.SelectedPath;
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Run_Click(object sender, RoutedEventArgs e) {
        try {
            PerformClone();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }
}