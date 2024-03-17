/*
*
* TabPage2Model.cs
*
* Copyright 2023 Yuichi Yoshii
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

namespace SubViews.TabPage2.Model {

    internal class TabPage2Model {
        public string Type { set; get; } = string.Empty;

        public string DataSource { set; get; } = string.Empty;

        public int PortNumber { set; get; } = 0;

        public string Tenant { set; get; } = string.Empty;

        public string UserId { set; get; } = string.Empty;

        public string Password { set; get; } = string.Empty;

        public string QueryString { set; get; } = string.Empty;

        public string FontFamily { set; get; } = @"Meiryo UI";

        public int FontSize { set; get; } = 11;

        internal bool QueryAbleToRun() => !string.IsNullOrEmpty(QueryString);

        internal DataTable? RunQuery() {
            using DatabaseAccessor.IAccessor a = DatabaseAccessor.AccessorFactory.Create(Type);
            a.DataSource = DataSource;
            a.PortNumber = PortNumber;
            a.Tenant = Tenant;
            a.UserId = UserId;
            a.Password = Password;
            a.Open();
            a.QueryString = QueryString;
            a.CreateCommand();
            var qr = a.Fetch();
            if (qr == null || qr.Tables.Count == 0 || qr.Tables[0].Rows.Count == 0) {
                return null;
            }
            return qr.Tables[0];
        }
    }
}