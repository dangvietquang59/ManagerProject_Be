using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskItem
{
    [Key]
    public int TaskID { get; set; }

    [Required]
    public int ProjectID { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public string Description { get; set; }

    public int StatusID { get; set; }
    public int PriorityID { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ForeignKey("ProjectID")]
    public required Project Project { get; set; }

    [ForeignKey("StatusID")]
    public Status Status { get; set; }

    [ForeignKey("PriorityID")]
    public Priority Priority { get; set; }
}
