/*
*
* Operator.cs
*
* Copyright 2017 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

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