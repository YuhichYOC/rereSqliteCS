using System;

public class AppBehind {
    public delegate void AppendErrorDelegate(string message, Exception ex);

    public delegate void ReloadDelegate();

    public delegate void SetQueryStringDelegate(string query);

    public delegate void AddPageDelegate(SqliteAccessor accessor);

    public delegate void StringStorageSetUpDelegate();

    public delegate void BinaryStorageSetUpDelegate();

    public double FontSize { get; }

    public string DBFilePath { get; set; }

    public string Password { get; set; }

    public AppendErrorDelegate AppendError { get; set; }

    public ReloadDelegate Reload { get; set; }

    public SetQueryStringDelegate SetQueryString { get; set; }

    public AddPageDelegate AddPage { get; set; }

    public StringStorageSetUpDelegate StringStorageSetUp { get; set; }

    public BinaryStorageSetUpDelegate BinaryStorageSetUp { get; set; }

    public AppBehind() {
        FontSize = 14;
    }
}