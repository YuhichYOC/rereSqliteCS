using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

public partial class TabPager : UserControl {
    private Tuple<Button, Page> currentPage;

    private List<Tuple<Button, Page>> pages;

    public AppBehind AppBehind { get; set; }

    public bool EnableRemovePage { get; set; }

    public int PagesCount => pages.Count;

    public TabPager() {
        InitializeComponent();
        Prepare();
    }

    private void Prepare() {
        currentPage = null;
        pages = new List<Tuple<Button, Page>>();
        DataContext = this;
    }

    public void AddPage(string name, Page page) {
        var addButton = new Button {Content = name, Tag = page, Margin = new Thickness(0, 0, 2, 0)};
        addButton.Click += (senderObj, e) => {
            currentPage = pages.First(page => page.Item2 == ((Button) senderObj).Tag);
            SwitchPage();
        };
        tabs.Children.Add(addButton);
        currentPage = new Tuple<Button, Page>(addButton, page);
        pages.Add(currentPage);
    }

    public Page GetPage(string name) {
        return pages.First(page => name.Equals(page.Item1.Content)).Item2;
    }

    private void SwitchPage() {
        if (null == currentPage) {
            canvas.Content = null;
        }
        else {
            canvas.Content = currentPage.Item2;
            canvas.NavigationUIVisibility = NavigationUIVisibility.Hidden;
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
        tabs.Children.Remove(removePage.Item1);
        pages.Remove(removePage);
        SwitchPage();
    }

    private void RemovePage_Click(object sender, RoutedEventArgs e) {
        try {
            RemovePage();
        }
        catch (Exception ex) {
            AppBehind.AppendError(ex.Message, ex);
        }
    }
}