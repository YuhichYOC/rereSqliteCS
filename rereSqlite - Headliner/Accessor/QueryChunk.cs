/*
*
* QueryChunk.cs
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
using Microsoft.Data.Sqlite;

namespace rereSqlite___Headliner.Accessor {
    public class QueryChunk {
        private readonly SqliteAccessor accessor;

        private List<SqliteCommand> commands;

        private SqliteTransaction transaction;

        public QueryChunk() {
            accessor = new SqliteAccessor {DataSource = AppBehind.Get.DBFilePath, Password = AppBehind.Get.Password};
        }

        public bool TransactionAlreadyBegun => accessor.TransactionAlreadyBegun;

        public void Open() {
            accessor.Open();
            transaction = null;
            commands = new List<SqliteCommand>();
        }

        public void Begin() {
            if (TransactionAlreadyBegun) return;
            Open();
            transaction = accessor.Begin();
        }

        public void AddCommand(string arg) {
            var qs = arg.Replace(@"\r\n", @"\n").Split(';');
            foreach (var q in qs) {
                if (string.IsNullOrEmpty(q.Trim())) continue;
                accessor.QueryString = q;
                commands.Add(accessor.CreateCommand());
            }
        }

        public void Execute() {
            commands.ForEach(ExecuteSingleCommand);
            commands.Clear();
        }

        private void ExecuteSingleCommand(SqliteCommand command) {
            if (TransactionAlreadyBegun) command.Transaction = transaction;
            accessor.Execute(command);
            if (0 < accessor.QueryResult.Count)
                AppBehind.Get.AddPage(accessor);
        }

        public void Commit() {
            transaction.Commit();
            Close();
        }

        public void Rollback() {
            transaction.Rollback();
            Close();
        }

        public void Close() {
            accessor.Close();
        }
    }
}