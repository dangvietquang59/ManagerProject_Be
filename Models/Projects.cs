using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Project
{
    [Key]
    public int ProjectID { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    [Required]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
