/*
*
* DbInfoViewModel.cs
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
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace DbSelectWindow.ViewModel {

    internal class DbInfoViewModel : INotifyPropertyChanged, IVariantArg {
        private readonly Model.DbInfoModel model;

        public int Index {
            set => model.Index = value;
            get => model.Index;
        }

        public DataTable? DbTypes {
            set => model.DbTypes = value;
            get => model.DbTypes;
        }

        public string Type {
            set {
                model.Type = value;
                NotifyPropertyChanged(nameof(Type));
                NotifyPropertyChanged(nameof(PortNumberEnabled));
                NotifyPropertyChanged(nameof(TenantEnabled));
                NotifyPropertyChanged(nameof(Title));
                ((VoidCommand)SelectSqliteFileCommand).RaiseCanExecuteChanged();
            }
            get => model.Type;
        }

        public string DataSource {
            set {
                model.DataSource = value;
                NotifyPropertyChanged(nameof(DataSource));
                NotifyPropertyChanged(nameof(Title));
            }
            get => model.DataSource;
        }

        public bool PortNumberEnabled => !model.Type.Equals(@"Sqlite");

        public int PortNumber {
            set { model.PortNumber = value; NotifyPropertyChanged(nameof(PortNumber)); }
            get => model.PortNumber;
        }

        public bool TenantEnabled => !model.Type.Equals(@"Sqlite");

        public string Tenant {
            set { model.Tenant = value; NotifyPropertyChanged(nameof(Tenant)); }
            get => model.Tenant;
        }

        public string UserId {
            set { model.UserId = value; NotifyPropertyChanged(nameof(UserId)); }
            get => model.UserId;
        }

        public string Password {
            set { model.Password = value; NotifyPropertyChanged(nameof(Password)); }
            get => model.Password;
        }

        public bool Selected {
            set { model.Selected = value; NotifyPropertyChanged(nameof(Selected)); }
            get => model.Selected;
        }

        // これを internal スコープにするとバインドが効かない。もういい加減にしてくれ
        public string Title => ToString();

        public string FontFamily {
            set { model.FontFamily = value; NotifyPropertyChanged(nameof(FontFamily)); }
            get => model.FontFamily;
        }

        public int FontSize {
            set { model.FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
            get => model.FontSize;
        }

        internal ICommand? NewDelg { set; private get; }

        public ICommand? NewCommand { set; get; }

        public ICommand SelectSqliteFileCommand { private set; get; }

        internal DbInfoViewModel(Model.DbInfoModel m) {
            model = m;
            NewCommand = new VoidCommand(() => true, New);
            SelectSqliteFileCommand = new VoidCommand(CanExecuteSelectSqliteFileCommand, SelectSqliteFile);
        }

        internal DbInfoViewModel(int index, string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            model = new Model.DbInfoModel() {
                Index = index,
                Type = type,
                DataSource = dataSource,
                PortNumber = portNumber,
                Tenant = tenant,
                UserId = userId,
                Password = password
            };
            NewCommand = new VoidCommand(() => true, New);
            SelectSqliteFileCommand = new VoidCommand(CanExecuteSelectSqliteFileCommand, SelectSqliteFile);
        }

        private void New() => ((VoidCommand)NewDelg!).Execute();

        private bool CanExecuteSelectSqliteFileCommand() => Type.Equals(@"Sqlite");

        private void SelectSqliteFile() {
            var ofw = new OfWindow.View.OfWindowView();
            ofw.SelectFileDelg = new VoidCommand(() => true, (string path) => DataSource = path);
            ofw.FontFamily = FontFamily;
            ofw.FontSize = FontSize;
            ofw.ShowDialog();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged

        #region Object

        public override string ToString() => $"{model.Type} {model.DataSource}";

        #endregion Object
    }
}