using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskAssignment
{
    [Key]
    public int AssignmentID { get; set; }

    [Required]
    public int TaskID { get; set; }

    [Required]
    public int UserID { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.Now;

    [ForeignKey("TaskID")]
    public TaskItem TaskItem { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }
}
