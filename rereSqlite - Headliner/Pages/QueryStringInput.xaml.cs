using System;
using System.Windows;
using System.Windows.Controls;

public partial class QueryStringInput : Page {
    private AppBehind appBehind;

    private QueryChunk qc;

    public AppBehind AppBehind {
        get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public QueryStringInput() {
        InitializeComponent();
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
            qc = new QueryChunk(AppBehind);
            qc.Open();
        }

        qc.AddCommand(queryInput.Text);
        qc.Execute();
        if (openNew) qc.Close();
    }

    private void PerformBegin() {
        qc?.Close();
        qc = new QueryChunk(AppBehind);
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
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void Begin_Click(object sender, RoutedEventArgs e) {
        try {
            PerformBegin();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void Commit_Click(object sender, RoutedEventArgs e) {
        try {
            PerformCommit();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }

    private void Rollback_Click(object sender, RoutedEventArgs e) {
        try {
            PerformRollback();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}