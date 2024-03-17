/*
*
* MainViewModel.cs
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

using ICommandImpl;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace DbSelectWindow.ViewModel {

    internal class MainViewModel : INotifyPropertyChanged {
        private readonly Model.MainModel model;

        public ObservableCollection<DbInfoViewModel> Dbs { get; }

        public int Index {
            set {
                if (model.Index == value) return;
                model.Index = value;
                NotifyPropertyChanged(nameof(Dbs));
                NotifyPropertyChanged(nameof(SelectedItem));
            }
            get => model.Index;
        }

        public DbInfoViewModel? SelectedItem {
            get {
                if (Dbs.Count == 0) return null;
                return Dbs[Index];
            }
        }

        internal ICommand DeleteCommand { get; }

        public ICommand SelectCommand { private set; get; }

        internal ICommand? SetDbInfoDelg { set; private get; }

        internal ICommand? CloseDelg { set; private get; }

        public MainViewModel() {
            model = new Model.MainModel();

            Dbs = new ObservableCollection<DbInfoViewModel>();

            DeleteCommand = new VoidCommand(() => true, Delete);
            SelectCommand = new VoidCommand(() => true, Select);
        }

        internal void Add(DataTable dbTypes, string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            Dbs.Add(new DbInfoViewModel(Dbs.Count - 1, type, dataSource, portNumber, tenant, userId, password) { DbTypes = dbTypes, NewDelg = new VoidCommand(() => true, New) });
            NotifyPropertyChanged(nameof(Dbs));
            if (Dbs.Count == 1) {
                NotifyPropertyChanged(nameof(Index));
                NotifyPropertyChanged(nameof(SelectedItem));
            }
        }

        internal void SetFontFamily(string fontFamily) => Dbs.ToList().ForEach(d => d.FontFamily = fontFamily);

        internal void SetFontSize(int fontSize) => Dbs.ToList().ForEach(d => d.FontSize = fontSize);

        private void New() => Index = Dbs.Count - 1;

        private void Delete(IVariantArg vm) {
            Index -= 1;
            Dbs.Remove((DbInfoViewModel)vm);
            NotifyPropertyChanged(nameof(Dbs));
        }

        private void Select() {
            ((VoidCommand)SetDbInfoDelg!).Execute(Index, Dbs[Index].Type, Dbs[Index].DataSource, Dbs[Index].PortNumber, Dbs[Index].Tenant, Dbs[Index].UserId, Dbs[Index].Password);
            ((VoidCommand)CloseDelg!).Execute();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}