/*
*
* OracleAccessor.cs
*
* Copyright 2024 Yuichi Yoshii
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

using System.Data;

namespace DatabaseAccessor {

    public class OracleAccessor : IAccessor, IDisposable {
        private string dataSource = string.Empty;

        private int portNumber = int.MinValue;

        private string tenant = string.Empty;

        private string userId = string.Empty;

        private string password = string.Empty;

        private Oracle.ManagedDataAccess.Client.OracleConnection connection;

        private OracleCommand? command;

        private string queryString = string.Empty;

        public string DataSource { set => dataSource = value; }

        public int PortNumber { set => portNumber = value; }

        public string Tenant { set => tenant = value; }

        public string UserId { set => userId = value; }

        public string Password { set => password = value; }

        public string QueryString { set => queryString = value; }

        public OracleAccessor() => connection = new Oracle.ManagedDataAccess.Client.OracleConnection();

        public void Open() {
            if (connection.State != System.Data.ConnectionState.Closed) {
                return;
            }

            connection.ConnectionString = $"Data Source={dataSource}:{portNumber}/{tenant}; User Id={userId}; password={password};";
            connection.Open();
        }

        public ITransaction Begin() => new OracleTransaction(connection.BeginTransaction());

        public ICommand CreateCommand() {
            command = new OracleCommand(connection.CreateCommand());
            command.Self.CommandText = queryString;
            return command;
        }

        public void ExecuteNonQuery() {
            if (command == null) {
                throw new InvalidOperationException();
            }

            command.Self.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(ICommand command) => ((OracleCommand)command).Self.ExecuteNonQuery();

        public object ExecuteScalar() {
            if (command == null) {
                throw new InvalidOperationException();
            }

            return command.Self.ExecuteScalar() ?? DBNull.Value;
        }

        public object ExecuteScalar(ICommand command) => ((OracleCommand)command).Self.ExecuteScalar() ?? DBNull.Value;

        public DataSet Fetch() {
            if (command == null) {
                throw new InvalidOperationException();
            }

            DataSet ds = new();
            using (Oracle.ManagedDataAccess.Client.OracleDataAdapter a = new(command.Self)) {
                a.Fill(ds);
            }
            return ds;
        }

        public DataSet Fetch(ICommand command) {
            DataSet ds = new();
            using (Oracle.ManagedDataAccess.Client.OracleDataAdapter a = new(((OracleCommand)command).Self)) {
                a.Fill(ds);
            }
            return ds;
        }

        public void Close() {
            if (connection.State == ConnectionState.Closed) {
                return;
            }

            connection.Close();
        }

        public void Dispose() {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}