using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Command;
using Prism.Mvvm;
using TabItem = HandyControl.Controls.TabItem;

namespace TodoNotes.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Менеджер задач";
        public List<TabItem> DataList { get; set; }

        public MainWindowViewModel()
        {
            DataList = new List<TabItem>();

            DataList.Add(new TabItem() {Header = $"{Guid.NewGuid()}"});
            DataList.Add(new TabItem() {Header = $"{Guid.NewGuid()}"});
            DataList.Add(new TabItem() {Header = $"{Guid.NewGuid()}"});
            DataList.Add(new TabItem() {Header = $"{Guid.NewGuid()}"});
            DataList.Add(new TabItem() {Header = $"{Guid.NewGuid()}"});
        }

        public RelayCommand<CancelRoutedEventArgs> ClosingCmd => new(Closing);

        private void Closing(CancelRoutedEventArgs args)
        {
            Growl.Info($"{(args.OriginalSource as TabItem)?.Header} Closing");
        }

        public RelayCommand<RoutedEventArgs> ClosedCmd => new(Closed);

        private void Closed(RoutedEventArgs args)
        {
            Growl.Info($"{(args.OriginalSource as TabItem)?.Header} Closed");
        }
    }
}
