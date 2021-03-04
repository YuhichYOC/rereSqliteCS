/*
*
* SqliteCloner.cs
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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.Sqlite;

public class SqliteCloner {
    #region -- Query Strings --

    private const string SELECT_SQLITE_MASTER =
        @" SELECT               " +
        @"     NAME,            " +
        @"     SQL              " +
        @" FROM                 " +
        @"     sqlite_master    " +
        @" WHERE                " +
        @"     TYPE   = 'table' " +
        @" ORDER BY             " +
        @"     NAME             ";

    #endregion

    private readonly List<TableInfo> tables;

    private SqliteAccessor accessorFrom;

    private SqliteAccessor accessorTo;

    public SqliteCloner() {
        tables = new List<TableInfo>();
    }

    public string DataSourceFrom { get; set; }

    public string PasswordFrom { get; set; }

    public string DataSourceTo { get; set; }

    public string PasswordTo { get; set; }

    public void AddPriorTable(string name) {
        tables.Add(new TableInfo {TableName = name});
    }

    public void Run() {
        accessorFrom = new SqliteAccessor {DataSource = DataSourceFrom, Password = PasswordFrom};
        accessorFrom.Open();
        accessorTo = new SqliteAccessor {DataSource = DataSourceTo, Password = PasswordTo};
        accessorTo.Open();
        FetchTables();
        tables.ForEach(t => {
            CreateTable(t.Sql);
            if (t.HasBlob)
                TransferBlob(t);
            else
                TransferNoBlob(t);
        });
        accessorFrom.Close();
        accessorTo.Close();
    }

    private void FetchTables() {
        accessorFrom.QueryString = SELECT_SQLITE_MASTER;
        accessorFrom.Execute(accessorFrom.CreateCommand());
        var queryResult = accessorFrom.QueryResult;
        queryResult.ForEach(row => {
            var tableInfo = tables.Find(t => row[0].ToString().Equals(t.TableName));
            if (null == tableInfo) return;
            tableInfo.Sql = row[1].ToString();
            accessorFrom.QueryString = @" PRAGMA table_info ('" + tableInfo.TableName + @"') ";
            accessorFrom.Execute(accessorFrom.CreateCommand());
            for (var i = 0; accessorFrom.QueryResult.Count > i; ++i)
                tableInfo.AddColumn(i, accessorFrom.QueryResult[i][1].ToString(),
                    accessorFrom.QueryResult[i][2].ToString());
        });
        queryResult.ForEach(row => {
            if (tables.Exists(t => row[0].ToString().Equals(t.TableName))) return;
            var add = new TableInfo {TableName = row[0].ToString(), Sql = row[1].ToString()};
            accessorFrom.QueryString = @" PRAGMA table_info ('" + add.TableName + @"') ";
            accessorFrom.Execute(accessorFrom.CreateCommand());
            for (var i = 0; accessorFrom.QueryResult.Count > i; ++i)
                add.AddColumn(i, accessorFrom.QueryResult[i][1].ToString(), accessorFrom.QueryResult[i][2].ToString());
            tables.Add(add);
        });
    }

    private void CreateTable(string ddl) {
        accessorTo.QueryString = ddl;
        accessorTo.Execute(accessorTo.CreateCommand());
    }

    private void TransferBlob(TableInfo tableInfo) {
        accessorFrom.QueryString = tableInfo.SelectQuery();
        accessorFrom.Execute(accessorFrom.CreateCommand());
        var tableFrom = accessorFrom.QueryResult;
        tableFrom.ForEach(row => {
            accessorTo.QueryString = tableInfo.InsertQuery();
            var command = accessorTo.CreateCommand();
            tableInfo.ColumnInfos.ForEach(c => command.Parameters.AddWithValue(c.InsertColumnParam(), row[c.Index]));
            var id = (long) accessorTo.ExecuteScalar(command);
            tableInfo.Blobs.ToList().ForEach(b => {
                accessorFrom.QueryString = @" SELECT " + b.ColumnName + @" FROM " + tableInfo.TableName +
                                           @" WHERE rowid = @idFrom ";
                var commandFrom = accessorFrom.CreateCommand();
                commandFrom.Parameters.AddWithValue(@"@idFrom", row[tableInfo.ColumnInfos.Count]);
                TransferBlob(commandFrom, tableInfo.TableName, b.ColumnName, id);
            });
        });
    }

    private void TransferBlob(SqliteCommand commandFrom, string tableName, string columnName, long rowId) {
        using var reader = commandFrom.ExecuteReader();
        while (reader.Read()) {
            using var outputStream = new SqliteBlob(accessorTo.Connection, tableName, columnName, rowId);
            using var inputStream = reader.GetStream(columnName);
            inputStream.CopyTo(outputStream);
        }
    }

    private void TransferNoBlob(TableInfo tableInfo) {
        accessorFrom.QueryString = tableInfo.SelectQuery();
        accessorFrom.Execute(accessorFrom.CreateCommand());
        accessorTo.QueryString = tableInfo.InsertQuery();
        accessorFrom.QueryResult.ForEach(row => {
            var command = accessorTo.CreateCommand();
            tableInfo.ColumnInfos.ForEach(i => command.Parameters.AddWithValue(i.InsertColumnParam(), row[i.Index]));
            accessorTo.Execute(command);
        });
    }

    private class TableInfo {
        public TableInfo() {
            ColumnInfos = new List<ColumnInfo>();
        }

        public string TableName { get; set; }

        public string Sql { get; set; }

        public List<ColumnInfo> ColumnInfos { get; }

        public bool HasBlob => ColumnInfos.Any(i => i.IsBlob);

        public IEnumerable<ColumnInfo> Blobs => ColumnInfos.Where(i => i.IsBlob);

        public void AddColumn(int index, string columnName, string columnType) {
            ColumnInfos.Add(new ColumnInfo {Index = index, ColumnName = columnName, ColumnType = columnType});
        }

        public string SelectQuery() {
            return @" SELECT " +
                   ColumnInfos.Aggregate(@"", (ret, item) => ret + item.SelectColumnName()) +
                   @", rowid FROM " +
                   TableName;
        }

        public string InsertQuery() {
            var query = @" INSERT " +
                        @" INTO " +
                        TableName +
                        @" ( " +
                        ColumnInfos.Aggregate(@"", (ret, item) => ret + item.InsertColumnName()) +
                        @" ) VALUES ( " +
                        ColumnInfos.Aggregate(@"", (ret, item) => ret + item.InsertColumnValue()) +
                        @" ) ";
            if (HasBlob) query += @"; SELECT last_insert_rowid(); ";

            return query;
        }
    }

    private class ColumnInfo {
        public int Index { get; set; }

        public string ColumnName { get; set; }

        public string ColumnType { get; set; }

        public bool IsBlob => @"BLOB".Equals(ColumnType.ToUpper());

        public string SelectColumnName() {
            if (IsBlob)
                return 0 == Index
                    ? @" length(" + ColumnName + @") AS " + ColumnName
                    : @", length(" + ColumnName + @") AS " + ColumnName;

            return 0 == Index ? @" " + ColumnName : @", " + ColumnName;
        }

        public string InsertColumnName() {
            return 0 == Index ? @" " + ColumnName : @", " + ColumnName;
        }

        public string InsertColumnValue() {
            if (IsBlob)
                return 0 == Index
                    ? @" zeroblob(@length_" + ColumnName + @")"
                    : @", zeroblob(@length_" + ColumnName + @")";

            return 0 == Index ? @" @" + ColumnName : @", @" + ColumnName;
        }

        public string InsertColumnParam() {
            if (IsBlob) return @"@length_" + ColumnName;

            return @"@" + ColumnName;
        }
    }
}