using ICommandImpl;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SubViews.OfWindow.ViewModel {

    internal class OfWindowViewModel : INotifyPropertyChanged {
        private readonly ObservableCollection<FileSystemNodeViewModel> blankFileList = new ObservableCollection<FileSystemNodeViewModel>();

        private ObservableCollection<FileSystemNodeViewModel> nodes;

        private string selectedPath = string.Empty;

        public ObservableCollection<FileSystemNodeViewModel> Nodes {
            set {
                nodes = value;
                NotifyPropertyChanged(nameof(Nodes));
            }
            get => nodes;
        }

        public ObservableCollection<FileSystemNodeViewModel> Files {
            get {
                var s = GetSelectedNode(nodes);
                if (s == null) {
                    return blankFileList;
                }
                return s.Files;
            }
        }

        public string SelectedPath => selectedPath;

        public OfWindowViewModel() {
            nodes = new ObservableCollection<FileSystemNodeViewModel>();
            Fill();
        }

        private void Fill() {
            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.Name == @"N:\") continue;
                if (drive.Name == @"P:\") continue;
                var add = new FileSystemNodeViewModel(new VoidCommand(() => true, Refresh));
                add.Init(drive.Name);
                nodes.Add(add);
            }
            NotifyPropertyChanged(nameof(Nodes));
        }

        private FileSystemNodeViewModel? GetSelectedNode(IEnumerable<FileSystemNodeViewModel> nodes) {
            var selected = nodes.FirstOrDefault(n => n.IsSelected);

            if (selected != null) return selected;

            foreach (var n in nodes) {
                selected = GetSelectedNode(n.Children);
                if (selected != null) return selected;
            }

            return null;
        }

        private void Refresh(string path) {
            NotifyPropertyChanged(nameof(Files));
            selectedPath = path;
            NotifyPropertyChanged(nameof(SelectedPath));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}