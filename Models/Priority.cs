using System.ComponentModel.DataAnnotations;

public class Priority
{
    [Key]
    public int PriorityID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}
