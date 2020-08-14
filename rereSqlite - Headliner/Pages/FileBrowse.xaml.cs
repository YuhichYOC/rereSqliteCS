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