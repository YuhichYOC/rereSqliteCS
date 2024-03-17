/*
*
* FileSystemNodeViewModel.cs
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
using System.Windows.Input;

namespace OfWindow.ViewModel {

    internal class FileSystemNodeViewModel : INotifyPropertyChanged {
        private Model.FileSystemNodeModel? model;

        private ObservableCollection<FileSystemNodeViewModel> children;

        private ObservableCollection<FileSystemNodeViewModel> files;

        private readonly ICommand refreshParentDelg;

        public string Name => model!.Name;

        public string Path => model!.Path;

        public ObservableCollection<FileSystemNodeViewModel> Children {
            set { children = value; NotifyPropertyChanged(nameof(Children)); }
            get => children;
        }

        public ObservableCollection<FileSystemNodeViewModel> Files {
            set { files = value; NotifyPropertyChanged(nameof(Files)); }
            get => files;
        }

        public bool IsExpanded {
            set { model!.IsExpanded = value; Describe(true); }
            get => model!.IsExpanded;
        }

        public bool IsSelected {
            set { model!.IsSelected = value; ((VoidCommand)refreshParentDelg).Execute(Path); }
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