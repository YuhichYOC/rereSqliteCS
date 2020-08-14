/*
*
* BinaryCard.cs
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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public partial class BinaryCard : UserControl {
    private AppBehind appBehind;

    private string fileFullPath;

    private bool hasBeenAnyOperation;

    private bool hasBinaryInDB;

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public string Key { get; set; }

    public string FileName { get; set; }

    public bool HasBeenAnyOperation {
        get => hasBeenAnyOperation;
        set {
            hasBeenAnyOperation = value;
            insertButton.IsEnabled = hasBeenAnyOperation & !hasBinaryInDB;
            updateButton.IsEnabled = hasBeenAnyOperation & hasBinaryInDB;
        }
    }

    public bool HasBinaryInDb {
        get => hasBinaryInDB;
        set {
            hasBinaryInDB = value;
            retrieveFileButton.IsEnabled = hasBinaryInDB;
        }
    }

    public BinaryCard() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        DataContext = this;
        fileFullPath = @"";
        selectFileButton.IsEnabled = true;
        retrieveFileButton.IsEnabled = false;
        insertButton.IsEnabled = false;
        updateButton.IsEnabled = false;
    }

    private void PerformSelectFile() {
        var of = new OFWindow {AppBehind = appBehind};
        of.ShowDialog();
        if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
        fileFullPath = of.SelectedPath;
        FileName = Path.GetFileName(fileFullPath);
        fileNameOutput.Content = FileName;
        HasBeenAnyOperation = true;
    }

    private void PerformRetrieveFile() {
        var of = new OFWindow {AppBehind = appBehind};
        of.CreateNewFile();
        of.ShowDialog();
        if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString = @" SELECT VALUE FROM BINARY_STORAGE WHERE KEY = @key "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        using var outputStream = new FileStream(of.SelectedPath, FileMode.Create, FileAccess.Write);
        accessor.RetrieveBlob(command, outputStream, 0);
        accessor.Close();
    }

    private void PerformInsertFile() {
        var totalLength = new FileInfo(fileFullPath).Length;
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString =
                @" INSERT INTO BINARY_STORAGE ( KEY, FILE_NAME, VALUE ) VALUES ( @key, @fileName, zeroblob(@length) ); SELECT rowid FROM BINARY_STORAGE WHERE KEY = @key; "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        command.Parameters.AddWithValue(@"@fileName", FileName);
        command.Parameters.AddWithValue(@"@length", totalLength);
        using var inputStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
        accessor.WriteBlob(inputStream, @"BINARY_STORAGE", @"VALUE", (long) accessor.ExecuteScalar(command));
        accessor.Close();
        HasBinaryInDb = true;
        HasBeenAnyOperation = false;
    }

    private void PerformUpdateFile() {
        var totalLength = new FileInfo(fileFullPath).Length;
        var accessor = new SqliteAccessor {
            DataSource = appBehind.DBFilePath, Password = appBehind.Password,
            QueryString =
                @" DELETE FROM BINARY_STORAGE WHERE KEY = @key; INSERT INTO BINARY_STORAGE ( KEY, FILE_NAME, VALUE ) VALUES ( @key, @fileName, zeroblob(@length) ); SELECT rowid FROM BINARY_STORAGE WHERE KEY = @key; "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        command.Parameters.AddWithValue(@"@fileName", FileName);
        command.Parameters.AddWithValue(@"@length", totalLength);
        using var inputStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
        accessor.WriteBlob(inputStream, @"BINARY_STORAGE", @"VALUE", (long) accessor.ExecuteScalar(command));
        accessor.Close();
        HasBeenAnyOperation = false;
    }

    private void SelectFile_Click(object sender, RoutedEventArgs e) {
        try {
            PerformSelectFile();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void RetrieveFile_Click(object sender, RoutedEventArgs e) {
        try {
            PerformRetrieveFile();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Insert_Click(object sender, RoutedEventArgs e) {
        try {
            PerformInsertFile();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void Update_Click(object sender, RoutedEventArgs e) {
        try {
            PerformUpdateFile();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }
}