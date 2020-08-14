using System.Windows.Controls;

public partial class QueryResultView : Page {
    private AppBehind appBehind;

    private Operator dataGridOperator;

    public AppBehind AppBehind {
        get => appBehind;
        set {
            appBehind = value;
            FontSize = appBehind.FontSize;
        }
    }

    public QueryResultView() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        dataGridOperator = new Operator();
        dataGridOperator.Prepare(ownGrid);
    }

    public void Show(SqliteAccessor accessor) {
        if (0 == accessor.QueryResultAttributes.Count || 0 == accessor.QueryResult.Count) return;
        accessor.QueryResultAttributes.ForEach(a => dataGridOperator.AddColumn(a.Item1, a.Item1));
        dataGridOperator.CreateColumns();
        accessor.QueryResult.ForEach(row => {
            var addRow = new RowEntity();
            for (var i = 0; accessor.QueryResultAttributes.Count > i; ++i)
                addRow.TrySetMember(accessor.QueryResultAttributes[i].Item1,
                    accessor.IsBlobColumn(accessor.QueryResultAttributes, i) ? @"[Blob data]" : row[i]);
            dataGridOperator.AddRow(addRow);
        });
        dataGridOperator.Refresh();
    }
}