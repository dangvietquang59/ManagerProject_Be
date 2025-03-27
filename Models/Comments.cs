using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comment
{
    [Key]
    public int CommentID { get; set; }

    [Required]
    public int TaskID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("TaskID")]
    public TaskItem TaskItem { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }
}
