namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// ShelfType enumeration represents the different types of shelves that can be used to categorize books in a user's library collection. It includes values for "Want to Read", "Currently Reading", and "Read". This enumeration allows us to standardize the categorization of books on different shelves and provides a clear way to identify the status of each book in the user's library.
    public enum ShelfType
    {
        WantToRead = 0,
        CurrentlyReading = 1,
        Read = 2
    }
}