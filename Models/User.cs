using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public int UserID { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }  // ✅ Cho phép NULL

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }  // ✅ Cho phép NULL

    [StringLength(255)]
    public string? AvatarUrl { get; set; }  // ✅ Cho phép NULL

    [Required]
    public int RoleID { get; set; }

    [ForeignKey("RoleID")]
    public Role Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
