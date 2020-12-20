/*
*
* QueryResultViewList.cs
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

using System.Windows.Controls;
using System.Windows.Media;

namespace rereSqlite___Headliner.Pages {
    public partial class QueryResultViewList : Page {
        private AppBehind appBehind;

        public QueryResultViewList() {
            InitializeComponent();
            Prepare();
        }

        public AppBehind AppBehind {
            set {
                appBehind = value;
                FontFamily = new FontFamily(appBehind.FontFamily);
                FontSize = appBehind.FontSize;
                pager.AppBehind = appBehind;
            }
        }

        private void Prepare() {
            pager.EnableRemovePage = true;
        }

        public void AddPage(SqliteAccessor accessor) {
            var addPage = new QueryResultView {AppBehind = appBehind};
            addPage.Show(accessor);
            pager.AddPage(@"Query " + (pager.PagesCount + 1), addPage);
        }
    }
}