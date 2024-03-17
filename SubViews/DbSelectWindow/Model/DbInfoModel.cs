using System.ComponentModel;
using System.Data;

namespace SubViews.DbSelectWindow.Model {

    internal class DbInfoModel : INotifyPropertyChanged {
        private string type;

        private string dataSource;

        private int portNumber;

        private string tenant;

        private string userId;

        private string password;

        private string fontFamily;

        private int fontSize;

        private bool selected;

        public int Index { set; get; }

        public DataTable Types { get; }

        public string Type {
            set {
                type = value;
                NotifyPropertyChanged(nameof(Type));
            }
            get => type;
        }

        public int DataSourceOFWButtonVisibility => IsSqlite() ? 0 : 2;

        public int DataSourceTextBoxVisibility => IsSqlite() ? 0 : 2;

        public string DataSource {
            set {
                dataSource = value;
                NotifyPropertyChanged(nameof(DataSource));
            }
            get => dataSource;
        }

        public int PortNumberVisibility => IsSqlite() ? 2 : 0;

        public int PortNumber {
            set {
                portNumber = value;
                NotifyPropertyChanged(nameof(PortNumber));
            }
            get => portNumber;
        }

        public int TenantVisibility => IsSqlite() ? 2 : 0;

        public string Tenant {
            set {
                tenant = value;
                NotifyPropertyChanged(nameof(Tenant));
            }
            get => tenant;
        }

        public string UserId {
            set {
                userId = value;
                NotifyPropertyChanged(nameof(UserId));
            }
            get => userId;
        }

        public string Password {
            set {
                password = value;
                NotifyPropertyChanged(nameof(Password));
            }
            get => password;
        }

        public string FontFamily {
            set {
                fontFamily = value;
                NotifyPropertyChanged(nameof(FontFamily));
            }
            get => fontFamily;
        }

        public int FontSize {
            set {
                fontSize = value;
                NotifyPropertyChanged(nameof(FontSize));
            }
            get => fontSize;
        }

        public bool Selected {
            set {
                selected = value;
                NotifyPropertyChanged(nameof(Selected));
            }
            get => selected;
        }

        public DbInfoModel() {
            Index = 0;
            Types = CreateTypesContents();
            type = string.Empty;
            dataSource = string.Empty;
            portNumber = 0;
            tenant = string.Empty;
            userId = string.Empty;
            password = string.Empty;
            fontFamily = @"Meiryo UI";
            fontSize = 11;
            selected = false;
        }

        private DataTable CreateTypesContents() {
            var t = new DataTable();
            t.Columns.Add(@"VALUE", System.Type.GetType(@"System.String")!);
            t.Columns.Add(@"CAPTION", System.Type.GetType(@"System.String")!);

            DataRow addRow;

            addRow = t.NewRow();
            addRow[@"VALUE"] = @"Postgres";
            addRow[@"CAPTION"] = @"Postgres";
            t.Rows.Add(addRow);

            addRow = t.NewRow();
            addRow[@"VALUE"] = @"Sqlserver";
            addRow[@"CAPTION"] = @"Sqlserver";
            t.Rows.Add(addRow);

            addRow = t.NewRow();
            addRow[@"VALUE"] = @"Sqlite";
            addRow[@"CAPTION"] = @"Sqlite";
            t.Rows.Add(addRow);

            return t;
        }

        private bool IsSqlite() => string.IsNullOrEmpty(type) ? false : @"Sqlite".Equals(type);

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}