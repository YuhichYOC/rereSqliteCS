/*
*
* DbInfoModel.cs
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

namespace DbSelectWindow.Model {

    internal class DbInfoModel {
        public int Index { set; get; } = 0;

        public DataTable? DbTypes { set; get; }

        public string Type { set; get; } = string.Empty;

        public int DataSourceOFWButtonVisibility => IsSqlite() ? 0 : 2;

        public int DataSourceTextBoxVisibility => IsSqlite() ? 0 : 2;

        public string DataSource { set; get; } = string.Empty;

        public int PortNumberVisibility => IsSqlite() ? 2 : 0;

        public int PortNumber { set; get; } = 0;

        public int TenantVisibility => IsSqlite() ? 2 : 0;

        public string Tenant { set; get; } = string.Empty;

        public string UserId { set; get; } = string.Empty;

        public string Password { set; get; } = string.Empty;

        public bool Selected { set; get; } = false;

        public string FontFamily { set; get; } = @"Meiryo UI";

        public int FontSize { set; get; } = 11;

        private bool IsSqlite() => !string.IsNullOrEmpty(Type) && @"Sqlite".Equals(Type);
    }
}