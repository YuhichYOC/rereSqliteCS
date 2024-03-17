/*
*
* TabPage2ViewModel.cs
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
using System.ComponentModel;
using System.Windows.Input;

namespace SubViews.TabPage2.ViewModel {

    public class TabPage2ViewModel : INotifyPropertyChanged {
        private readonly Model.TabPage2Model model;

        public string QueryString {
            set {
                model.QueryString = value;
                NotifyPropertyChanged(nameof(QueryString));
                ((VoidCommand)RunQueryCommand).RaiseCanExecuteChanged();
            }
            get => model.QueryString;
        }

        public string FontFamily {
            set { model.FontFamily = value; NotifyPropertyChanged(nameof(FontFamily)); }
            get => model.FontFamily;
        }

        public int FontSize {
            set { model.FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
            get => model.FontSize;
        }

        public ICommand? Page3CountPagesDelg { set; private get; }

        public ICommand? Page3AddPageDelg { set; private get; }

        public ICommand? Page3ClosePageDelg { set; private get; }

        public ICommand RunFormatCommand { get; }

        public ICommand RunQueryCommand { get; }

        public TabPage2ViewModel() {
            model = new Model.TabPage2Model();
            RunFormatCommand = new VoidCommand(() => true, () => QueryString = new Formatter.SqlFormatter(QueryString).ToString());
            RunQueryCommand = new VoidCommand(model.QueryAbleToRun, RunQuery);
        }

        public void SetDbInfo(string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            model.Type = type;
            model.DataSource = dataSource;
            model.PortNumber = portNumber;
            model.Tenant = tenant;
            model.UserId = userId;
            model.Password = password;
        }

        private void RunQuery() {
            if (Page3CountPagesDelg == null || Page3AddPageDelg == null || Page3ClosePageDelg == null) return;
            var data = model.RunQuery();
            if (data == null) return;
            ((VoidCommand)Page3AddPageDelg).Execute(
                ((IntCommand)Page3CountPagesDelg).Execute(),
                $"Page {((IntCommand)Page3CountPagesDelg).Execute() + 1}",
                data,
                FontFamily,
                FontSize,
                Page3ClosePageDelg);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}