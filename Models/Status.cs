using System.ComponentModel.DataAnnotations;

public class Status
{
    [Key]
    public int StatusID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}
