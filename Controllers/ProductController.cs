using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OrderSystem.Services.Shared.Models;
using OrderSystem.Services;
using System.Threading.Tasks;
using OrderSystem.Services.Services;

namespace OrderSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableService _tableService;
        private readonly BlobService _blobService;

        // Initializes the controller with TableService and BlobService
        public ProductController(TableService tableService, BlobService blobService)
        {
            _tableService = tableService;
            _blobService = blobService;
        }

        // Displays a list of all products
        public async Task<IActionResult> Index()
        {
            var products = await _tableService.GetAllProductsAsync();
            return View(products); // Displays a list of all products.
        }

        // Displays details of a single product by ID
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var product = await _tableService.GetProductAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Shows the create product form
        public IActionResult Create()
        {
            return View();
        }

        // Handles creating a new product with optional image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile productImage)
        {
            // Upload product image if provided
            if (productImage != null && productImage.Length > 0)
            {
                using var ms = new MemoryStream();
                await productImage.CopyToAsync(ms);

                product.ProductImage = await _blobService.UploadFileAsync(
                    productImage.FileName,
                    ms.ToArray(),
                    "product-images"
                );

            }

            // Add product to Table Storage
            await _tableService.AddProductAsync(product);

            return RedirectToAction(nameof(Index));
        }

        // Shows the edit product form
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var product = await _tableService.GetProductAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Handles updating a product with optional new image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile productImage)
        {
            if (id != product.RowKey)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Upload new image if selected
                if (productImage != null && productImage.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await productImage.CopyToAsync(ms);

                    product.ProductImage = await _blobService.UploadFileAsync(
                        productImage.FileName,
                        ms.ToArray(),
                        "product-images"
                    );

                }

                // Update product in Table Storage (ETag.All avoids ETag issues)
                await _tableService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            // Return view if ModelState invalid
            return View(product);
        }

        // Shows the delete confirmation page for a product
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var product = await _tableService.GetProductAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Handles deletion of a product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _tableService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
