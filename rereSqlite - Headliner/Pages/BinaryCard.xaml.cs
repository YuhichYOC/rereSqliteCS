using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

public partial class BinaryCard : UserControl {
    private AppBehind appBehind;

    private string fileFullPath;

    private bool hasBeenAnyOperation;

    private bool hasBinaryInDB;

    public AppBehind AppBehind {
        get => appBehind;
        set {
            appBehind = value;
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
        var of = new OFWindow();
        of.ShowDialog();
        if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
        fileFullPath = of.SelectedPath;
        FileName = Path.GetFileName(fileFullPath);
        fileNameOutput.Content = FileName;
        HasBeenAnyOperation = true;
    }

    private void PerformRetrieveFile() {
        var of = new OFWindow();
        of.CreateNewFile();
        of.ShowDialog();
        if (null == of.SelectedPath || @"".Equals(of.SelectedPath)) return;
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
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
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
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
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
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
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void RetrieveFile_Click(object sender, RoutedEventArgs e) {
        try {
            PerformRetrieveFile();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void Insert_Click(object sender, RoutedEventArgs e) {
        try {
            PerformInsertFile();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void Update_Click(object sender, RoutedEventArgs e) {
        try {
            PerformUpdateFile();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}