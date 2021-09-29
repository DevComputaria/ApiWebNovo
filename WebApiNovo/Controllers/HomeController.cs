using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApiNovo.Data;
using WebApiNovo.Models;

namespace WebApiNovo.Controllers
{
    [Route("v1")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var employee = new User { Id = 7, UserName = "Robin", Password = "robin", Role = "employeee" };
            var manager = new User { Id = 8, UserName = "Batman", Password = "batman", Role = "manager" };
            var category = new Category { Id = 3, Title = "Informática" };
            var product = new Product { Id = 3, Category = category, Title = "Mouse", Price = 299, Description = "Mouse Game" };
            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dados configurados"

            });

        }
    }
}
