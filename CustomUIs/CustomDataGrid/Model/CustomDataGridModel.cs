/*
*
* CustomDataGridModel.cs
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

namespace CustomUIs.CustomDataGrid.Model {

    internal class CustomDataGridModel {
        public int PageId { set; get; }

        public string PageName { set; get; }

        public DataTable Data { set; get; }

        public string FontFamily { set; get; }

        public int FontSize { set; get; }

        public CustomDataGridModel(int pageId, string pageName, DataTable data, string fontFamily, int fontSize) {
            PageId = pageId;
            PageName = pageName;
            Data = data;
            FontFamily = fontFamily;
            FontSize = fontSize;
        }
    }
}