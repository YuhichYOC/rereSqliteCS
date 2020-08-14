using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

public partial class BinaryStorage : Page {
    private AppBehind appBehind;

    public AppBehind AppBehind {
        get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public BinaryStorage() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        DataContext = AppBehind;
    }

    public void SetUp() {
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString =
                @" SELECT COUNT(NAME) AS COUNT_TABLES FROM sqlite_master WHERE TYPE = 'table' AND NAME = 'BINARY_STORAGE' "
        };
        accessor.Open();
        if (0 < (long) accessor.ExecuteScalar(accessor.CreateCommand())) {
            accessor.Close();
            return;
        }

        accessor.QueryString =
            @" CREATE TABLE BINARY_STORAGE ( KEY TEXT, FILE_NAME TEXT, VALUE BLOB, PRIMARY KEY (KEY) ) ";
        accessor.Execute(accessor.CreateCommand());
        accessor.Close();
    }

    private void PerformSelect() {
        var accessor = new SqliteAccessor {
            DataSource = AppBehind.DBFilePath, Password = AppBehind.Password,
            QueryString = @" SELECT KEY, FILE_NAME FROM BINARY_STORAGE WHERE KEY LIKE @key || '%' "
        };
        accessor.Open();
        var command = accessor.CreateCommand();
        command.Parameters.AddWithValue(@"@key", keyInput.Text);
        accessor.Execute(command);
        accessor.Close();
        FillCardList(accessor.QueryResult);
    }
    
    private void FillCardList(List<List<object>> rows) {
        cardList.Children.Clear();
        var keyHit = false;
        rows.ForEach(row => {
            cardList.Children.Add(new BinaryCard {
                AppBehind = AppBehind, Key = row[0].ToString(), FileName = row[1].ToString(), HasBinaryInDb = true,
                HasBeenAnyOperation = false, Margin = new Thickness(0, 2, 0, 0)
            });
            if (keyInput.Text.Equals(row[0].ToString())) keyHit = true;
        });
        if (keyHit) return;
        cardList.Children.Add(new BinaryCard {
            AppBehind = AppBehind, Key = keyInput.Text, FileName = @"", HasBinaryInDb = false,
            HasBeenAnyOperation = false, Margin = new Thickness(0, 2, 0, 0)
        });
    }

    private void KeyInput_Change(object sender, RoutedEventArgs e) {
        try {
            PerformSelect();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}