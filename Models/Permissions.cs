using System.ComponentModel.DataAnnotations;

public class Permission
{
    [Key]
    public int PermissionID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }
}
