using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProjectMember
{
    [Key]
    public int MemberID { get; set; }

    [Required]
    public int ProjectID { get; set; }

    [Required]
    public int UserID { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = "Member";

    public DateTime JoinedAt { get; set; } = DateTime.Now;

    [ForeignKey("ProjectID")]
    public Project Project { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }
}
