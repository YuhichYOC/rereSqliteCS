using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

public class Operator {
    private DataGrid ownGrid;
    private List<ColumnDefinition> columns;
    private ObservableCollection<RowEntity> rows;

    public void Prepare(DataGrid grid) {
        ownGrid = grid;
        rows = new ObservableCollection<RowEntity>();
    }

    public void AddColumn(string bindName, string title) {
        columns ??= new List<ColumnDefinition>();
        columns.Add(new ColumnDefinition {BindName = bindName, Title = title});
    }

    public void CreateColumns() {
        ownGrid.CanUserAddRows = false;
        ownGrid.Columns.Clear();
        columns.ForEach(c => c.AddColumn(ownGrid));
    }

    public void Refresh() {
        ownGrid.ItemsSource = rows;
    }

    public void Blank() {
        rows = new ObservableCollection<RowEntity>();
        ownGrid.ItemsSource = rows;
    }

    public void AddRow(RowEntity row) {
        rows.Add(row);
    }

    protected ColumnDefinition Column(int i) {
        return columns[i];
    }
}