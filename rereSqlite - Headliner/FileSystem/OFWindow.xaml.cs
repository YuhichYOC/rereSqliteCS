/*
*
* OFWindow.cs
*
* Copyright 2017 Yuichi Yoshii
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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

public partial class OFWindow {
    private FileSystemTreeEx driveTreeOperator;

    public OFWindow(bool createNewFile) {
        InitializeComponent();
        CreateNewFile = createNewFile;
    }

    public Action<string, Exception> AppendErrorDelegate { get; set; }

    public double RowHeightPlus { get; set; }

    public string OpenButtonCaption { get; set; }

    public string NewFileButtonCaption { get; set; }

    public string FileNameColumnCaption { get; set; }

    public string OverwriteDialogTitle { get; set; }

    public string OverwriteMessage { get; set; }

    public bool CreateNewFile { get; }

    public string SelectedPath { get; private set; }

    public void Init() {
        Drives.ItemsSource = Directory.GetLogicalDrives().ToList();
        var fileListOperator = new OperatorEx();
        fileListOperator.Prepare(FileList);
        fileListOperator.AddColumn(@"FileName", FileNameColumnCaption);
        fileListOperator.CreateColumns();
        FileList.RowHeight = FontSize + RowHeightPlus;
        driveTreeOperator = new FileSystemTreeEx {AppendErrorDelegate = AppendErrorDelegate};
        driveTreeOperator.Prepare(DriveTree);
        driveTreeOperator.GridOperator = fileListOperator;
        driveTreeOperator.FileFullPathInput = FileFullPathInput;
        driveTreeOperator.CreateNewFile = CreateNewFile;
        OpenButton.Content = CreateNewFile ? NewFileButtonCaption : OpenButtonCaption;
    }

    private void SwitchDrive() {
        driveTreeOperator.Fill(Drives.SelectedItem.ToString());
    }

    private void Drives_Change(object sender, SelectionChangedEventArgs e) {
        try {
            SwitchDrive();
        }
        catch (Exception ex) {
            AppendErrorDelegate?.Invoke(ex.Message, ex);
        }
    }

    private void FileList_Select(object sender, SelectedCellsChangedEventArgs e) {
        try {
            if (!((sender as DataGrid)?.SelectedItem is RowEntity row)) return;
            if (row.TryGetMember(@"FileName", out var selectedFile))
                FileFullPathInput.Text = selectedFile as string ?? @"";
        }
        catch (Exception ex) {
            AppendErrorDelegate?.Invoke(ex.Message, ex);
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e) {
        try {
            if (CreateNewFile && File.Exists(FileFullPathInput.Text) && MessageBoxResult.No ==
                MessageBox.Show(this, OverwriteMessage, OverwriteDialogTitle, MessageBoxButton.YesNo)) return;
            SelectedPath = FileFullPathInput.Text;
            Close();
        }
        catch (Exception ex) {
            AppendErrorDelegate?.Invoke(ex.Message, ex);
        }
    }

    private class OperatorEx : Operator {
        public void DisplayDirectory(FileSystemNode node) {
            Blank();
            TryGetFiles(node.FullPath)?.ToList().ForEach(f => {
                var add = new RowEntity();
                add.TrySetMember(Column(0).BindName, f);
                AddRow(add);
            });
        }

        private IEnumerable<string> TryGetFiles(string path) {
            try {
                return Directory.EnumerateFiles(path);
            }
            catch (UnauthorizedAccessException) {
                return null;
            }
        }
    }

    private class FileSystemTreeEx : FileSystemTree {
        public Action<string, Exception> AppendErrorDelegate { get; set; }

        public OperatorEx GridOperator { private get; set; }

        public TextBox FileFullPathInput { private get; set; }

        public bool CreateNewFile { private get; set; }

        public new void Fill(string path) {
            if (0 < OwnTree.Items.Count) OwnTree.Items.Clear();
            base.Fill(path);
            OwnTree.SelectedItemChanged += Item_Select;
        }

        private void Item_Select(object sender, RoutedEventArgs e) {
            try {
                if (!((sender as TreeView)?.SelectedItem is FileSystemNode item)) return;
                GridOperator.DisplayDirectory(item);
                if (CreateNewFile) FileFullPathInput.Text = item.FullPath;
            }
            catch (Exception ex) {
                AppendErrorDelegate?.Invoke(ex.Message, ex);
            }
        }
    }
}