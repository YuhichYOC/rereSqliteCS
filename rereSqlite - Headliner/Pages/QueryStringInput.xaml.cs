/*
*
* QueryStringInput.cs
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

public partial class QueryStringInput : Page {
    private AppBehind appBehind;

    private QueryChunk qc;

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public QueryStringInput() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        executeButton.IsEnabled = true;
        beginButton.IsEnabled = true;
        commitButton.IsEnabled = false;
        rollbackButton.IsEnabled = false;
    }

    public void SetQueryString(string arg) {
        queryInput.Text = arg;
    }

    private void PerformExecute() {
        var openNew = !(null != qc && qc.TransactionAlreadyBegun);
        if (openNew) {
            qc = new QueryChunk(appBehind);
            qc.Open();
        }

        qc.AddCommand(queryInput.Text);
        qc.Execute();
        if (openNew) qc.Close();
    }

    private void PerformBegin() {
        qc?.Close();
        qc = new QueryChunk(appBehind);
        qc.Begin();
        beginButton.IsEnabled = false;
        commitButton.IsEnabled = true;
        rollbackButton.IsEnabled = true;
    }

    private void PerformCommit() {
        qc.Commit();
        qc.Close();
        beginButton.IsEnabled = true;
        commitButton.IsEnabled = false;
        rollbackButton.IsEnabled = false;
    }

    private void PerformRollback() {
        qc.Rollback();
        qc.Close();
        beginButton.IsEnabled = true;
        commitButton.IsEnabled = false;
        rollbackButton.IsEnabled = false;
    }

    private void Execute_Click(object sender, RoutedEventArgs e) {
        try {
            PerformExecute();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Begin_Click(object sender, RoutedEventArgs e) {
        try {
            PerformBegin();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Commit_Click(object sender, RoutedEventArgs e) {
        try {
            PerformCommit();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Rollback_Click(object sender, RoutedEventArgs e) {
        try {
            PerformRollback();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }
}