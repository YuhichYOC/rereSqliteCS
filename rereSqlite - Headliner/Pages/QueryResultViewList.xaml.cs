using System.Windows.Controls;

public partial class QueryResultViewList : Page {
    private AppBehind appBehind;

    public AppBehind AppBehind {
        get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public QueryResultViewList() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        pager.EnableRemovePage = true;
    }

    public void AddPage(SqliteAccessor accessor) {
        var addPage = new QueryResultView {AppBehind = AppBehind};
        addPage.Show(accessor);
        pager.AddPage(@"Query " + (pager.PagesCount + 1), addPage);
    }
}