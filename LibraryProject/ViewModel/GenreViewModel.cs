using LibraryProject.Core;
using LibraryProject.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.ViewModel
{
    public class GenreViewModel : BaseViewModel
    {
        public Library_DBContext dbContext = new Library_DBContext();

        public List<Genre> Genres { get; set; }
        public GenreViewModel()
        {
            Genres = dbContext.Genres.ToList();
        }
    }
}
