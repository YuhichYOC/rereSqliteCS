/*
*
* Settings.cs
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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace rereSqlite.Model {

    internal class Settings {

        internal class DbInfo {

            [JsonProperty(nameof(Type))]
            internal string Type { set; get; } = string.Empty;

            [JsonProperty(nameof(DataSource))]
            internal string DataSource { set; get; } = string.Empty;

            [JsonProperty(nameof(PortNumber))]
            internal int PortNumber { set; get; } = 0;

            [JsonProperty(nameof(Tenant))]
            internal string Tenant { set; get; } = string.Empty;

            [JsonProperty(nameof(UserId))]
            internal string UserId { set; get; } = string.Empty;

            [JsonProperty(nameof(Password))]
            internal string Password { set; get; } = string.Empty;
        }

        [JsonProperty(nameof(DbInfos))]
        internal IList<DbInfo> DbInfos { set; get; } = new List<DbInfo>();

        [JsonProperty(nameof(DbTypes))]
        internal IList<string> DbTypes { set; get; } = new List<string>();

        [JsonProperty(nameof(FontFamily))]
        internal string FontFamily { set; get; } = string.Empty;

        [JsonProperty(nameof(FontSize))]
        internal int FontSize { set; get; } = 0;

        internal void Read(string path) {
            using StreamReader sr = new(path, Encoding.UTF8);
            var s = JsonConvert.DeserializeObject<Settings>(sr.ReadToEnd());
            DbInfos = s!.DbInfos;
            DbTypes = s!.DbTypes;
            FontFamily = s!.FontFamily;
            FontSize = s!.FontSize;
        }

        internal void Write(string path) {
            var s = new Settings {
                DbInfos = DbInfos,
                DbTypes = DbTypes,
                FontFamily = FontFamily,
                FontSize = FontSize
            };
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            sw.Write(JsonConvert.SerializeObject(s, Formatting.Indented));
        }

        internal IList<IDictionary<string, object>> GetDbInfos()
            => new List<IDictionary<string, object>>(DbInfos.Select(i => {
                return new Dictionary<string, object>() {
                { nameof(i.Type), i.Type },
                { nameof(i.DataSource), i.DataSource },
                { nameof(i.PortNumber), i.PortNumber },
                { nameof(i.Tenant), i.Tenant },
                { nameof(i.UserId), i.UserId },
                { nameof(i.Password), i.Password }
            };
            }));
    }
}