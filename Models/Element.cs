using System.ComponentModel.DataAnnotations;

public class Element
{

    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public int ERDDataId { get; set; }
    public ERDData ERDData { get; set; }

}