using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OrderSystem.Controllers
{
    public class FileController : Controller
    {
        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

       // Displays the view for uploading files.
        public IActionResult Index()
        {
            return View();
        }

   
             
        [HttpPost] // Handles the file upload from the user.

        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                await _fileService.UploadFileAsync(
                    file.FileName,
                    ms.ToArray(),
                    "contracts"
                );

                ViewBag.Message = "File uploaded successfully!";
            }
            else
            {
                ViewBag.Message = "Please select a file to upload.";
            }

            return View("Index");
        }
    }
}