/*
*
* CustomComboBox.xaml.cs
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

using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CustomUIs.CustomComboBox {

    public partial class CustomComboBox : UserControl {

        public static readonly DependencyProperty SourceTableProperty
            = DependencyProperty.Register(@"SourceTable", typeof(DataTable), typeof(CustomComboBox));

        public static readonly DependencyProperty SelectedIndexProperty
            = DependencyProperty.Register(@"SelectedIndex", typeof(int), typeof(CustomComboBox));

        public static readonly DependencyProperty SelectedValueProperty
            = DependencyProperty.Register(@"SelectedValue", typeof(object), typeof(CustomComboBox));

        public static readonly DependencyProperty EditableProperty
            = DependencyProperty.Register(@"Editable", typeof(bool), typeof(CustomComboBox));

        public DataTable SourceTable {
            set => SetValue(SourceTableProperty, value);
            get => (DataTable)GetValue(SourceTableProperty);
        }

        public int SelectedIndex {
            set => SetValue(SelectedIndexProperty, value);
            get => (int)GetValue(SelectedIndexProperty);
        }

        public object SelectedValue {
            set => SetValue(SelectedValueProperty, value);
            get => GetValue(SelectedValueProperty);
        }

        public bool Editable {
            set => SetValue(EditableProperty, value);
            get => (bool)GetValue(EditableProperty);
        }

        public CustomComboBox() => InitializeComponent();
    }
}