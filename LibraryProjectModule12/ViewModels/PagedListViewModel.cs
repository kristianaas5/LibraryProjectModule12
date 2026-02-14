namespace LibraryProjectModule12.ViewModels
{
    /// <summary>
    /// PagedListViewModel is a generic view model class that represents a paginated list of items. It contains properties for the items to be displayed, the current page number, the page size, and the total number of items in the list. The class also includes calculated properties for the total number of pages, whether there is a previous page, and whether there is a next page. This view model can be used in scenarios where you want to display a large list of items in a paginated format, allowing users to navigate through the pages of results easily.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedListViewModel<T>
    {
        /// The Items property represents the collection of items to be displayed on the current page. It is initialized to an empty enumerable to avoid null reference issues and can be set to a list of items when creating an instance of the PagedListViewModel.
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}