namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// ErrorViewModel class is used to represent error information in the application. It contains a RequestId property to track the specific request that caused the error, and a ShowRequestId property to determine whether to display the RequestId in the error view.
    /// </summary>
    public class ErrorViewModel
    {
        // The unique identifier for the request that caused the error. This property is optional and can be used for tracking and debugging purposes.
        public string? RequestId { get; set; }
        // A boolean property that indicates whether to display the RequestId in the error view. It returns true if the RequestId is not null or empty, allowing developers to easily identify and troubleshoot errors based on the request information.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
