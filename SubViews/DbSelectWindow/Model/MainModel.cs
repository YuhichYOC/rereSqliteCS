using System.ComponentModel;

namespace SubViews.DbSelectWindow.Model {

    internal class MainModel : INotifyPropertyChanged {
        private int index;

        private string fontFamily;

        private int fontSize;

        public int Index {
            set {
                index = value;
                NotifyPropertyChanged(nameof(Index));
            }
            get => index;
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

        public MainModel() {
            index = 0;
            fontFamily = @"Meiryo UI";
            fontSize = 11;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}