/*
*
* TagMaster.cs
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

namespace rereSqlite___Headliner.Data {
    public class TagMaster : DaoCommon {
        public void SetUp(AppBehind appBehind) {
            if (TableExists(appBehind)) return;
            CreateTable(appBehind);
        }

        public List<List<object>> Query(AppBehind appBehind) {
            var parameters = new Dictionary<string, string>();
            return Query(appBehind, parameters);
        }

        public List<List<object>> Query(AppBehind appBehind, string tag) {
            var parameters = new Dictionary<string, string> {{@"@tag", tag}};
            return Query(appBehind, QuerySelectWithCondition, parameters);
        }

        public static void Register(bool insert, AppBehind appBehind, string newTag, string oldTag) {
            using var accessor = new SqliteAccessor {
                DataSource = appBehind.DBFilePath,
                Password = appBehind.Password,
                QueryString = insert ? QueryInsert : QueryUpdate
            };
            accessor.Open();
            var command = accessor.CreateCommand();
            command.Parameters.AddWithValue(@"@tag", newTag);
            if (!insert) command.Parameters.AddWithValue(@"@oldTag", oldTag);
            accessor.Execute(command);
        }

        protected override string GetQueryTableExists() {
            return QueryTableExists;
        }

        protected override string GetQueryCreateTable() {
            return QueryCreateTable;
        }

        protected override string GetQuerySelect() {
            return QuerySelect;
        }

        #region -- Query Strings --

        private const string QueryTableExists =
            @" SELECT                                                                         " +
            @"     COUNT(NAME) AS COUNT_TABLES                                                " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE   = 'table'                                                           " +
            @" AND NAME   = 'TAG_MASTER'                                                      ";

        private const string QueryCreateTable =
            @" CREATE                                                                         " +
            @" TABLE                                                                          " +
            @"     TAG_MASTER                                                                 " +
            @"     (                                                                          " +
            @"     TAG   TEXT,                                                                " +
            @"     PRIMARY KEY                                                                " +
            @"       (                                                                        " +
            @"       TAG                                                                      " +
            @"       )                                                                        " +
            @"     )                                                                          ";

        private const string QuerySelect =
            @" SELECT                                                                         " +
            @"     TAG                                                                        " +
            @" FROM                                                                           " +
            @"     TAG_MASTER                                                                 ";

        private const string QuerySelectWithCondition =
            QuerySelect +
            @" WHERE                                                                          " +
            @"     TAG   LIKE @tag || '%'                                                     ";

        private const string QueryInsert =
            @" INSERT                                                                         " +
            @" INTO                                                                           " +
            @"     TAG_MASTER                                                                 " +
            @"     (                                                                          " +
            @"     TAG                                                                        " +
            @"     )                                                                          " +
            @" VALUES                                                                         " +
            @"     (                                                                          " +
            @"     @tag                                                                       " +
            @"     )                                                                          ";

        private const string QueryUpdate =
            @" UPDATE                                                                         " +
            @"     TAG_MASTER                                                                 " +
            @" SET                                                                            " +
            @"     TAG   = @tag                                                               " +
            @" WHERE                                                                          " +
            @"     TAG   = @oldTag                                                            ";

        #endregion
    }
}