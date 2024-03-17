/*
*
* TemplateSelector.cs
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

using System.Windows;
using System.Windows.Controls;

namespace DbSelectWindow.ViewModel {

    public class TemplateSelector : DataTemplateSelector {
        public DataTemplate? Default { set; private get; }

        public DataTemplate? Add { set; private get; }

        public DataTemplate? Sqlite { set; private get; }

        public DataTemplate? Others { set; private get; }

        public DataTemplate? New { set; private get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (Default == null) {
                if (item is not DbInfoViewModel)
                    return New!;

                if (string.IsNullOrEmpty(((DbInfoViewModel)item).Type))
                    return New!;

                if (((DbInfoViewModel)item).Type.Equals(@"Sqlite"))
                    return Sqlite!;

                return Others!;
            }

            if (item is not DbInfoViewModel)
                return Add!;

            if (string.IsNullOrEmpty(((DbInfoViewModel)item).Type))
                return Add!;

            return Default!;
        }
    }
}