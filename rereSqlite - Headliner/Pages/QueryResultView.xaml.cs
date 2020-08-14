/*
*
* QueryResultView.cs
*
* Copyright 2020 Yuichi Yoshii
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