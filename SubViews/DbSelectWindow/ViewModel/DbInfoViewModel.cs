using ICommandImpl;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace SubViews.DbSelectWindow.ViewModel {

    internal class DbInfoViewModel : INotifyPropertyChanged, IVariantArg {
        private readonly Model.DbInfoModel model;

        public int Index {
            set {
                model.Index = value;
            }
            get => model.Index;
        }

        public DataTable Types => model.Types;

        public string Type {
            set {
                model.Type = value;
                NotifyPropertyChanged(nameof(Type));
                NotifyPropertyChanged(nameof(DataSourceTextBoxVisibility));
                NotifyPropertyChanged(nameof(PortNumberVisibility));
                NotifyPropertyChanged(nameof(TenantVisibility));
            }
            get => model.Type;
        }

        public int DataSourceTextBoxVisibility => model.DataSourceTextBoxVisibility;

        public string DataSource {
            set {
                model.DataSource = value;
                NotifyPropertyChanged(nameof(DataSource));
            }
            get => model.DataSource;
        }

        public ICommand OfWindowOpenCommand { get; }

        public int PortNumberVisibility => model.PortNumberVisibility;

        public int PortNumber {
            set {
                model.PortNumber = value;
                NotifyPropertyChanged(nameof(PortNumber));
            }
            get => model.PortNumber;
        }

        public int TenantVisibility => model.TenantVisibility;

        public string Tenant {
            set {
                model.Tenant = value;
                NotifyPropertyChanged(nameof(Tenant));
            }
            get => model.Tenant;
        }

        public string UserId {
            set {
                model.UserId = value;
                NotifyPropertyChanged(nameof(UserId));
            }
            get => model.UserId;
        }

        public string Password {
            set {
                model.Password = value;
                NotifyPropertyChanged(nameof(Password));
            }
            get => model.Password;
        }

        public string FontFamily {
            set {
                model.FontFamily = value;
                NotifyPropertyChanged(nameof(FontFamily));
            }
            get => model.FontFamily;
        }

        public int FontSize {
            set {
                model.FontSize = value;
                NotifyPropertyChanged(nameof(FontSize));
            }
            get => model.FontSize;
        }

        public bool Selected {
            set {
                model.Selected = value;
                NotifyPropertyChanged(nameof(Selected));
            }
            get => model.Selected;
        }

        public string Title => ToString();

        public DbInfoViewModel(Model.DbInfoModel m) {
            model = m;
            model.PropertyChanged += Model_PropertyChanged;
            OfWindowOpenCommand = new VoidCommand(() => true, OpenOfWindow);
        }

        internal DbInfoViewModel(
            int index,
            string type,
            string dataSource,
            int portNumber,
            string tenant,
            string userId,
            string password) {
            model = new Model.DbInfoModel() {
                Index = index,
                Type = type,
                DataSource = dataSource,
                PortNumber = portNumber,
                Tenant = tenant,
                UserId = userId,
                Password = password
            };
            model.PropertyChanged += Model_PropertyChanged;
            OfWindowOpenCommand = new VoidCommand(() => true, OpenOfWindow);
        }

        private void Model_PropertyChanged(object? s, PropertyChangedEventArgs e) {
            if (s == null) return;
            if (string.IsNullOrEmpty(e.PropertyName)) return;

            NotifyPropertyChanged(nameof(e.PropertyName));
        }

        private void OpenOfWindow() {
            var ofw = new OfWindow.View.OfWindowView();
            ofw.Show();
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