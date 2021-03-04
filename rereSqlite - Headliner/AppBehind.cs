/*
*
* AppBehind.cs
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
using System.Collections.ObjectModel;
using rereSqlite___Headliner.Data;
using rereSqlite___Headliner.Localization;
using rereSqlite___Headliner.UserControls;

namespace rereSqlite___Headliner {
    public class AppBehind {
        private AppBehind() {
            var r = new XReader {Directory = @".", FileName = @"Setting.config"};
            r.Parse();
            WindowHeight = int.Parse(r.Node.Find(@"SettingDef").Find(@"Window").Find(@"Height").NodeValue);
            WindowWidth = int.Parse(r.Node.Find(@"SettingDef").Find(@"Window").Find(@"Width").NodeValue);
            FontFamily = r.Node.Find(@"SettingDef").Find(@"Window").Find(@"FontFamily").NodeValue;
            FontSize = double.Parse(r.Node.Find(@"SettingDef").Find(@"Window").Find(@"FontSize").NodeValue);
            DataGridRowHeightPlus =
                double.Parse(r.Node.Find(@"SettingDef").Find(@"DataGrid").Find(@"RowHeightPlus").NodeValue);
            RunInitialize = @"Yes".Equals(r.Node.Find(@"SettingDef").Find(@"Initialize").NodeValue);
            ClonePriorTables = new List<string>();
            r.Node.Find(@"SettingDef").Find(@"Clone").Find(@"PriorTables").Children
                .ForEach(c => ClonePriorTables.Add(c.NodeValue));
            MainWindowCaptions = new MainWindowCaptions(r.Node);
            FileBrowseCaptions = new FileBrowseCaptions(r.Node);
            OFWindowCaptions = new OFWindowCaptions(r.Node);
            TableListCaptions = new TableListCaptions(r.Node);
            QueryStringInputCaptions = new QueryStringInputCaptions(r.Node);
            StringStorageCaptions = new StringStorageCaptions(r.Node);
            StringCardCaptions = new StringCardCaptions(r.Node);
            BinaryStorageCaptions = new BinaryStorageCaptions(r.Node);
            BinaryCardCaptions = new BinaryCardCaptions(r.Node);
            TagMasterCaptions = new TagMasterCaptions(r.Node);
            TagCardCaptions = new TagCardCaptions(r.Node);
            CloneCaptions = new CloneCaptions(r.Node);
        }

        public static AppBehind Get { get; } = new AppBehind();

        public static ObservableCollection<ComboBoxItem> Tags { get; set; }
            = new ObservableCollection<ComboBoxItem>();

        public int WindowHeight { get; }

        public int WindowWidth { get; }

        public string FontFamily { get; }

        public double FontSize { get; }

        public double DataGridRowHeightPlus { get; }

        public bool RunInitialize { get; }

        public List<string> ClonePriorTables { get; }

        public MainWindowCaptions MainWindowCaptions { get; }

        public FileBrowseCaptions FileBrowseCaptions { get; }

        public OFWindowCaptions OFWindowCaptions { get; }

        public TableListCaptions TableListCaptions { get; }

        public QueryStringInputCaptions QueryStringInputCaptions { get; }

        public StringStorageCaptions StringStorageCaptions { get; }

        public StringCardCaptions StringCardCaptions { get; }

        public BinaryStorageCaptions BinaryStorageCaptions { get; }

        public BinaryCardCaptions BinaryCardCaptions { get; }

        public TagMasterCaptions TagMasterCaptions { get; }

        public TagCardCaptions TagCardCaptions { get; }

        public CloneCaptions CloneCaptions { get; }

        public string DBFilePath { get; set; }

        public string Password { get; set; }

        public Action<string, Exception> AppendError { get; set; }

        public Action<string> AppendInfo { get; set; }

        public Action Reload { get; set; }

        public Action<string> SetQueryString { get; set; }

        public Action<SqliteAccessor> AddPage { get; set; }

        public void ReloadTags() {
            Tags.Clear();
            Tags.Add(new ComboBoxItem(@"", @""));
            var tItems = new TagMaster().Query();
            tItems.ForEach(row => { Tags.Add(new ComboBoxItem(row[0].ToString(), row[0].ToString())); });
        }
    }

    namespace Localization {
        public class MainWindowCaptions {
            public MainWindowCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"PageName");
                TabFileBrowse = page.Find(@"FileBrowse").NodeValue;
                TabTableList = page.Find(@"TableList").NodeValue;
                TabQueryStringInput = page.Find(@"QueryStringInput").NodeValue;
                TabQueryResultViewList = page.Find(@"QueryResultViewList").NodeValue;
                TabStringStorage = page.Find(@"StringStorage").NodeValue;
                TabBinaryStorage = page.Find(@"BinaryStorage").NodeValue;
                TabTagMaster = page.Find(@"TagMaster").NodeValue;
                TabClone = page.Find(@"Clone").NodeValue;
                TabRunningInformation = page.Find(@"RunningInformations").NodeValue;
            }

            public string TabFileBrowse { get; }

            public string TabTableList { get; }

            public string TabQueryStringInput { get; }

            public string TabQueryResultViewList { get; }

            public string TabStringStorage { get; }

            public string TabBinaryStorage { get; }

            public string TabTagMaster { get; }

            public string TabClone { get; }

            public string TabRunningInformation { get; }
        }

        public class FileBrowseCaptions {
            public FileBrowseCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"FileBrowse");
                PasswordInput = page.Find(@"PasswordInput").NodeValue;
                BrowseButton = page.Find(@"BrowseButton").NodeValue;
                NewFileButton = page.Find(@"NewFileButton").NodeValue;
                FilePathOutput = page.Find(@"FilePathOutput").NodeValue;
            }

            public string PasswordInput { get; }

            public string BrowseButton { get; }

            public string NewFileButton { get; }

            public string FilePathOutput { get; }
        }

        public class OFWindowCaptions {
            public OFWindowCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"OFWindow");
                OpenDatabaseTitle = page.Find(@"OpenDatabaseTitle").NodeValue;
                NewDatabaseTitle = page.Find(@"NewDatabaseTitle").NodeValue;
                OpenDatabaseButton = page.Find(@"OpenDatabaseButton").NodeValue;
                NewDatabaseButton = page.Find(@"NewDatabaseButton").NodeValue;
                FileNameColumn = page.Find(@"FileNameColumn").NodeValue;
                CloneDatabaseTitle = page.Find(@"CloneDatabaseTitle").NodeValue;
                OverwriteDialogTitle = page.Find(@"OverwriteDialogTitle").NodeValue;
                OverwriteMessage = page.Find(@"OverwriteMessage").NodeValue;
                SelectFileTitle = page.Find(@"SelectFileTitle").NodeValue;
                RetrieveFileTitle = page.Find(@"RetrieveFileTitle").NodeValue;
                SelectPathButton = page.Find(@"SelectPathButton").NodeValue;
                SaveFileButton = page.Find(@"SaveFileButton").NodeValue;
            }

            public string OpenDatabaseTitle { get; }

            public string NewDatabaseTitle { get; }

            public string OpenDatabaseButton { get; }

            public string NewDatabaseButton { get; }

            public string FileNameColumn { get; }

            public string CloneDatabaseTitle { get; }

            public string OverwriteDialogTitle { get; }

            public string OverwriteMessage { get; }

            public string SelectFileTitle { get; }

            public string RetrieveFileTitle { get; }

            public string SelectPathButton { get; }

            public string SaveFileButton { get; }
        }

        public class TableListCaptions {
            public TableListCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"TableList");
                ReloadButton = page.Find(@"ReloadButton").NodeValue;
                TableNameColumn = page.Find(@"TableNameColumn").NodeValue;
            }

            public string ReloadButton { get; }

            public string TableNameColumn { get; }
        }

        public class QueryStringInputCaptions {
            public QueryStringInputCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"QueryStringInput");
                ExecuteButton = page.Find(@"ExecuteButton").NodeValue;
                BeginButton = page.Find(@"BeginButton").NodeValue;
                CommitButton = page.Find(@"CommitButton").NodeValue;
                RollbackButton = page.Find(@"RollbackButton").NodeValue;
            }

            public string ExecuteButton { get; }

            public string BeginButton { get; }

            public string CommitButton { get; }

            public string RollbackButton { get; }
        }

        public class StringStorageCaptions {
            public StringStorageCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"StringStorage");
                KeyInput = page.Find(@"KeyInput").NodeValue;
                TagInput = page.Find(@"TagInput").NodeValue;
                SearchButton = page.Find(@"SearchButton").NodeValue;
            }

            public string KeyInput { get; }

            public string TagInput { get; }

            public string SearchButton { get; }
        }

        public class StringCardCaptions {
            public StringCardCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"StringCard");
                Key = page.Find(@"Key").NodeValue;
                Value = page.Find(@"Value").NodeValue;
                TagInputList = page.Find(@"TagInputList").NodeValue;
                RegisterButton = page.Find(@"RegisterButton").NodeValue;
            }

            public string Key { get; }

            public string Value { get; }

            public string TagInputList { get; }

            public string RegisterButton { get; }
        }

        public class BinaryStorageCaptions {
            public BinaryStorageCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"BinaryStorage");
                KeyInput = page.Find(@"KeyInput").NodeValue;
                TagInput = page.Find(@"TagInput").NodeValue;
                SearchButton = page.Find(@"SearchButton").NodeValue;
            }

            public string KeyInput { get; }

            public string TagInput { get; }

            public string SearchButton { get; }
        }

        public class BinaryCardCaptions {
            public BinaryCardCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"BinaryCard");
                Key = page.Find(@"Key").NodeValue;
                FileNameOutput = page.Find(@"FileNameOutput").NodeValue;
                TagInputList = page.Find(@"TagInputList").NodeValue;
                RetrieveFileButton = page.Find(@"RetrieveFileButton").NodeValue;
                SelectFileButton = page.Find(@"SelectFileButton").NodeValue;
                RegisterButton = page.Find(@"RegisterButton").NodeValue;
            }

            public string Key { get; }

            public string FileNameOutput { get; }

            public string TagInputList { get; }

            public string RetrieveFileButton { get; }

            public string SelectFileButton { get; }

            public string RegisterButton { get; }
        }

        public class TagMasterCaptions {
            public TagMasterCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"TagMaster");
                TagInput = page.Find(@"TagInput").NodeValue;
                SearchButton = page.Find(@"SearchButton").NodeValue;
            }

            public string TagInput { get; }

            public string SearchButton { get; }
        }

        public class TagCardCaptions {
            public TagCardCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"TagCard");
                TagInput = page.Find(@"TagInput").NodeValue;
                RegisterButton = page.Find(@"RegisterButton").NodeValue;
            }

            public string TagInput { get; }

            public string RegisterButton { get; }
        }

        public class CloneCaptions {
            public CloneCaptions(NodeEntity n) {
                var lang = n.Find(@"SettingDef").Find(@"Language").NodeValue;
                var page = n.Find(@"SettingDef").Find(@"Localization").Find(lang).Find(@"Clone");
                PasswordInput = page.Find(@"PasswordInput").NodeValue;
                SelectFileButton = page.Find(@"SelectFileButton").NodeValue;
                RunButton = page.Find(@"RunButton").NodeValue;
                FilePathOutput = page.Find(@"FilePathOutput").NodeValue;
            }

            public string PasswordInput { get; }

            public string SelectFileButton { get; }

            public string RunButton { get; }

            public string FilePathOutput { get; }
        }
    }
}