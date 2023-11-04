using System.ComponentModel.DataAnnotations;


public class CsvFileModel
{
    [Key]
    public int CsvFileModelID { get; set; }
    public string FileName { get; set; }

    public byte[] FileData { get; set; }
}