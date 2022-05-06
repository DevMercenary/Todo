using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using TodoNotes.Models;

namespace TodoNotes.ViewModels
{
    public class NoteViewModel : BindableBase
    {
        readonly Note _model = new ();
        public NoteViewModel()
        {
            //таким нехитрым способом мы пробрасываем изменившиеся свойства модели во View
            _model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            AddCommand = new DelegateCommand<string>(str => {
                //проверка на валидность ввода - обязанность VM
                _model.AddNote(str);
            });
            RemoveCommand = new DelegateCommand<string?>(i => { 
                _model.RemoveNote(i);
            });
        }
        public DelegateCommand<string> AddCommand { get; }
        public DelegateCommand<string?> RemoveCommand { get; }
        public string Current => _model.CurrentNote;
        public ReadOnlyObservableCollection<string> MyNotes => _model.Notes;
    }
}
