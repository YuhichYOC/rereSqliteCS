/*
*
* Schema.cs
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

namespace rereSqlite___Headliner.Data {
    public class Schema : DaoCommon {
        public List<List<object>> Query() {
            return Query(new Dictionary<string, string>());
        }

        public List<List<object>> Query(string filter) {
            return Query(QuerySelectTable, new Dictionary<string, string> {{@"@filter", filter}});
        }

        public List<List<object>> QueryTableInfo(string tableName) {
            return Query(@" PRAGMA table_info ('" + tableName + @"') ", new Dictionary<string, string>());
        }

        protected override string GetQuerySelect() {
            return QuerySelect;
        }

        protected override string GetQueryCreateTable() {
            // Nothing to do
            throw new NotImplementedException();
        }

        protected override string GetQueryTableExists() {
            // Nothing to do
            throw new NotImplementedException();
        }

        #region -- Query Strings --

        private const string QuerySelect =
            @" SELECT                                                                         " +
            @"     NAME,                                                                      " +
            @"     SQL                                                                        " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE   = 'table'                                                           " +
            @" ORDER BY                                                                       " +
            @"     NAME                                                                       ";

        private const string QuerySelectTable =
            @" SELECT                                                                         " +
            @"     NAME,                                                                      " +
            @"     SQL                                                                        " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE      = 'table'                                                        " +
            @" AND NAME   LIKE '%' || @filter || '%'                                          " +
            @" ORDER BY                                                                       " +
            @"     NAME                                                                       ";

        #endregion
    }
}