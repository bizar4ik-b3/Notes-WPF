using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Notes
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        ApplicationContext db;
        RelayCommand addCommand;
        RelayCommand editCommand;
        RelayCommand deleteCommand;
        IEnumerable<Note> notes;

        public IEnumerable<Note> Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }

        public ApplicationViewModel()
        {
            db = new ApplicationContext();
            db.Notes.Load();
            Notes = db.Notes.Local.ToBindingList();
        }
        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      NoteWindow noteWindow = new NoteWindow(new Note());
                      if (noteWindow.ShowDialog() == true)
                      {
                          Note note = noteWindow.Note;
                          db.Notes.Add(note);
                          db.SaveChanges();
                      }
                  }));
            }
        }
        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Note note = selectedItem as Note;

                      Note vm = new Note()
                      {
                          Id = note.Id,
                          Text = note.Text,
                          Title = note.Title
                      };
                      NoteWindow noteWindow = new NoteWindow(vm);


                      if (noteWindow.ShowDialog() == true)
                      {
                          // получаем измененный объект
                          note = db.Notes.Find(noteWindow.Note.Id);
                          if (note != null)
                          {
                              note.Text = noteWindow.Note.Text;
                              note.Title = noteWindow.Note.Title;
                              db.Entry(note).State = EntityState.Modified;
                              db.SaveChanges();
                          }
                      }
                  }));
            }
        }
        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Note phone = selectedItem as Note;
                      db.Notes.Remove(phone);
                      db.SaveChanges();
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
