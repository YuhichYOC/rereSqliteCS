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
using System.Windows.Media;

public partial class OFWindow {
    private AppBehind appBehind;

    private bool createNewFile;

    private FileSystemTreeEx driveTreeOperator;

    public OFWindow() {
        InitializeComponent();
        Prepare();
    }

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public string SelectedPath { get; private set; }

    private void Prepare() {
        createNewFile = false;
        FillDriveLetters();
        PrepareDriveTree(PrepareFileList());
        OpenButton.Click += OpenButton_Click;
        DataContext = this;
    }

    private void FillDriveLetters() {
        Drives.ItemsSource = Directory.GetLogicalDrives().ToList();
        Drives.SelectionChanged += Drives_Change;
    }

    private OperatorEx PrepareFileList() {
        var fileListOperator = new OperatorEx();
        fileListOperator.Prepare(FileList);
        fileListOperator.AddColumn(@"FileName", @"ファイル名");
        fileListOperator.CreateColumns();
        FileList.SelectedCellsChanged += FileList_Select;
        return fileListOperator;
    }

    private void PrepareDriveTree(OperatorEx gridOperator) {
        driveTreeOperator = new FileSystemTreeEx {AppBehind = appBehind};
        driveTreeOperator.Prepare(DriveTree);
        driveTreeOperator.GridOperator = gridOperator;
        driveTreeOperator.FileFullPathInput = FileFullPathInput;
        driveTreeOperator.CreateNewFile = createNewFile;
    }

    public void CreateNewFile() {
        createNewFile = true;
        driveTreeOperator.CreateNewFile = true;
    }

    private void SwitchDrive() {
        driveTreeOperator.Fill(Drives.SelectedItem.ToString());
    }

    private void Drives_Change(object sender, SelectionChangedEventArgs e) {
        try {
            SwitchDrive();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void FileList_Select(object sender, SelectedCellsChangedEventArgs e) {
        try {
            if (!((sender as DataGrid)?.SelectedItem is RowEntity row)) return;
            if (row.TryGetMember(@"FileName", out var selectedFile))
                FileFullPathInput.Text = selectedFile as string ?? @"";
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e) {
        try {
            if (createNewFile && File.Exists(FileFullPathInput.Text) && MessageBoxResult.No ==
                MessageBox.Show(this, @"既に存在するファイルです。上書きしますか？", @"上書き確認", MessageBoxButton.YesNo)) return;
            SelectedPath = FileFullPathInput.Text;
            Close();
        }
        catch (Exception ex) {
            appBehind.AppendError(ex.Message, ex);
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
        private AppBehind appBehind;

        public AppBehind AppBehind {
            set => appBehind = value;
        }

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
                appBehind.AppendError(ex.Message, ex);
            }
        }
    }
}