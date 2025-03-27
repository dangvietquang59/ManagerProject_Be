using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FileItem
{
    [Key]
    public int FileID { get; set; }

    [Required]
    public int TaskID { get; set; }

    [Required]
    public int ProjectID { get; set; }

    [Required]
    public int UploadedBy { get; set; }

    [Required]
    [StringLength(255)]
    public string FilePath { get; set; }

    [Required]
    [StringLength(50)]
    public string FileType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    [ForeignKey("TaskID")]
    public TaskItem TaskItem { get; set; }

    [ForeignKey("ProjectID")]
    public Project Project { get; set; }

    [ForeignKey("UploadedBy")]
    public User User { get; set; }
}
