using LibraryProjectModule12.Models;
using System.Collections.Generic;

namespace LibraryProjectModule12.ViewModels
{
    public class LibraryShelvesViewModel
    {
        public IList<Library> WantToRead { get; set; } = new List<Library>();
        public IList<Library> CurrentlyReading { get; set; } = new List<Library>();
        public IList<Library> ReadReviewed { get; set; } = new List<Library>();
        public IList<Library> ReadNotReviewed { get; set; } = new List<Library>();
    }
}