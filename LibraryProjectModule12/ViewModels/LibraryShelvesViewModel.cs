using LibraryProjectModule12.Models;
using System.Collections.Generic;

namespace LibraryProjectModule12.ViewModels
{
    /// <summary>
    /// LibraryShelvesViewModel is a view model class that represents the data structure for displaying a user's library shelves in the application. It contains four properties, each representing a different shelf type: WantToRead, CurrentlyReading, ReadReviewed, and ReadNotReviewed. Each property is a list of Library objects, which represent the books that belong to each respective shelf. This view model allows us to organize and display the user's library collection based on their reading status and review status.
    /// </summary>
    public class LibraryShelvesViewModel
    {
        //
        public IList<Library> WantToRead { get; set; } = new List<Library>();
        public IList<Library> CurrentlyReading { get; set; } = new List<Library>();
        public IList<Library> ReadReviewed { get; set; } = new List<Library>();
        public IList<Library> ReadNotReviewed { get; set; } = new List<Library>();
    }
}