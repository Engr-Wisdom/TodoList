using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.Api.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [Required(ErrorMessage = "A title is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "The title must be between 3 and 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "A summary is required.")]
    [StringLength(500,
        ErrorMessage = "The summary cannot exceed 500 characters.")]
    public string Summary { get; set; } = string.Empty;

    [Range(1, 4, ErrorMessage = "Please select a valid category.")]
    public TaskCategory Category { get; set; }

    [Required(ErrorMessage = "A due date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Due Date")]
    public DateOnly? DueDate { get; set; }

    [Range(1, 10,
        ErrorMessage = "Priority must be between 1 and 10.")]
    public int Priority { get; set; }

    [Display(Name = "Completed")]
    public bool IsCompleted { get; set; }
}