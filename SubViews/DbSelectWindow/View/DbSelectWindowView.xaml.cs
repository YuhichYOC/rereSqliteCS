using System.Windows;

namespace SubViews.DbSelectWindow.View {

    public partial class DbSelectWindowView : Window {

        public DbSelectWindowView() => InitializeComponent();

        public void Add(string type, string dataSource, int portNumber, string tenant, string userId, string password)
            => ((ViewModel.MainViewModel)DataContext).Add(type, dataSource, portNumber, tenant, userId, password);
    }
}