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

using Microsoft.Data.Sqlite;
using System.Data;

namespace DatabaseAccessor {

    public class SqliteAccessor : IAccessor, IDisposable {
        private string dataSource = string.Empty;

        private int portNumber = int.MinValue;

        private string tenant = string.Empty;

        private string userId = string.Empty;

        private string password = string.Empty;

        private SqliteConnection connection;

        private SqliteCommand? command;

        private string queryString = string.Empty;

        public string DataSource { set => dataSource = value; }

        public int PortNumber { set => portNumber = value; }

        public string Tenant { set => tenant = value; }

        public string UserId { set => userId = value; }

        public string Password { set => password = value; }

        public string QueryString { set => queryString = value; }

        public SqliteAccessor() => connection = new SqliteConnection();

        public void Open() {
            if (connection.State != ConnectionState.Closed) {
                return;
            }

            var csb = string.IsNullOrEmpty(password)
                ? new SqliteConnectionStringBuilder() { DataSource = dataSource }
                : new SqliteConnectionStringBuilder() { DataSource = dataSource, Password = password };
            connection.ConnectionString = csb.ToString();
            connection.Open();
        }

        public ITransaction Begin() => new SqliteTransaction(connection.BeginTransaction());

        public ICommand CreateCommand() {
            command = new SqliteCommand(connection.CreateCommand());
            command.Self.CommandText = queryString;
            return command;
        }

        public void ExecuteNonQuery() {
            if (command == null) {
                throw new InvalidOperationException();
            }

            command.Self.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(ICommand command) => ((SqliteCommand)command).Self.ExecuteNonQuery();

        public object ExecuteScalar() {
            if (command == null) {
                throw new InvalidOperationException();
            }

            return command.Self.ExecuteScalar() ?? DBNull.Value;
        }

        public object ExecuteScalar(ICommand command) => ((SqliteCommand)command).Self.ExecuteScalar() ?? DBNull.Value;

        public DataSet Fetch() {
            if (command == null) {
                throw new InvalidOperationException();
            }

            DataSet ds;
            using (SqliteDataReader reader = command.Self.ExecuteReader()) {
                ds = Fill(reader, reader);
            }
            return ds;
        }

        public DataSet Fetch(ICommand command) {
            DataSet ds;
            using (SqliteDataReader reader = ((SqliteCommand)(command)).Self.ExecuteReader()) {
                ds = Fill(reader, reader);
            }
            return ds;
        }

        public void Close() {
            if (connection.State != ConnectionState.Open) {
                return;
            }

            connection.Close();
        }

        public void Dispose() {
            Close();
            GC.SuppressFinalize(this);
        }

        private DataSet Fill(IDataRecord record, IDataReader reader) {
            var t = Describe(record);

            var table = new DataTable();
            t.ForEach(column => {
                switch (column.Item2) {
                    case @"text":
                        table.Columns.Add(new DataColumn(column.Item1, Type.GetType(@"System.String")!));
                        break;

                    case @"integer":
                        table.Columns.Add(new DataColumn(column.Item1, Type.GetType(@"System.Int64")!));
                        break;

                    case @"real":
                        table.Columns.Add(new DataColumn(column.Item1, Type.GetType(@"System.Double")!));
                        break;

                    default:
                        table.Columns.Add(new DataColumn(column.Item1, Type.GetType(@"System.Object")!));
                        break;
                }
            });

            while (reader.Read()) {
                var add = table.NewRow();
                t.ForEach(column => {
                    switch (column.Item2) {
                        case @"text":
                            add[column.Item1] = reader.GetValue(column.Item3).ToString();
                            break;

                        case @"integer":
                            add[column.Item1] = (long)reader.GetValue(column.Item3);
                            break;

                        case @"real":
                            add[column.Item1] = (double)reader.GetValue(column.Item3);
                            break;

                        default:
                            add[column.Item1] = reader.GetValue(column.Item3);
                            break;
                    }
                });
                table.Rows.Add(add);
            }

            var ds = new DataSet();
            ds.Tables.Add(table);
            return ds;
        }

        private List<Tuple<string, string, int>> Describe(IDataRecord record) {
            var t = new List<Tuple<string, string, int>>();
            for (var i = 0; i < record.FieldCount; i++) {
                t.Add(new Tuple<string, string, int>(record.GetName(i), record.GetDataTypeName(i).ToLower(), i));
            }
            return t;
        }
    }
}