using System;
using System.Windows;
using System.Windows.Controls;

public partial class StringCard : UserControl {
    private AppBehind appBehind;

    private string originalValue;

    public AppBehind AppBehind {
        get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public string Key { get; set; }

    public string Value { get; set; }

    public string OriginalValue {
        get => originalValue;
        set {
            originalValue = value;
            insertButton.IsEnabled = @"".Equals(value);
        }
    }

    public StringCard() {
        InitializeComponent();
    }

    private void Prepare() {
        DataContext = this;
        originalValue = @"";
        insertButton.IsEnabled = false;
        updateButton.IsEnabled = false;
    }

    private void PerformInsert() {
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString = @" INSERT INTO STRING_STORAGE ( KEY, VALUE ) VALUES ( @key, @value ) "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        command.Parameters.AddWithValue(@"@value", Value);
        accessor.Execute(command);
        accessor.Close();
        insertButton.IsEnabled = false;
    }

    private void PerformUpdate() {
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString = @" UPDATE STRING_STORAGE SET VALUE = @value WHERE KEY = @key "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", Key);
        command.Parameters.AddWithValue(@"@value", Value);
        accessor.Execute(command);
        accessor.Close();
        OriginalValue = Value;
        updateButton.IsEnabled = false;
    }

    private void ValueInput_Change(object sender, RoutedEventArgs e) {
        Value = valueInput.Text;
        updateButton.IsEnabled = !(@"".Equals(OriginalValue) || OriginalValue.Equals(Value));
    }

    private void Insert_Click(object sender, RoutedEventArgs e) {
        try {
            PerformInsert();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void Update_Click(object sender, RoutedEventArgs e) {
        try {
            PerformUpdate();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}