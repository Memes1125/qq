using LibraryProject.Core;
using LibraryProject.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryProject.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainWindow mwindow;
        public CustomCommand BookPageCommand { get; set; }
        public CustomCommand AuthorPageCommand { get; set; }
        public CustomCommand ShelfPageCommand { get; set; }
        public CustomCommand GenrePageCommand { get; set; }

        private Page _currentPage;

        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; SignalChanged("CurrentPage"); }
        }

        public MainViewModel()
        {

            CurrentPage = new BookPage();

            BookPageCommand = new CustomCommand(() => CurrentPage = new BookPage());
            GenrePageCommand = new CustomCommand(() => CurrentPage = new GenrePage());
        }
    }
}
