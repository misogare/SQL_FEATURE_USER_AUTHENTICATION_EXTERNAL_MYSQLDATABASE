using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Keyless]
public class CsvFileviewModel
{


    [Required(ErrorMessage = "Please select a CSV file.")]
    [Display(Name = "CSV File")]
    public IFormFile File { get; set; }
}