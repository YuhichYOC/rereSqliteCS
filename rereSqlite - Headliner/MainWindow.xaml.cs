using System;
using System.Windows;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private static LogSpooler Logger { get; } = new LogSpooler();

    private static AppBehind AppBehind { get; } = new AppBehind();

    public MainWindow() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        pages.EnableRemovePage = false;
        pages.AddPage(@"ファイルを開く", new FileBrowse {AppBehind = AppBehind});
        pages.AddPage(@"テーブル一覧", new TableList {AppBehind = AppBehind});
        pages.AddPage(@"クエリ実行", new QueryStringInput {AppBehind = AppBehind});
        pages.AddPage(@"問い合わせ結果", new QueryResultViewList {AppBehind = AppBehind});
        pages.AddPage(@"文字列データ", new StringStorage {AppBehind = AppBehind});
        pages.AddPage(@"バイナリデータ", new BinaryStorage {AppBehind = AppBehind});
        AppBehind.Reload = ((TableList) pages.GetPage(@"テーブル一覧")).FillTableList;
        AppBehind.SetQueryString = ((QueryStringInput) pages.GetPage(@"クエリ実行")).SetQueryString;
        AppBehind.AddPage = ((QueryResultViewList) pages.GetPage(@"問い合わせ結果")).AddPage;
        AppBehind.StringStorageSetUp = ((StringStorage) pages.GetPage(@"文字列データ")).SetUp;
        AppBehind.BinaryStorageSetUp = ((BinaryStorage) pages.GetPage(@"バイナリデータ")).SetUp;
        Logger.SafeTicks = -1;
        Logger.Start();
        pages.SwitchPage(0);
    }

    private void Window_Close(object sender, EventArgs e) {
        Logger.Dispose();
    }
}