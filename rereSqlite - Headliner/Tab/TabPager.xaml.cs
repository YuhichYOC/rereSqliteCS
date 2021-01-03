/*
*
* TabPager.cs
*
* Copyright 2020 Yuichi Yoshii
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

public partial class TabPager {
    public Action<string, Exception> AppendErrorDelegate;
    private Tuple<Button, Page> currentPage;

    private List<Tuple<Button, Page>> pages;

    public TabPager() {
        InitializeComponent();
    }

    public bool EnableRemovePage { get; set; }

    public int PagesCount => pages.Count;

    public void Init() {
        currentPage = null;
        pages = new List<Tuple<Button, Page>>();
        RemovePageButton.IsEnabled = EnableRemovePage;
    }

    public void AddPage(string name, Page page) {
        var addButton = new Button {Content = name, Tag = page, Margin = new Thickness(2)};
        addButton.Click += (senderObj, e) => {
            currentPage = pages.First(page => page.Item2 == ((Button) senderObj).Tag);
            SwitchPage();
        };
        Tabs.Children.Add(addButton);
        currentPage = new Tuple<Button, Page>(addButton, page);
        pages.Add(currentPage);
    }

    public Page GetPage(string name) {
        return pages.First(page => name.Equals(page.Item1.Content)).Item2;
    }

    private void SwitchPage() {
        if (null == currentPage) {
            Canvas.Content = null;
        }
        else {
            Canvas.Content = currentPage.Item2;
            Canvas.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }
    }

    public void SwitchPage(int pageIndex) {
        if (0 > pageIndex || PagesCount - 1 < pageIndex) return;
        currentPage = pages[pageIndex];
        SwitchPage();
    }

    public void RemovePage() {
        if (null == currentPage) return;
        var removePage = currentPage;
        currentPage = 0 >= pages.IndexOf(currentPage) ? null : pages[pages.IndexOf(currentPage) - 1];
        Tabs.Children.Remove(removePage.Item1);
        pages.Remove(removePage);
        SwitchPage();
    }

    private void RemovePage_Click(object sender, RoutedEventArgs e) {
        try {
            RemovePage();
        }
        catch (Exception ex) {
            AppendErrorDelegate?.Invoke(ex.Message, ex);
        }
    }
}