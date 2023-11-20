using Microsoft.EntityFrameworkCore;

namespace FoodXYZ.Models;

public class DataContext :DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<DetailInvoice> DetailInvoices { get; set; }
}