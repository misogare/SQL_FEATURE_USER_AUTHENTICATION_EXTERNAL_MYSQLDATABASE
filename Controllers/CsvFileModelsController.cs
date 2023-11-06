using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Controllers
{
    public class CsvFileModelsController : Controller
    {
        private readonly appContext _context;
        private readonly CsvFileviewModel _viewModel;
        public CsvFileModelsController(appContext context, CsvFileviewModel viewmodel)
        {
            _context = context;
            _viewModel = viewmodel;
        }

        // GET: CsvFileModels
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            IQueryable<CsvFileModel> files;

            if (userRole == "Admin")
            {
                files = _context.CsvFileModel;

            }
            else
            {
                files = _context.CsvFileModel.Where(f => f.UserId == userId);


            }
            return View(await files.ToListAsync());
        }

        // GET: CsvFileModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CsvFileModel == null)
            {
                return NotFound();
            }

            var csvFileModel = await _context.CsvFileModel
                .FirstOrDefaultAsync(m => m.CsvFileModelID == id);
            if (csvFileModel == null)
            {
                return NotFound();
            }

            return View(csvFileModel);
        }



        // GET: CsvFileModels/Create
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CsvFileviewModel();
            return View(viewModel);
        }
        // POST: CsvFileModels/Create
        [HttpPost]
        public async Task<IActionResult> Create(CsvFileviewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.File != null && viewModel.File.Length > 0)
                {
                    // Read the file content into a byte array
                    byte[] fileData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await viewModel.File.CopyToAsync(memoryStream);
                        fileData = memoryStream.ToArray();

                    }

                    // Create a CsvFileModel instance to store the file data
                    var csvFile = new CsvFileModel
                    {
                        FileName = viewModel.File.FileName,
                        FileData = fileData,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)

                    };

                    // Save the CsvFileModel instance to the database
                    _context.Add(csvFile);
                    await _context.SaveChangesAsync();

                    // Redirect to a success page or return a success response
                    return RedirectToAction(nameof(Index));
                }
            }

            // If there are validation errors or no file was provided, return the view with the model
            return View(viewModel);
        }



        public async Task<IActionResult> Edit(int? id)
        {

            return RedirectToAction("Index", "FileUpload", new { fileId = id });
        }






        // GET: CsvFileModels/SQL/5
        public async Task<IActionResult> SQL(int? id)
        {
            return RedirectToAction("Index", "ERDDatas", new { fileId = id });
        }

        // POST: CsvFileModels/SQL/5
        [HttpPost, ActionName("SQL")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLConfirmed(int id)
        {
            return View();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var csvFileModel = await _context.CsvFileModel
                .FirstOrDefaultAsync(m => m.CsvFileModelID == id);
            if (csvFileModel == null)
            {
                return NotFound();
            }

            return View(csvFileModel);
        }

        // POST: CsvFileModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var csvFileModel = await _context.CsvFileModel.FindAsync(id);
            _context.CsvFileModel.Remove(csvFileModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CsvFileModelExists(int id)
        {
            return (_context.CsvFileModel?.Any(e => e.CsvFileModelID == id)).GetValueOrDefault();
        }
    }
}
