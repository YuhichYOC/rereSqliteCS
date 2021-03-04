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
        public void SetUp() {
            if (TableExists()) return;
            CreateTable();
        }

        public List<List<object>> Query() {
            var parameters = new Dictionary<string, string>();
            return Query(parameters);
        }

        public List<List<object>> Query(string tag) {
            var parameters = new Dictionary<string, string> {{@"@tag", tag}};
            return Query(SELECT_WITH_CONDITION, parameters);
        }

        public static void Register(bool insert, string newTag, string oldTag) {
            using var accessor = new SqliteAccessor {
                DataSource = AppBehind.Get.DBFilePath,
                Password = AppBehind.Get.Password,
                QueryString = insert ? INSERT : UPDATE
            };
            accessor.Open();
            var command = accessor.CreateCommand();
            command.Parameters.AddWithValue(@"@tag", newTag);
            if (!insert) command.Parameters.AddWithValue(@"@oldTag", oldTag);
            accessor.Execute(command);
        }

        protected override string GetQueryTableExists() {
            return TABLE_EXISTS;
        }

        protected override string GetQueryCreateTable() {
            return CREATE_TABLE;
        }

        protected override string GetQuerySelect() {
            return SELECT;
        }

        #region -- Query Strings --

        private const string TABLE_EXISTS =
            @" SELECT                                                                         " +
            @"     COUNT(NAME) AS COUNT_TABLES                                                " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE   = 'table'                                                           " +
            @" AND NAME   = 'TAG_MASTER'                                                      ";

        private const string CREATE_TABLE =
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

        private const string SELECT =
            @" SELECT                                                                         " +
            @"     TAG                                                                        " +
            @" FROM                                                                           " +
            @"     TAG_MASTER                                                                 ";

        private const string SELECT_WITH_CONDITION =
            SELECT +
            @" WHERE                                                                          " +
            @"     TAG   LIKE @tag || '%'                                                     ";

        private const string INSERT =
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

        private const string UPDATE =
            @" UPDATE                                                                         " +
            @"     TAG_MASTER                                                                 " +
            @" SET                                                                            " +
            @"     TAG   = @tag                                                               " +
            @" WHERE                                                                          " +
            @"     TAG   = @oldTag                                                            ";

        #endregion
    }
}