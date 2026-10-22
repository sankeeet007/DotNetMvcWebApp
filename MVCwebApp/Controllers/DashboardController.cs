using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCwebApp.Data;
using MVCwebApp.Dto;

namespace MVCwebApp.Controllers
{
    [Authorize]
    public class DashboardController(AppDbContext context) : Controller
    {
        public IActionResult Index()
        {
            // Projection using LINQ to select specific properties and map them to ProductDto
            var list = context.Products.Select(x => new ProductDto
                       {
                           Id = x.Id,
                           ProductName = x.ProductName,
                           Description = x.Description,
                           Price = x.Price,
                           Color = x.Color
                       })
                       .ToList();
            return View(list);
        }
        public IActionResult ProductForm() => View();

        public IActionResult UpdateProductForm(int id)
        {
            var data = context.Products.Select(x => new ProductDto
                                {
                                    Id = x.Id,
                                    ProductName = x.ProductName,
                                    Description = x.Description,
                                    Color = x.Color,
                                    Price = x.Price
                                })
                            .FirstOrDefault(x => x.Id == id);
            return View(data);
        } 

        
        public async Task<IActionResult> CreateProduct(ProductDto dto)
        {
            if (dto == null)
            {
                ViewBag.ErrorMessage = "Kindly Fill All the details!!!";
                return View("ProductForm");
            }
            context.Products.Add(new Models.Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Color = dto.Color
            });
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProduct(int productid)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == productid);
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateProduct(ProductDto dto)
        {
            if (dto == null)
            {
                ViewBag.ErrorMessage = "Kindly Fill All the details!!!";
                return View("UpdateProductForm");
            }

            var data = context.Products.FirstOrDefault(x => x.Id == dto.Id);
            data.ProductName = dto.ProductName;
            data.Description = dto.Description;
            data.Color = dto.Color;
            data.Price = dto.Price;

            context.Products.Update(data);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
