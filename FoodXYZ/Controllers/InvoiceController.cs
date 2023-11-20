using System.Security.Claims;
using FoodXYZ.Contracts.Invoice;
using FoodXYZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodXYZ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;

        public InvoiceController(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddInvoice(AddInvoiceRequest request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int userId = 0;
            if (identity != null)
            {
                userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            else
            {
                return Unauthorized();
            }
            var inv = new Invoice
            {
                InvoiceNum = Guid.NewGuid().ToString(),
                QtyTotal = request.QtyTotal,
                PriceTotal = request.PriceTotal,
                UserId = userId
            };

            dataContext.Invoices.Add(inv);
            await dataContext.SaveChangesAsync();

            List<EntityInvoice> invs = new List<EntityInvoice>(); 
            var price_total = 0;
            var qty_total = 0;

            foreach (ItemEntity item in request.items)
            {
                var product = await dataContext.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    continue;
                }

                var entity = new DetailInvoice
                {
                    InvoiceId = inv.Id,
                    Qty = item.Qty,
                    ProductId = item.ProductId,
                    SubTotal = item.SubTotal
                };

                dataContext.DetailInvoices.Add(entity);
                invs.Add(new EntityInvoice{
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Rating = product.Rating,
                    Image = product.Image,
                    Qty = item.Qty
                });

                price_total += item.Qty * product.Price;
                qty_total += item.Qty; 
            }
            await dataContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Berhasil menyimpan Invoice",
                data_inv = invs,
                price_total = price_total,
                qty_total = qty_total
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetInvoice()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int userId = 0;
            if (identity != null)
            {
                userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            else
            {
                return Unauthorized();
            }
            var inv = await dataContext.Invoices.Where(x => x.UserId == userId).ToListAsync();

            return Ok(new
            {
                data = inv
            });
        }
    }
}