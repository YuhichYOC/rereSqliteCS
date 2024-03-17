/*
*
* TabPage1ViewModel.cs
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
using System.Windows.Input;

namespace SubViews.TabPage1.ViewModel {

    public class TabPage1ViewModel : INotifyPropertyChanged {
        private readonly Model.TabPage1Model model;

        public DataTable? DbTypes {
            set => model.DbTypes = value;
            get => model.DbTypes;
        }

        public string FontFamily {
            set { model.FontFamily = value; NotifyPropertyChanged(nameof(FontFamily)); }
            get => model.FontFamily;
        }

        public int FontSize {
            set { model.FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
            get => model.FontSize;
        }

        public ICommand? GetDbInfosDelg { set; private get; }

        public ICommand DbSelectWindowOpenCommand { get; }

        public ICommand? SetDbInfoDelg { set; private get; }

        public TabPage1ViewModel() {
            model = new Model.TabPage1Model();
            DbSelectWindowOpenCommand = new VoidCommand(() => true, DbSelectWindowOpen);
        }

        private void DbSelectWindowOpen() {
            if (GetDbInfosDelg == null)
                return;
            var settings = ((ListCommand<IDictionary<string, object>>)GetDbInfosDelg).Execute();
            var dsw = new DbSelectWindow.View.DbSelectWindowView();
            foreach (Dictionary<string, object> item in settings) {
                dsw.Add(DbTypes!, (string)item[@"Type"], (string)item[@"DataSource"], (int)item[@"PortNumber"], (string)item[@"Tenant"], (string)item[@"UserId"], (string)item[@"Password"]);
            }
            dsw.Add(DbTypes!, string.Empty, string.Empty, -1, string.Empty, string.Empty, string.Empty);
            dsw.SetDbInfoDelg = SetDbInfoDelg;
            dsw.FontFamily = FontFamily;
            dsw.FontSize = FontSize;
            dsw.ShowDialog();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}