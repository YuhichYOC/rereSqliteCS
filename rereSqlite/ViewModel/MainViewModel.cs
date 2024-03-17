/*
*
* MainViewModel.cs
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

using ICommandImpl;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace rereSqlite.ViewModel {

    internal class MainViewModel : INotifyPropertyChanged {
        private readonly Model.MainModel model;

        public SubViews.TabPage1.ViewModel.TabPage1ViewModel TabPage1ViewModel { get; }

        public SubViews.TabPage2.ViewModel.TabPage2ViewModel TabPage2ViewModel { get; }

        public SubViews.TabPage3.ViewModel.TabPage3ViewModel TabPage3ViewModel { get; }

        public IList<IDictionary<string, object>> DbInfos {
            set => model.DbInfos = value;
            get => model.DbInfos;
        }

        public DataTable FontFamilies => model.FontFamilies;

        public string FontFamily {
            set { model.FontFamily = value; ChangeFont(); }
            get => model.FontFamily;
        }

        public int FontSize {
            set { model.FontSize = value; ChangeFont(); }
            get => model.FontSize;
        }

        public MainViewModel() {
            model = new Model.MainModel();
            var s = new Model.Settings();
            s.Read(@"settings.json");
            DbInfos = s.GetDbInfos();
            model.PrepareDbTypes(s.DbTypes);
            TabPage1ViewModel = new SubViews.TabPage1.ViewModel.TabPage1ViewModel();
            TabPage2ViewModel = new SubViews.TabPage2.ViewModel.TabPage2ViewModel();
            TabPage3ViewModel = new SubViews.TabPage3.ViewModel.TabPage3ViewModel();
            TabPage1ViewModel.DbTypes = model.DbTypes;
            TabPage1ViewModel.GetDbInfosDelg = new ListCommand<IDictionary<string, object>>(() => true, GetDbInfos);
            TabPage1ViewModel.SetDbInfoDelg = new VoidCommand(() => true, SetDbInfo);
            TabPage2ViewModel.Page3CountPagesDelg = new IntCommand(() => true, TabPage3ViewModel.CountPages);
            TabPage2ViewModel.Page3AddPageDelg = new VoidCommand(() => true, TabPage3ViewModel.AddPage);
            TabPage2ViewModel.Page3ClosePageDelg = new VoidCommand(() => true, TabPage3ViewModel.ClosePage);
            FontFamily = s.FontFamily;
            FontSize = s.FontSize;
        }

        private IList<IDictionary<string, object>> GetDbInfos() => DbInfos;

        private void SetDbInfo(int index, string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            model.SetDbInfo(index, type, dataSource, portNumber, tenant, userId, password);
            TabPage2ViewModel.SetDbInfo(type, dataSource, portNumber, tenant, userId, password);
        }

        private void ChangeFont() {
            NotifyPropertyChanged(nameof(FontFamily));
            NotifyPropertyChanged(nameof(FontSize));
            TabPage1ViewModel.FontFamily = model.FontFamily;
            TabPage1ViewModel.FontSize = model.FontSize;
            TabPage2ViewModel.FontFamily = model.FontFamily;
            TabPage2ViewModel.FontSize = model.FontSize;
            TabPage3ViewModel.FontFamily = model.FontFamily;
            TabPage3ViewModel.FontSize = model.FontSize;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}