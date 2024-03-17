/*
*
* CustomDataGridViewModel.cs
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

using ICommandImpl;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace CustomUIs.CustomDataGrid.ViewModel {

    public class CustomDataGridViewModel : INotifyPropertyChanged, IVariantArg {
        private Model.CustomDataGridModel? model;

        public int PageId {
            set { model!.PageId = value; NotifyPropertyChanged(nameof(PageId)); }
            get => model!.PageId;
        }

        public string PageName {
            set { model!.PageName = value; NotifyPropertyChanged(nameof(PageName)); }
            get => model!.PageName;
        }

        public DataTable Data {
            set { model!.Data = value; NotifyPropertyChanged(nameof(Data)); }
            get => model!.Data;
        }

        public string FontFamily {
            set { model!.FontFamily = value; NotifyPropertyChanged(nameof(FontFamily)); }
            get => model!.FontFamily;
        }

        public int FontSize {
            set { model!.FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
            get => model!.FontSize;
        }

        public ICommand? PageCloseDelg { set; private get; }

        public ICommand? PageCloseCommand { private set; get; }

        public void Initialize(int pageId, string pageName, DataTable data, string fontFamily, int fontSize) {
            if (PageCloseDelg == null) throw new System.InvalidOperationException();
            model = new Model.CustomDataGridModel(pageId, pageName, data, fontFamily, fontSize);
            PageCloseCommand = new VoidCommand(() => true, () => ((VoidCommand)PageCloseDelg)?.Execute(this));
        }

        public void Refresh() => NotifyPropertyChanged(nameof(Data));

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string name = @"") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion INotifyPropertyChanged
    }
}