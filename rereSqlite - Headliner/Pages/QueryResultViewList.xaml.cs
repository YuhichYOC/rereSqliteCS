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

using System.Windows.Media;

namespace rereSqlite___Headliner.Pages {
    public partial class QueryResultViewList {
        public QueryResultViewList() {
            InitializeComponent();
        }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
            Pager.AppendErrorDelegate += AppBehind.Get.AppendError;
            Pager.FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            Pager.FontSize = AppBehind.Get.FontSize;
            Pager.EnableRemovePage = true;
            Pager.Init();
        }

        public void AddPage(SqliteAccessor accessor) {
            var addPage = new QueryResultView();
            addPage.Init();
            addPage.Show(accessor);
            Pager.AddPage(@"Query " + (Pager.PagesCount + 1), addPage);
        }
    }
}