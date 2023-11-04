
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Linq;
using System.Security.Claims;

namespace App.Controllers
{
    public class ERDDatasController : Controller
    {
        private readonly appContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ERDDatasController(appContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ERDDatas
        public async Task<IActionResult> Index(int? fileId)

        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (fileId.HasValue)
            {
                // Check if the file belongs to the current user
                var file = await _context.CsvFileModel.FindAsync(fileId);
                if (file == null || file.UserId != userId)
                {
                    return NotFound();
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
                return NotFound();
            }
            var csvFileModel = await _context.CsvFileModel.FindAsync(fileId);
            var csvData = ConvertCsvFileToString(csvFileModel.FileData);
            string[] titles = csvData.Split('\n')[0].Split(',');
            ViewBag.Titles = titles;

            // Remove existing ERDData for the current file
            var existingERDData = _context.ERDData.Where(e => e.FileId == fileId);

            if (!existingERDData.Any())
            {
                // Add titles to ERDData as table names
                foreach (var title in titles)
                {
                    var erdData = new ERDData { TableName = title, FileId = fileId.Value, UserId = userId };
                    _context.ERDData.Add(erdData);
                }
                await _context.SaveChangesAsync();
            }

            List<ERDData> erdDataList = await _context.ERDData.Include(e => e.Elements).Where(e => e.FileId == fileId && e.UserId == userId).ToListAsync();

            // Create the search view model
            var viewModel = new SearchViewModel
            {
                Results = erdDataList
            };

            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Index(SearchViewModel viewModel)
        {
            List<ERDData> erdDataList = await _context.ERDData.Include(e => e.Elements).ToListAsync();
            string query = viewModel.Query;
            if (string.IsNullOrWhiteSpace(query) || query.Equals("select *", StringComparison.OrdinalIgnoreCase))
            {
                // Return all ERD data
                return RedirectToAction(nameof(Index));
            }
            else if (query.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase))
            {
                // Extract the table name from the query

                List<ERDData> searchResults = SearchERDData(query);

                // Update the view model with the search results
                viewModel.Results = searchResults;
            }


            else
            {

                var searchResults = _context.Database.ExecuteSqlRaw(query);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            return View(viewModel);


        }
        private List<ERDData> SearchERDData(string query)
        {
            // Perform the search logic based on the query
            // You can implement your custom search logic here based on the query

            // if (string.IsNullOrWhiteSpace(query) || query.Equals("select *", StringComparison.OrdinalIgnoreCase))
            // {
            // Return all ERD data
            if (query.StartsWith("SELECT ERDData.Id, ERDData.TableName, Elements.name ", StringComparison.OrdinalIgnoreCase))
            {
                // Split the query to extract the table name and element name
                string[] parts = query.Split(new[] { "WHERE ERDData.TableName = '", "' AND Elements.name = '", "'" }, StringSplitOptions.RemoveEmptyEntries);
                string tableName = parts[1];
                string elementName = parts[2];

                if (elementName.Split(",").Length > 1)
                {
                    string[] wew = elementName.Split(',');

                    var searchResults1 = _context.ERDData
        .Where(d => d.TableName == tableName)
        .FirstOrDefault();
                    if (searchResults1 != null)
                    {
                        // Add the specified elements to the search results
                        searchResults1.Elements = searchResults1.Elements
                            .Where(e => wew.Contains(e.Name))
                            .ToList();
                    }

                    return new List<ERDData> { searchResults1 };
                }

                // Perform the search based on the extracted table name and element name
                var searchResults = _context.ERDData
                    .Where(d => d.TableName == tableName)
                    .Select(d => new ERDData
                    {
                        Id = d.Id,
                        TableName = d.TableName,
                        Elements = d.Elements.Where(e => e.Name == elementName).ToList()
                    })
                    .ToList();
                return searchResults;


            }



            //  }
            else
            {
                var searchResults = _context.ERDData.FromSqlRaw(query);
                // Perform the search logic based on the query
                // Assuming the query is a table name to search for


                return searchResults.ToList();

            }


        }
        private string ConvertCsvFileToString(byte[] fileData)
        {
            // Convert byte array to CSV string
            using (var memoryStream = new MemoryStream(fileData))
            using (var reader = new StreamReader(memoryStream))
            {
                return reader.ReadToEnd();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ClearLog()
        {
            var fileId = HttpContext.Session.GetInt32("FileId");
            if (!fileId.HasValue)
            {
                return NotFound();
            }
            // Retrieve the fileId from session
            fileId = HttpContext.Session.GetInt32("FileId");

            // Remove existing ERDData for the current file
            var existingERDData = _context.ERDData.Where(e => e.FileId == fileId );
            _context.ERDData.RemoveRange(existingERDData);
            await _context.SaveChangesAsync();

            // Redirect to the Index action to reload the original data
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Save()
        {
            // Retrieve the fileId from session
            var fileId = HttpContext.Session.GetInt32("FileId");
            if (!fileId.HasValue)
            {
                return NotFound();
            }

            // Get the ERDData for the current file
            var erdDataList = await _context.ERDData.Include(e => e.Elements).Where(e => e.FileId == fileId).ToListAsync();

            // Convert the ERDData to a CSV string
            var csvData = ConvertERDDataToCsv(erdDataList);

            // Get the CsvFileModel for the current file
            var csvFileModel = await _context.CsvFileModel.FindAsync(fileId);

            // Replace the FileData in the CsvFileModel with the new CSV data
            csvFileModel.FileData = ConvertCsvStringToFile(csvData);

            // Save the changes to the database
            _context.Update(csvFileModel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private string ConvertERDDataToCsv(List<ERDData> erdDataList)
        {
            var csv = new StringBuilder();

            var elementsByTable = new Dictionary<string, List<string>>();

            foreach (var erdData in erdDataList)
            {
                string tableName = erdData.TableName.Trim();
                if (!elementsByTable.ContainsKey(tableName))
                {
                    elementsByTable[tableName] = new List<string>();
                }

                // Add the elements of the current ERDData under the corresponding table name
                elementsByTable[tableName].AddRange(erdData.Elements.Select(element => element.Name.Trim()));
            }

            // Write the table names to the CSV
            csv.Append(string.Join(",", elementsByTable.Keys));
            csv.Append(" \n");
            int maxElementCount = elementsByTable.Values.Max(list => list.Count);
            for (int i = 0; i < maxElementCount; i++)
            {
                foreach (var tableName in elementsByTable.Keys)
                {
                    if (i < elementsByTable[tableName].Count)
                    {
                        csv.Append(elementsByTable[tableName][i]);
                    }
                    csv.Append(",");
                }
                csv.Append(" \n");
            }

            return csv.ToString();
        }

        private byte[] ConvertCsvStringToFile(string csvData)
        {
            return Encoding.UTF8.GetBytes(csvData);
        }
        // GET: ERDDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ERDData == null)
            {
                return NotFound();
            }

            var eRDData = await _context.ERDData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eRDData == null)
            {
                return NotFound();
            }

            return View(eRDData);
        }


    }
}
