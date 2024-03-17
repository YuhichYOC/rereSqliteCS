/*
*
* TabPage3ViewModel.cs
*
* Copyright 2023 Yuichi Yoshii
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

using CustomUIs.CustomDataGrid.ViewModel;
using ICommandImpl;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace SubViews.TabPage3.ViewModel {

    public class TabPage3ViewModel : INotifyPropertyChanged {
        private readonly Model.TabPage3Model model;

        public ObservableCollection<CustomDataGridViewModel> Pages { get; }

        public string FontFamily {
            set { model.FontFamily = value; ChangeFont(); }
            get => model.FontFamily;
        }

        public int FontSize {
            set { model.FontSize = value; ChangeFont(); }
            get => model.FontSize;
        }

        public TabPage3ViewModel() {
            model = new Model.TabPage3Model();
            Pages = new ObservableCollection<CustomDataGridViewModel>();
        }

        public int CountPages() => Pages.Count;

        public void AddPage(int pageId, string pageName, DataTable data, string fontFamily, int fontSize, ICommand pageCloseDelg) {
            var add = new CustomDataGridViewModel() { PageCloseDelg = pageCloseDelg };
            add.Initialize(pageId, pageName, data, fontFamily, fontSize);
            Pages.Add(add);
            add.Refresh();
        }

        public void ClosePage(IVariantArg p) {
            Pages.Remove((CustomDataGridViewModel)p);
            NotifyPropertyChanged(nameof(Pages));
        }

        private void ChangeFont() {
            NotifyPropertyChanged(nameof(FontFamily));
            NotifyPropertyChanged(nameof(FontSize));
            Pages.ToList().ForEach(p => { p.FontFamily = FontFamily; p.FontSize = FontSize; });
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}