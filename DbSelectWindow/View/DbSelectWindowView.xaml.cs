/*
*
* DbSelectWindowView.xaml.cs
*
* Copyright 2024 Yuichi Yoshii
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
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace DbSelectWindow.View {

    public partial class DbSelectWindowView : Window {

        public new static readonly DependencyProperty FontFamilyProperty
            = DependencyProperty.Register(@"FontFamily", typeof(string), typeof(DbSelectWindowView));

        public new static readonly DependencyProperty FontSizeProperty
            = DependencyProperty.Register(@"FontSize", typeof(int), typeof(DbSelectWindowView));

        public new string FontFamily {
            set { SetValue(FontFamilyProperty, value); ((ViewModel.MainViewModel)DataContext).SetFontFamily(value); }
            get => (string)GetValue(FontFamilyProperty);
        }

        public new int FontSize {
            set { SetValue(FontSizeProperty, value); ((ViewModel.MainViewModel)(DataContext)).SetFontSize(value); }
            get => (int)GetValue(FontSizeProperty);
        }

        public ICommand? SetDbInfoDelg {
            set => ((ViewModel.MainViewModel)DataContext).SetDbInfoDelg = value;
        }

        public DbSelectWindowView() {
            InitializeComponent();
            ((ViewModel.MainViewModel)DataContext).CloseDelg = new VoidCommand(() => true, Close);
        }

        public void Add(DataTable dbTypes, string type, string dataSource, int portNumber, string tenant, string userId, string password)
            => ((ViewModel.MainViewModel)DataContext).Add(dbTypes, type, dataSource, portNumber, tenant, userId, password);
    }
}