using LibraryProject.Core;
using LibraryProject.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibraryProject.ViewModel.AddViewModel
{
    public class AddBookViewModel : BaseViewModel
    {
        public Library_DBContext dbContext = new Library_DBContext();

        public List<Genre> Genres { get; set; }
        public List<Author> Authors { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<Department> Departaments { get; set; }

        public Book AddBookVM { get; set; }

        public decimal FullPrice { get; set; }

        private ObservableCollection<BookGenreCross> thisGenres { get; set; }
        public ObservableCollection<BookGenreCross> ThisGenres
        {
            get => thisGenres;
            set
            {
                thisGenres = value;
                SignalChanged();
            }
        }

        private Genre selectedGenre { get;set; }
        public Genre SelectedGenre
        {
            get => selectedGenre;
            set
            {
                selectedGenre = value;
                SignalChanged();
            }
        }

        private BookGenreCross selectedThisGenre { get; set; }
        public BookGenreCross SelectedThisGenre
        {
            get => selectedThisGenre;
            set
            {
                selectedThisGenre = value;
                SignalChanged();
            }
        }

        private Author selectedAuthor { get; set; }
        public Author SelectedAuthor
        {
            get => selectedAuthor;
            set
            {
                selectedAuthor = value;
                SignalChanged();
            }
        }

        private BookAuthorCross selectedThisAuthor { get; set; }
        public BookAuthorCross SelectedThisAuthor
        {
            get => selectedThisAuthor;
            set
            {
                selectedThisAuthor = value;
                SignalChanged();
            }
        }

        private ObservableCollection<BookAuthorCross> thisAuthors { get; set; }
        public ObservableCollection<BookAuthorCross> ThisAuthors
        {
            get => thisAuthors;
            set
            {
                thisAuthors = value;
                SignalChanged();
            }
        }

        private Publisher selectedPublisher { get; set; }
        public Publisher SelectedPublisher
        { 
            get => selectedPublisher;
            set
            {
                selectedPublisher = value;
                SignalChanged();
            }
        }

        private Department selectedDepartament { get; set; }
        public Department SelectedDepartament
        {
            get => selectedDepartament;
            set
            {
                selectedDepartament = value;
                SignalChanged();
            }
        }

        public CustomCommand AddGenre { get; set; }
        public CustomCommand AddAuthor { get; set; }
        public CustomCommand DeleteGenre { get; set; }
        public CustomCommand DeleteAuthor { get; set; }
        public CustomCommand SaveBook { get; set; }
        public CustomCommand Cancel { get; set; }

        public AddBookViewModel(Book book)
        {
            Genres = dbContext.Genres.ToList();
            Authors = dbContext.Authors.ToList();
            Publishers = dbContext.Publishers.ToList();
            Departaments = dbContext.Departments.ToList();
            if(book == null)
            {
                AddBookVM = new Book { };
            }
            else
            {
                GetInfo(book);
                AddBookVM = new Book
                {
                    Id = book.Id,
                    BookName = book.BookName,
                    Count = book.Count,
                    YearPublished = book.YearPublished,
                    Price = book.Price,
                    Departament = book.Departament,
                    DepartamentId = book.DepartamentId,
                    Publisher = book.Publisher,
                    PublisherId = book.PublisherId,
                    BookGenreCrosses = book.BookGenreCrosses,
                    BookAuthorCrosses = book.BookAuthorCrosses
                };

                SelectedDepartament = Departaments.FirstOrDefault(s=> s.Id == AddBookVM.DepartamentId);
                SelectedPublisher = Publishers.FirstOrDefault(s=> s.Id == AddBookVM.PublisherId);
                ThisAuthors = new ObservableCollection<BookAuthorCross>(book.BookAuthorCrosses);

                foreach(var author in ThisAuthors)
                {
                    author.Author = dbContext.Authors.FirstOrDefault(s => s.Id == author.AuthorId);
                }
                ThisGenres = new ObservableCollection<BookGenreCross>(book.BookGenreCrosses);
                foreach(var genre in ThisGenres)
                {
                    genre.Genre = dbContext.Genres.FirstOrDefault(s=> s.Id == genre.GenreId);
                }

            }

            AddGenre = new CustomCommand(() =>
            {
                BookGenreCross genrecross = new BookGenreCross
                {
                    BookId = AddBookVM.Id,
                    Genre = SelectedGenre,
                    GenreId = SelectedGenre.Id
                };
                ThisGenres.Add(genrecross);
                dbContext.BookGenreCrosses.Add(genrecross);
                SignalChanged(nameof(ThisGenres));
            });

            AddAuthor = new CustomCommand(() =>
            {
                BookAuthorCross bookAuthor = new BookAuthorCross
                {
                    BookId = AddBookVM.Id,
                    Author = SelectedAuthor,
                    FullName = SelectedAuthor.LastName + SelectedAuthor.FirstName + SelectedAuthor.Patronimyc,
                    AuthorId = SelectedAuthor.Id
                };
                ThisAuthors.Add(bookAuthor);
                dbContext.BookAuthorCrosses.Add(bookAuthor);
                SignalChanged(nameof(ThisAuthors));
            });

            SaveBook = new CustomCommand(() =>
            {
                AddBookVM.BookAuthorCrosses = ThisAuthors.ToList();
                AddBookVM.BookGenreCrosses = ThisGenres.ToList();
                AddBookVM.Departament = SelectedDepartament;
                AddBookVM.DepartamentId = SelectedDepartament.Id;
                AddBookVM.Publisher = SelectedPublisher;
                AddBookVM.PublisherId = SelectedPublisher.Id;
                AddBookVM.BookAuthor = book.BookAuthor;
                AddBookVM.BookGenre = book.BookGenre;
                AddBookVM.BookShelf = book.BookShelf;

                if(AddBookVM.Id == 0)
                {
                    dbContext.Books.Add(AddBookVM);
                }
                else
                {
                    dbContext.Entry(book).CurrentValues.SetValues(AddBookVM);
                    book.BookAuthorCrosses = AddBookVM.BookAuthorCrosses;
                    book.BookGenreCrosses = AddBookVM.BookGenreCrosses;
                }

                dbContext.SaveChanges();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                        CloseModalWindow(window);
                }
            });

            Cancel = new CustomCommand(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                        CloseModalWindow(window);
                }
            });
        }

        public void GetInfo(Book book)
        {
            book.BookGenreCrosses = dbContext.BookGenreCrosses.Where(s => s.BookId == book.Id).ToList();
            book.BookAuthorCrosses = dbContext.BookAuthorCrosses.Where(s => s.BookId == book.Id).ToList();
        }

        private void CloseModalWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
