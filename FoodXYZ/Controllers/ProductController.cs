using FoodXYZ.Contracts.Products;
using FoodXYZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodXYZ.Controllers
{
    [ApiController]
    [Route("api/product")]

    public class ProductController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;

        public ProductController(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetProduct()
        {
            var product = dataContext.Products.ToList();

            return Ok(new
            {
                data = product
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductsRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Rating = request.Rating
            };
            string fileName = "";
            if (request.Image == null)
            {
                return BadRequest(new
                {
                    message = "Gambar tidak boleh kosong"
                });
            }
            fileName = Guid.NewGuid() + "-" + request.Image.FileName;
            string directory = Path.Combine(Directory.GetCurrentDirectory(), configuration.GetSection("AppSettings:UploadFolder").Value);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            request.Image.CopyTo(new FileStream(Path.Combine(directory, fileName), FileMode.Create));
            product.Image = fileName;


            dataContext.Products.Add(product);
            await dataContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Produk Berhasil Ditambah.",
                produk = product
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductsRequest request, int id)
        {
            var product = await dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new
                {
                    message = "Produk tidak ditemukan"
                });
            }

            product.Name = request.Name;
            product.Price = request.Price;
            product.Rating = request.Rating;

            if (request.Image != null)
            {
                string directory = Path.Combine(Directory.GetCurrentDirectory(), configuration.GetSection("AppSettings:UploadFolder").Value);

                if (System.IO.File.Exists(Path.Combine(directory, product.Image)))
                {
                    System.IO.File.Delete(Path.Combine(directory, product.Image));
                }

                string fileName = Guid.NewGuid() + "-" + request.Image.FileName;
                request.Image.CopyTo(new FileStream(Path.Combine(directory, fileName), FileMode.Create));
                product.Image = fileName;
            }
            dataContext.Products.Update(product);
            await dataContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Produk berhasil diubah",
                data = product
            });

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product tidak ada."
                });
            }

            string directory = Path.Combine(Directory.GetCurrentDirectory(), configuration.GetSection("AppSettings:UploadFolder").Value);
            if (System.IO.File.Exists(Path.Combine(directory, product.Image)))
            {
                System.IO.File.Delete(Path.Combine(directory, product.Image));
            }

            dataContext.Remove(product);
            await dataContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Produk Berhasil Dihapus."
            });
        }
    }
}