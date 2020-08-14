using System.Windows.Controls;
using System.Windows.Data;

public class ColumnDefinition {
    private string title;

    public string BindName { get; set; }

    public string Title {
        set => title = value;
    }

    public void AddColumn(DataGrid grid) {
        var add = new DataGridTextColumn {Header = title, Binding = new Binding(BindName)};
        grid.Columns.Add(add);
    }
}