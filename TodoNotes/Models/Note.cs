using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace TodoNotes.Models
{
    public class Note : BindableBase
    {
        private ObservableCollection<string> _notes = new();
        public ReadOnlyObservableCollection<string> Notes;

        public Note()
        {
            _notes.Add("TEST");
            Notes = new ReadOnlyObservableCollection<string>(_notes);
  
        }

        public void AddNote(string note)
        {
            _notes.Add(note);
            RaisePropertyChanged(nameof(CurrentNote));
        }

        public void RemoveNote(string note)
        {
            _notes.Remove(note);
            RaisePropertyChanged(nameof(CurrentNote));
        }

        public string CurrentNote => $"W";
    }
}
