using ICommandImpl;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SubViews.DbSelectWindow.ViewModel {

    internal class MainViewModel : INotifyPropertyChanged {
        private readonly Model.MainModel model;

        public ObservableCollection<DbInfoViewModel> Dbs { get; }

        public int Index {
            set {
                if (model.Index == value) return;
                model.Index = value;
                NotifyPropertyChanged(nameof(Dbs));
                NotifyPropertyChanged(nameof(Type));
                NotifyPropertyChanged(nameof(DataSource));
                NotifyPropertyChanged(nameof(PortNumber));
                NotifyPropertyChanged(nameof(Tenant));
                NotifyPropertyChanged(nameof(UserId));
                NotifyPropertyChanged(nameof(Password));
            }
            get => model.Index;
        }

        public string? Type => Dbs.Count == 0 ? null : Dbs[Index].Type;

        public string? DataSource => Dbs.Count == 0 ? null : Dbs[Index].DataSource;

        public int? PortNumber => Dbs.Count == 0 ? null : Dbs[Index].PortNumber;

        public string? Tenant => Dbs.Count == 0 ? null : Dbs[Index].Tenant;

        public string? UserId => Dbs.Count == 0 ? null : Dbs[Index].UserId;

        public string? Password => Dbs.Count == 0 ? null : Dbs[Index].Password;

        internal ICommand NewCommand { get; }

        internal ICommand DeleteCommand { get; }

        public MainViewModel() {
            model = new Model.MainModel();

            Dbs = new ObservableCollection<DbInfoViewModel>();

            NewCommand = new VoidCommand(() => true, New);
            DeleteCommand = new VoidCommand(() => true, Delete);
        }

        internal void Add(string type, string dataSource, int portNumber, string tenant, string userId, string password) {
            Dbs.Add(new DbInfoViewModel(Dbs.Count - 1, type, dataSource, portNumber, tenant, userId, password));
            NotifyPropertyChanged(nameof(Dbs));
            if (Dbs.Count == 1) {
                NotifyPropertyChanged(nameof(Index));
                NotifyPropertyChanged(nameof(Type));
                NotifyPropertyChanged(nameof(DataSource));
                NotifyPropertyChanged(nameof(PortNumber));
                NotifyPropertyChanged(nameof(Tenant));
                NotifyPropertyChanged(nameof(UserId));
                NotifyPropertyChanged(nameof(Password));
            }
        }

        private void New() {
            Dbs.Add(new DbInfoViewModel(Dbs.Count - 1, string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty));
            NotifyPropertyChanged(nameof(Dbs));
            Index = Dbs.Count - 1;
        }

        private void Delete(IVariantArg vm) {
            Index -= 1;
            Dbs.Remove((DbInfoViewModel)vm);
            NotifyPropertyChanged(nameof(Dbs));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}