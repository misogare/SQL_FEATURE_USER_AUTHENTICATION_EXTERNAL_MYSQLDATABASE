using App.Data;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Controllers 
{
    public class FileUploadController : Controller
    {
        private readonly appContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadController(appContext context, IWebHostEnvironment webHostEnvironment)

        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [HttpPost]        /*
Method Name: UploadFile
Description:
    This method handles the file upload action. It receives an 'IFormFile' containing the uploaded file data. If a file is uploaded and its length is greater than 0, it generates a unique file name and saves the file to the 'uploads' folder. It then reads the uploaded CSV file, extracts the fields and their data types using the 'ReadCsvFields' method, and passes the field models to the 'DiagrammingArea' view.

Parameters:
    - file: An 'IFormFile' object representing the uploaded file.

Returns:
    - If the file is uploaded successfully, it returns the 'DiagrammingArea' view with the extracted field models. If no file is uploaded, it redirects to the 'Index' action.
*/


        public async Task<IActionResult> Index(int? fileId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (fileId.HasValue)
            {
               
                var file = await _context.CsvFileModel.FindAsync(fileId);

                if ((file == null || file.UserId != userId) && userRole != "Admin")
                {
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });

                }
                // Store the fileId in session
                HttpContext.Session.SetInt32("FileId", fileId.Value);
            }
            else
            {
                // Retrieve the fileId from session
                fileId = HttpContext.Session.GetInt32("FileId");
            }
            if (!fileId.HasValue)
            {
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }
            var csvFileModel = await _context.CsvFileModel.FindAsync(fileId);



            // Read the CSV file and extract the fields and data types
            var fieldModels = ReadCsvFields(csvFileModel.FileData);

            // Pass the field models to the view
            return View("Index", fieldModels);

        }
        /*
Method Name: ReadCsvFields
Description:
    This method reads field information from a CSV file located at the specified 'filePath'. It assumes that the CSV file has a header row followed by a data row. It parses the header to extract field names and the data row to infer data types for each field. The resulting field information is stored in a list of 'FieldModel' objects.

Parameters:
    - filePath: The file path to the CSV file to be read.

Returns:
    - A list of 'FieldModel' objects representing the fields and their inferred data types from the CSV file.
*/

        private List<FieldModel> ReadCsvFields(byte[] filePath)
        {
            var fields = new List<FieldModel>();
            using (var memoryStream = new MemoryStream(filePath))
            using (var reader = new StreamReader(memoryStream))
            {
                if (!reader.EndOfStream)
                {
                    var headerLine = reader.ReadLine();
                    var headerFields = headerLine.Split(',');

                    if (!reader.EndOfStream)
                    {
                        var dataLine = reader.ReadLine();
                        var dataFields = dataLine.Split(',');

                        for (int i = 0; i < headerFields.Length; i++)
                        {
                            var fieldModel = new FieldModel();
                            fieldModel.FieldName = headerFields[i].Trim();
                            fieldModel.DataType = InferDataType(dataFields[i].Trim());
                            fields.Add(fieldModel);
                        }
                    }

                }
            }

            return fields;
        }

        /*
Method Name: InferDataType
Description:
    This method infers the data type of a given value based on its content. It checks the value against several common data types, such as 'int', 'float', 'date', 'bool', 'decimal', 'long', 'guid', and defaults to 'string' if no match is found.

Parameters:
    - value: The input value for which the data type is to be inferred.

Returns:
    - A string representing the inferred data type.
*/
        private string InferDataType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "string";
            }
            else if (int.TryParse(value, out _))
            {
                return "int";
            }
            else if (double.TryParse(value, out _))
            {
                return "float";
            }
            else if (DateTime.TryParse(value, out _))
            {
                return "date";
            }
            else if (bool.TryParse(value, out _))
            {
                return "bool";
            }
            else if (decimal.TryParse(value, out _))
            {
                return "decimal";
            }
            else if (long.TryParse(value, out _))
            {
                return "long";
            }
            else if (Guid.TryParse(value, out _))
            {
                return "guid";
            }
            else
            {
                return "string";
            }
        }



    }
}
