/*
*
* DaoCommon.cs
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
using System.Linq;

namespace rereSqlite___Headliner.Data {
    public abstract class DaoCommon {
        protected abstract string GetQueryTableExists();

        protected abstract string GetQueryCreateTable();

        protected abstract string GetQuerySelect();

        protected bool TableExists(AppBehind appBehind) {
            using var accessor = new SqliteAccessor {
                DataSource = appBehind.DBFilePath,
                Password = appBehind.Password,
                QueryString = GetQueryTableExists()
            };
            accessor.Open();
            return 0 < (long) accessor.ExecuteScalar(accessor.CreateCommand());
        }

        protected void CreateTable(AppBehind appBehind) {
            using var accessor = new SqliteAccessor {
                DataSource = appBehind.DBFilePath,
                Password = appBehind.Password,
                QueryString = GetQueryCreateTable()
            };
            accessor.Open();
            accessor.Execute(accessor.CreateCommand());
        }

        protected List<List<object>> Query(AppBehind appBehind, string query,
            Dictionary<string, string> parameters) {
            using var accessor = new SqliteAccessor {
                DataSource = appBehind.DBFilePath,
                Password = appBehind.Password,
                QueryString = query
            };
            accessor.Open();
            var command = accessor.CreateCommand();
            parameters.ToList().ForEach(p => { command.Parameters.AddWithValue(p.Key, p.Value); });
            accessor.Execute(command);
            return CloneQueryResult(accessor.QueryResult);
        }

        protected List<List<object>> Query(AppBehind appBehind, Dictionary<string, string> parameters) {
            return Query(appBehind, GetQuerySelect(), parameters);
        }

        private static List<List<object>> CloneQueryResult(List<List<object>> cloneFrom) {
            var ret = new List<List<object>>();
            cloneFrom.ForEach(row => { ret.Add(new List<object>(row)); });
            return ret;
        }
    }
}