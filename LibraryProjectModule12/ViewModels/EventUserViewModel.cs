namespace LibraryProjectModule12.ViewModels;

using System.ComponentModel.DataAnnotations;
/// <summary>
/// ViewModel за поръчка на билети
/// Използва се в /Events/Order/{id}
/// </summary>
public class EventUserViewModel
{
    /// <summary>
    /// Id на събитието за което поръчваме
    /// Hidden field във формата
    /// </summary>
    public Guid Id { get; set; }
    public int eventId { get; set; }

    /// <summary>
    /// Име на събитието (само за показване)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Брой билети за поръчка
    /// </summary>
}