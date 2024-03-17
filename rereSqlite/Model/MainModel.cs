/*
*
* MainModel.cs
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

using DatabaseAccessor;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace rereSqlite.Model {

    internal class MainModel : INotifyPropertyChanged {
        private string fontFamily;

        private int fontSize;

        private IList<string>? dbTypes;

        internal IList<IDictionary<string, object>> DbInfos { set; get; }

        internal DataTable? DbTypes { set; get; }

        internal int Index { set; get; }

        internal string Type { set; get; }

        internal string DataSource { set; get; }

        internal int PortNumber { set; get; }

        internal string Tenant { set; get; }

        internal string UserId { set; get; }

        internal string Password { set; get; }

        internal DataTable FontFamilies { private set; get; }

        internal string FontFamily {
            set {
                fontFamily = value;
                NotifyPropertyChanged(nameof(FontFamily));
            }
            get => fontFamily;
        }

        internal int FontSize {
            set {
                fontSize = value;
                NotifyPropertyChanged(nameof(FontSize));
            }
            get => fontSize;
        }

        internal MainModel() {
            DbInfos = new List<IDictionary<string, object>>();
            Index = 0;
            Type = string.Empty;
            DataSource = string.Empty;
            PortNumber = 0;
            Tenant = string.Empty;
            UserId = string.Empty;
            Password = string.Empty;
            FontFamilies = new DataTable();
            PrepareFontFamilies();
            fontFamily = @"Meiryo UI";
            fontSize = 11;
        }

        internal void PrepareDbTypes(IList<string> types) {
            dbTypes = types;
            DbTypes = new DataTable();
            DbTypes.Columns.Add(new DataColumn(@"CAPTION"));
            DbTypes.Columns.Add(new DataColumn(@"VALUE"));
            foreach (var type in dbTypes) {
                var add = DbTypes.NewRow();
                add[@"CAPTION"] = type;
                add[@"VALUE"] = type;
                DbTypes.Rows.Add(add);
            }
        }

        internal void SetDbInfo(int index, string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            Index = index;
            SetDbInfo(type, dataSource, portNumber, tenant, userId, password);
            CreateNewSqliteFileIfNotExists();
            if (index == DbInfos.Count) {
                UpdateDbInfos(type, dataSource, portNumber, tenant, userId, password);
            }
            else {
                UpdateDbInfos(index, type, dataSource, portNumber, tenant, userId, password);
            }
            SaveSettings();
        }

        private void SaveSettings()
            => new Settings {
                DbInfos = DbInfos.Select(i => new Settings.DbInfo() {
                    Type = i[@"Type"].ToString()!,
                    DataSource = i[@"DataSource"].ToString()!,
                    PortNumber = (int)i[@"PortNumber"],
                    Tenant = i[@"Tenant"].ToString()!,
                    UserId = i[@"UserId"].ToString()!,
                    Password = i[@"Password"].ToString()!
                }).ToList(),
                DbTypes = dbTypes!,
                FontFamily = fontFamily,
                FontSize = fontSize
            }.Write(@"settings.json");

        private void PrepareFontFamilies() {
            FontFamilies.Columns.Add(new DataColumn(@"CAPTION"));
            FontFamilies.Columns.Add(new DataColumn(@"VALUE"));
            var i = 1;
            foreach (var f in Fonts.SystemFontFamilies) {
                var add = FontFamilies.NewRow();
                add[@"CAPTION"] = $"{i} : {f.Source}";
                add[@"VALUE"] = f.Source;
                FontFamilies.Rows.Add(add);
                i++;
            }
        }

        private void CreateNewSqliteFileIfNotExists() {
            if (!Type.Equals(@"Sqlite")) return;
            if (File.Exists(DataSource)) return;
            using IAccessor accessor = AccessorFactory.Create(@"Sqlite");
            accessor.DataSource = DataSource;
            accessor.UserId = UserId;
            accessor.Password = Password;
            accessor.Open();
            accessor.Close();
        }

        private void SetDbInfo(string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            Type = type;
            DataSource = dataSource;
            PortNumber = portNumber;
            Tenant = tenant;
            UserId = userId;
            Password = password;
        }

        private void UpdateDbInfos(string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            DbInfos.Add(new Dictionary<string, object>() {
                { @"Type", type },
                { @"DataSource", dataSource },
                { @"PortNumber", portNumber },
                { @"Tenant", tenant },
                { @"UserId", userId },
                { @"Password", password }
            });
        }

        private void UpdateDbInfos(int index, string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            DbInfos[index][@"Type"] = type;
            DbInfos[index][@"DataSource"] = dataSource;
            DbInfos[index][@"PortNumber"] = portNumber;
            DbInfos[index][@"Tenant"] = tenant;
            DbInfos[index][@"UserId"] = userId;
            DbInfos[index][@"Password"] = password;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}