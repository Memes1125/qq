using LibraryProject.Core;
using LibraryProject.DB;
using LibraryProject.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibraryProject.ViewModel
{
    public class BookViewModel : BaseViewModel
    {
        public Library_DBContext dbContext = new Library_DBContext();

        public List<Book> Books { get; set; }
        public List<BookGenreCross> Genres { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<BookAuthorCross> Authors { get; set; }
        public List<BookShelfCross> ShelfFilter { get; set; }
        public List<Department> DepartmentFilter { get; set; }

        private List<Book> searchResult;

        private Department selectedDepartmentFilter { get; set; }
        public Department SelectedDepartmentFilter
        {
            get => selectedDepartmentFilter;
            set
            {
                selectedDepartmentFilter = value;
                Search();
            }
        }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }

        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Search();
            }
        }

        private Book selectedBook { get; set; }
        public Book SelectedBook
        {
            get => selectedBook;
            set
            {
                selectedBook = value;
                SignalChanged();
            }
        }

        //public string BookGenres { get; set; }

        public void GetBook(Library_DBContext dBContext)
        {
            Books = dBContext.Books.ToList();
            Genres = dBContext.BookGenreCrosses.ToList();
            Authors = dBContext.BookAuthorCrosses.ToList();
            ShelfFilter = dBContext.BookShelfCrosses.ToList();
            Publishers = dBContext.Publishers.ToList();
            DepartmentFilter = dBContext.Departments.ToList();
            foreach (var book in Books)
            {
                book.Publisher = Publishers.FirstOrDefault(s => s.Id == book.PublisherId);
                book.Departament = DepartmentFilter.FirstOrDefault(s => s.Id == book.DepartamentId);
                book.BookGenreCrosses = Genres.Where(s=> s.BookId == book.Id).ToList();
                foreach (var genre in Genres.Where(s => s.BookId == book.Id))
                {
                    genre.Genre = dBContext.Genres.FirstOrDefault(s => s.Id == genre.GenreId);

                    book.BookGenre += $"{genre.Genre.GenreName}";
                }
                foreach (var shelf in ShelfFilter.Where(s => s.BookId == book.Id))
                {
                    shelf.Shelf = dBContext.Shelves.FirstOrDefault(s => s.Id == shelf.ShelfId);
                    book.BookShelf += $"{shelf.ShelfId}";
                }
                foreach (var author in Authors.Where(s => s.BookId == book.Id))
                {
                    author.Author = dBContext.Authors.FirstOrDefault(s => s.Id == author.AuthorId);
                    book.BookAuthor += $"{author.Author.LastName }" + $"{author.Author.FirstName}";
                }
            }
        }

        public CustomCommand EditBook { get; set; }
        public CustomCommand AddBook { get; set; }
        public CustomCommand DeleteBook { get; set; }

        public BookViewModel()
        {
            GetBook(dbContext);

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Название", "Автор", "Цена", "Жанр", "Год публикации", "Номер стеллажа" });
            selectedSearchType = SearchType.First();
            DepartmentFilter.Add(new Department { DepartmentName = "Все типы" });
            selectedDepartmentFilter = DepartmentFilter.Last();

            AddBook = new CustomCommand(() =>
            {
                AddBookWindow addBook = new AddBookWindow(null);
                addBook.ShowDialog();
                searchResult = Books;
                //SignalChanged(nameof(searchResult));
            });

            EditBook = new CustomCommand(() =>
            {
                if (SelectedBook == null || SelectedBook.Id == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Вы не выбрали книгу!", "Ошибка", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                else
                {
                    AddBookWindow addWindow = new AddBookWindow(SelectedBook);
                    addWindow.ShowDialog();
                    searchResult = Books;
                    //SignalChanged(nameof(searchResult));
                }
            });
            searchResult = dbContext.Books.ToList();
            SignalChanged("Books");
        }

        public void Search()
        {
            var search = SearchText.ToLower();
            if(search == "" || search == null)
            {
                MessageBox.Show("Error");
            }
            if(SelectedDepartmentFilter.DepartmentName == "Все типы")
            {
                if(SelectedSearchType == "Название")
                {
                    searchResult = dbContext.Books.Where(s => s.BookName.ToLower().Contains(search)).ToList();
                    Books = searchResult;
                    SignalChanged(nameof(Books));
                }
                else if(SelectedSearchType == "Автор")
                {
                    searchResult = dbContext.Books.Where(s=> s.BookAuthor.ToLower().Contains(search)).ToList();
                    Books = searchResult;
                    SignalChanged(nameof(Books));
                }
                else if(SelectedSearchType == "Цена")
                {
                    searchResult = dbContext.Books.Where(s => s.Price.ToString().ToLower().Contains(search)).ToList();
                    Books = searchResult;
                    SignalChanged(nameof(Books));
                }
                else if (SelectedSearchType == "Жанр")
                {
                    searchResult = dbContext.Books.Where(s => s.BookGenre.ToLower().Contains(search)).ToList();
                    Books = searchResult;
                    SignalChanged(nameof(Books));
                }
                else if (SelectedSearchType == "Год публикации")
                {
                    searchResult = dbContext.Books.Where(s => s.YearPublished.Value.ToShortDateString().ToLower().Contains(search)).ToList();
                    Books = searchResult;
                    SignalChanged(nameof(Books));
                }
                else if(SelectedSearchType == "Номер стеллажа")
                {
                    searchResult = dbContext.Books.Where(s => s.BookShelf.ToLower().Contains(search)).ToList();
                    Books = searchResult;
                    SignalChanged(nameof(Books));
                }
                else if(searchResult == null)
                {
                    MessageBox.Show("Error");
                }
                else
                {
                    if (SelectedSearchType == "Название")
                    {
                        searchResult = dbContext.Books.Where(s => s.BookName.ToLower().Contains(search) && s.Departament.DepartmentName.ToLower().Contains(SelectedDepartmentFilter.DepartmentName)).ToList();
                        Books = searchResult;
                        SignalChanged(nameof(Books));
                    }
                    else if (SelectedSearchType == "Автор")
                    {
                        searchResult = dbContext.Books.Where(s => s.BookAuthor.ToLower().Contains(search) && s.Departament.DepartmentName.ToLower().Contains(SelectedDepartmentFilter.DepartmentName)).ToList();
                        Books = searchResult;
                        SignalChanged(nameof(Books));
                    }
                    else if (SelectedSearchType == "Цена")
                    {
                        searchResult = dbContext.Books.Where(s => s.Price.ToString().ToLower().Contains(search) && s.Departament.DepartmentName.ToLower().Contains(SelectedDepartmentFilter.DepartmentName)).ToList();
                        Books = searchResult;
                        SignalChanged(nameof(Books));
                    }
                    else if (SelectedSearchType == "Жанр")
                    {
                        searchResult = dbContext.Books.Where(s => s.BookGenre.ToLower().Contains(search) && s.Departament.DepartmentName.ToLower().Contains(SelectedDepartmentFilter.DepartmentName)).ToList();
                        Books = searchResult;
                        SignalChanged(nameof(Books));
                    }
                    else if (SelectedSearchType == "Год публикации")
                    {
                        searchResult = dbContext.Books.Where(s => s.YearPublished.Value.ToShortDateString().ToLower().Contains(search)).ToList();
                        Books = searchResult;
                        SignalChanged(nameof(Books));
                    }
                    else if (SelectedSearchType == "Номер стеллажа")
                    {
                        searchResult = dbContext.Books.Where(s => s.BookShelf.ToLower().Contains(search)).ToList();
                        Books = searchResult;
                        SignalChanged(nameof(Books));
                    }
                }
            }
        }

        public void InitPagination()
        {

        }

        public void Pagination()
        {

        }
    }
}
