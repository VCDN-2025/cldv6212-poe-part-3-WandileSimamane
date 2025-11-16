using Microsoft.AspNetCore.Mvc;
using OrderSystem.Services.Shared.Models;
using OrderSystem.Services.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Azure;

namespace OrderSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableService _tableService;
        private readonly BlobService _blobService;

        public ProductController(TableService tableService, BlobService blobService)
        {
            _tableService = tableService;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Shop()
        {
            var products = await _tableService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? productImage)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();

            if (string.IsNullOrWhiteSpace(product.ProductName))
                ModelState.AddModelError("ProductName", "Name is required.");
            if (product.Price <= 0)
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            if (product.Stock < 0)
                ModelState.AddModelError("Stock", "Stock cannot be negative.");

            if (!ModelState.IsValid) return View(product);

            try
            {
                if (productImage?.Length > 0) 
                {
                    using var ms = new MemoryStream();
                    await productImage.CopyToAsync(ms);
                    var fileName = Guid.NewGuid() + Path.GetExtension(productImage.FileName);
                    product.ProductImage = await _blobService.UploadFileAsync(fileName, ms.ToArray());
                }
                else
                {
                    product.ProductImage = "";
                }

                product.PartitionKey = "Product";
                product.RowKey = Guid.NewGuid().ToString();

                await _tableService.AddProductAsync(product);

                TempData["Success"] = "Product created!";
                return RedirectToAction(nameof(Index));
            }
            catch (RequestFailedException ex)
            {
                ModelState.AddModelError("", $"Azure error: {ex.Message}");
                return View(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();
            if (string.IsNullOrEmpty(id)) return NotFound();
            var product = await _tableService.GetProductAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile? productImage)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();
            if (id != product.RowKey) return NotFound();

            if (!ModelState.IsValid) return View(product);

            try
            {
                if (productImage?.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await productImage.CopyToAsync(ms);
                    var fileName = Guid.NewGuid() + Path.GetExtension(productImage.FileName);
                    product.ProductImage = await _blobService.UploadFileAsync(fileName, ms.ToArray());
                }

                await _tableService.UpdateProductAsync(product);
                TempData["Success"] = "Product updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(product);
            }
        }



[HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var product = await _tableService.GetProductAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }
        // GET: /Product/Details/{id} – Public (customer view)
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var product = await _tableService.GetProductAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: /Product/Details/{id} – Add to Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string id, int quantity = 1)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id) || quantity <= 0) return NotFound();

            var product = await _tableService.GetProductAsync(id);
            if (product == null || product.Stock < quantity)
            {
                TempData["Error"] = "Product not available or out of stock.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var userId = HttpContext.Session.GetString("UserId")!;
            var cartItem = new CartItem
            {
                PartitionKey = userId,
                RowKey = Guid.NewGuid().ToString(),
                ProductId = id,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = quantity,
                ProductImage = product.ProductImage
            };

            await _tableService.AddCartItemAsync(cartItem);

            TempData["Success"] = $"Added {quantity} × {product.ProductName} to cart.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Unauthorized();
            await _tableService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}