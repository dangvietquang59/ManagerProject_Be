using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
}
