using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

public partial class OFWindow : Window {
    private OperatorEx fileListOperator;

    private FileSystemTreeEx driveTreeOperator;

    private bool createNewFile;

    public AppBehind AppBehind { get; set; }

    public string SelectedPath { get; private set; }

    public OFWindow() {
        InitializeComponent();
    }

    private void Prepare() {
        createNewFile = false;
        FillDriveLetters();
        PrepareDriveTree();
        PrepareFileList();
        openButton.Click += OpenButton_Click;
    }

    private void FillDriveLetters() {
        drives.ItemsSource = Directory.GetLogicalDrives().ToList();
        drives.SelectionChanged += Drives_Change;
    }

    private void PrepareDriveTree() {
        driveTreeOperator = new FileSystemTreeEx {AppBehind = AppBehind};
        driveTreeOperator.Prepare(driveTree);
        driveTreeOperator.GridOperator = fileListOperator;
        driveTreeOperator.FileFullPathInput = fileFullPathInput;
        driveTreeOperator.CreateNewFile = createNewFile;
        driveTreeOperator.Fill(@"C:\");
    }

    private void PrepareFileList() {
        fileListOperator = new OperatorEx {AppBehind = AppBehind};
        fileListOperator.Prepare(fileList);
        fileListOperator.AddColumn(@"FileName", @"ファイル名");
        fileListOperator.CreateColumns();
        fileList.SelectedCellsChanged += FileList_Select;
    }

    public void CreateNewFile() {
        createNewFile = true;
        driveTreeOperator.CreateNewFile = true;
    }

    private void SwitchDrive() {
        driveTreeOperator.Fill(drives.SelectedItem.ToString());
    }

    private void Drives_Change(object sender, SelectionChangedEventArgs e) {
        try {
            SwitchDrive();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void FileList_Select(object sender, SelectedCellsChangedEventArgs e) {
        try {
            if (!((sender as DataGrid)?.SelectedItem is RowEntity row)) return;
            if (row.TryGetMember(@"FileName", out var selectedFile))
                fileFullPathInput.Text = selectedFile as string ?? @"";
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e) {
        try {
            if (createNewFile && File.Exists(fileFullPathInput.Text) && MessageBoxResult.No ==
                MessageBox.Show(this, @"既に存在するファイルです。上書きしますか？", @"上書き確認", MessageBoxButton.YesNo)) return;
            SelectedPath = fileFullPathInput.Text;
            Close();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private class OperatorEx : Operator {
        public AppBehind AppBehind { get; set; }

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
        public AppBehind AppBehind { get; set; }

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
                AppBehind.AppendError(ex.Message, ex);
            }
        }
    }
}