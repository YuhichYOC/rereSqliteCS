/*
*
* SqliteAccessor.cs
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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

public class SqliteAccessor : IDisposable {
    private const string Type_SqliteText = @"text";
    private const string Type_SqliteInteger = @"integer";
    private const string Type_SqliteReal = @"real";
    private const string Type_SqliteBlob = @"blob";

    private const string DotNet_Text = @"String";
    private const string DotNet_Integer = @"Integer";
    private const string DotNet_Real = @"Double";
    private const string DotNet_Blob = @"Object";
    private const string DotNet_Other = @"not supported";

    private string dataSource;
    private string password;

    private string queryString;

    public string DataSource {
        set => dataSource = value;
    }

    public string Password {
        set => password = value;
    }

    private string ConnectionString => string.IsNullOrEmpty(password)
        ? new SqliteConnectionStringBuilder {DataSource = dataSource}.ToString()
        : new SqliteConnectionStringBuilder {DataSource = dataSource, Password = password}.ToString();

    private SqliteConnection Connection { get; }

    public bool TransactionAlreadyBegun { get; private set; }

    public string QueryString {
        set => queryString = value;
    }

    public List<Tuple<string, string>> QueryResultAttributes { get; private set; }

    public List<List<object>> QueryResult { get; private set; }

    public SqliteAccessor() {
        dataSource = @"";
        password = @"";
        Connection = new SqliteConnection();
        QueryResultAttributes = new List<Tuple<string, string>>();
        QueryResult = new List<List<object>>();
    }

    public void Open() {
        if (null == Connection || ConnectionState.Closed != Connection.State) return;
        Connection.ConnectionString = ConnectionString;
        Connection.Open();
    }

    public SqliteTransaction Begin() {
        if (TransactionAlreadyBegun) return null;
        TransactionAlreadyBegun = true;
        return Connection.BeginTransaction();
    }

    public SqliteCommand CreateCommand() {
        if (null == Connection || ConnectionState.Open != Connection.State) return null;
        var ret = Connection.CreateCommand();
        if (string.IsNullOrEmpty(queryString)) return ret;
        ret.CommandText = queryString;
        return ret;
    }

    public void Execute(SqliteCommand command) {
        using var reader = command.ExecuteReader();
        if (!reader.HasRows) return;
        QueryResultAttributes = FetchResultAttributes(reader);
        QueryResult = FetchResult(reader);
    }

    private List<Tuple<string, string>> FetchResultAttributes(IDataRecord record) {
        var ret = new List<Tuple<string, string>>();
        for (var i = 0; record.FieldCount > i; ++i)
            ret.Add(new Tuple<string, string>(record.GetName(i),
                TypeNameFromSqliteTypeName(record.GetDataTypeName(i))));

        return ret;
    }

    private string TypeNameFromSqliteTypeName(string sqliteTypeName) {
        return sqliteTypeName.ToLower() switch {
            Type_SqliteText => DotNet_Text,
            Type_SqliteInteger => DotNet_Integer,
            Type_SqliteReal => DotNet_Real,
            Type_SqliteBlob => DotNet_Blob,
            _ => DotNet_Other
        };
    }

    private static List<List<object>> FetchResult(IDataReader reader) {
        var ret = new List<List<object>>();
        while (reader.Read()) {
            var addRow = new List<object>();
            for (var i = 0; reader.FieldCount > i; ++i) addRow.Add(reader.GetValue(i));
            ret.Add(addRow);
        }

        return ret;
    }

    public int ColumnIndexFromName(List<Tuple<string, string>> attributes, string name) {
        return attributes.IndexOf(attributes.First(a => name.Equals(a.Item1)));
    }

    public bool IsBlobColumn(List<Tuple<string, string>> attributes, int column) {
        return DotNet_Blob.Equals(attributes[column].Item2);
    }

    public void RetrieveBlob(SqliteCommand command, FileStream outputStream, int blobColumn) {
        using var reader = command.ExecuteReader();
        while (reader.Read()) {
            using var inputStream = reader.GetStream(blobColumn);
            inputStream.CopyTo(outputStream);
        }
    }

    public void WriteBlob(FileStream inputStream, string tableName, string columnName, long rowId) {
        using var outputStream = new SqliteBlob(Connection, tableName, columnName, rowId);
        inputStream.CopyTo(outputStream);
    }

    public object ExecuteScalar(SqliteCommand command) {
        return command.ExecuteScalar();
    }

    public void Close() {
        if (null == Connection || ConnectionState.Open != Connection.State) return;
        Connection.Close();
    }

    public void Dispose() {
        Dispose(true);
    }

    private void Dispose(bool value) {
        if (null != Connection && ConnectionState.Open == Connection.State) Connection.Close();
        Connection?.Dispose();
    }
}