using ICommandImpl;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SubViews.OfWindow.ViewModel {

    internal class FileSystemNodeViewModel : INotifyPropertyChanged {
        private Model.FileSystemNodeModel? model;

        private ObservableCollection<FileSystemNodeViewModel> children;

        private ObservableCollection<FileSystemNodeViewModel> files;

        private readonly ICommand refreshParentDelg;

        public string Name => model!.Name;

        public string Path => model!.Path;

        public ObservableCollection<FileSystemNodeViewModel> Children {
            set {
                children = value;
                NotifyPropertyChanged(nameof(Children));
            }
            get => children;
        }

        public ObservableCollection<FileSystemNodeViewModel> Files {
            set {
                files = value;
                NotifyPropertyChanged(nameof(Files));
            }
            get => files;
        }

        public bool IsExpanded {
            set {
                model!.IsExpanded = value;
                Describe(true);
            }
            get => model!.IsExpanded;
        }

        public bool IsSelected {
            set {
                model!.IsSelected = value;
                ((VoidCommand)refreshParentDelg).Execute(Path);
            }
            get => model!.IsSelected;
        }

        public FileSystemNodeViewModel(ICommand refreshParent) {
            children = new ObservableCollection<FileSystemNodeViewModel>();
            files = new ObservableCollection<FileSystemNodeViewModel>();
            this.refreshParentDelg = refreshParent;
        }

        public void Init(string path) {
            model = new Model.FileSystemNodeModel(path);
        }

        public void Describe(bool recursive = false) {
            if (model!.Described) return;

            var subDirectories = model!.GetDirectories();
            foreach (var directory in subDirectories) {
                var child = new FileSystemNodeViewModel(refreshParentDelg);
                child.Init(directory);
                if (recursive) {
                    child.Describe();
                }
                children.Add(child);
            }

            var subFiles = model!.GetFiles();
            foreach (var file in subFiles) {
                var child = new FileSystemNodeViewModel(refreshParentDelg);
                child.Init(file);
                children.Add(child);
                files.Add(child);
            }

            model.Described = true;
            NotifyPropertyChanged(nameof(Children));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}