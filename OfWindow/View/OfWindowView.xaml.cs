/*
*
* OfWindowView.xaml.cs
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
using System.Windows;
using System.Windows.Input;

namespace OfWindow.View {

    public partial class OfWindowView : Window {

        public new static readonly DependencyProperty FontFamilyProperty
            = DependencyProperty.Register(@"FontFamily", typeof(string), typeof(OfWindowView));

        public new static readonly DependencyProperty FontSizeProperty
            = DependencyProperty.Register(@"FontSize", typeof(int), typeof(OfWindowView));

        public new string FontFamily {
            set { SetValue(FontFamilyProperty, value); }
            get => (string)GetValue(FontFamilyProperty);
        }

        public new int FontSize {
            set { SetValue(FontSizeProperty, value); }
            get => (int)GetValue(FontSizeProperty);
        }

        public ICommand SelectFileDelg {
            set => ((ViewModel.OfWindowViewModel)DataContext).SelectFileDelg = value;
        }

        public OfWindowView() {
            InitializeComponent();
            ((ViewModel.OfWindowViewModel)DataContext).CloseDelg = new VoidCommand(() => true, Close);
        }
    }
}