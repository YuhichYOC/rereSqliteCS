using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SubViews.OfWindow.Model {

    internal class FileSystemNodeModel : INotifyPropertyChanged {
        private bool isExpanded;

        private bool isSelected;

        public string Name { set; get; } = string.Empty;

        public string Path { set; get; } = string.Empty;

        public bool Described { set; get; } = false;

        public bool IsDirectory { set; get; } = false;

        public bool IsExpanded {
            set {
                isExpanded = value;
                NotifyPropertyChanged(nameof(IsExpanded));
            }
            get => isExpanded;
        }

        public bool IsSelected {
            set {
                isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
            get => isSelected;
        }

        public FileSystemNodeModel(string path) {
            if (DriveInfo.GetDrives().Any(d => d.Name == path)) {
                Name = path;
            }
            else {
                Name = System.IO.Path.GetFileName(path);
            }
            NotifyPropertyChanged(nameof(Name));

            Path = path;
            NotifyPropertyChanged(nameof(Path));

            isExpanded = false;
            isSelected = false;
        }

        public IEnumerable<string> GetDirectories() {
            try {
                return Directory.GetDirectories(Path);
            }
            catch (Exception) {
                return Array.Empty<string>();
            }
        }

        public IEnumerable<string> GetFiles() {
            try {
                return Directory.GetFiles(Path);
            }
            catch (Exception) {
                return Array.Empty<string>();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}