/*
*
* OfWindowViewModel.cs
*
* Copyright 2022 Yuichi Yoshii
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
using System.IO;
using System.Windows.Input;

namespace OfWindow.ViewModel {

    internal class OfWindowViewModel : INotifyPropertyChanged {
        private readonly ObservableCollection<FileSystemNodeViewModel> blankFileList = new ObservableCollection<FileSystemNodeViewModel>();

        private ObservableCollection<FileSystemNodeViewModel> nodes;

        private string selectedPath = string.Empty;

        public ObservableCollection<FileSystemNodeViewModel> Nodes {
            set { nodes = value; NotifyPropertyChanged(nameof(Nodes)); }
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

        public string SelectedPath {
            set { selectedPath = value; NotifyPropertyChanged(nameof(SelectedPath)); }
            get => selectedPath;
        }

        public ICommand SelectFileCommand { private set; get; }

        internal ICommand? SelectFileDelg { set; private get; }

        internal ICommand? CloseDelg { set; private get; }

        public OfWindowViewModel() {
            nodes = new ObservableCollection<FileSystemNodeViewModel>();
            Fill();
            SelectFileCommand = new VoidCommand(() => true, SelectFile);
        }

        private void Fill() {
            foreach (var drive in DriveInfo.GetDrives()) {
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

        private void SelectFile() {
            ((VoidCommand)SelectFileDelg!).Execute(SelectedPath);
            ((VoidCommand)CloseDelg!).Execute();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}