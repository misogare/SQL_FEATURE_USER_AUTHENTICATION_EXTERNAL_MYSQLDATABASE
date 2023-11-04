using System.ComponentModel.DataAnnotations;

public class ERDData
{
    [Key]
    public int Id { get; set; }
    public string TableName { get; set; }
    [Required]
    public int FileId { get; set; } // Add this line
    [Required]
    public string UserId { get; set; } // Add this line

    public List<Element> Elements { get; set; }
    // Add a method to convert the Elements string to a list of strings

}