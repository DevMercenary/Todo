using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Command;

namespace TodoNotes.ViewModels
{
    public class MainMenuViewModel : BindableBase
    {
        public RelayCommand<FunctionEventArgs<object>> SwitchItemCmd => new(SwitchItem);

        private void SwitchItem(FunctionEventArgs<object> info) => Growl.Info((info.Info as SideMenuItem)?.Header.ToString());

        public RelayCommand<string> SelectCmd => new(Select);

        private void Select(string header) => Growl.Success(header);
        public MainMenuViewModel()
        {

        }
    }
}
