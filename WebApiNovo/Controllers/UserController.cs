using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiNovo.Data;
using WebApiNovo.Models;
using WebApiNovo.Services;

namespace WebApiNovo.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context
                  .Users
                  .AsNoTracking()
                  .ToListAsync();
            return users;

        }


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        // [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
         [FromServices] DataContext context,
         [FromBody] User model)

        // Varifica se os dados são válidos
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //Forçar o usuário a ser sempre "funcionário"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                //Esconder a senha
                model.Password = "";   
                return model;

            }
            catch (Exception e)
            {

                var exception = e;
                return BadRequest(new { message = "Não foi possivel criar o usuário" });


            }


        }

        [HttpPut]
        [Route("{id:int}")]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            int id,
            [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar usuário" });

            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
                     [FromServices] DataContext context,
                     [FromBody] User model)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.UserName == model.UserName && x.Password == model.Password)
                .FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            // Esconder a senha
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };

        }
        [HttpDelete]
        [Route("{id:int}")]
        //[Authorize(Roles = "manager")]

        public async Task<ActionResult<List<User>>> Delete(
            int id,
            [FromServices] DataContext context
        )

        {
            var users = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (users == null)
                return NotFound(new { message = "Usuário não encontrado" });



            try
            {
                context.Users.Remove(users);
                await context.SaveChangesAsync();
                return Ok(new { message = "Usuário removido com sucesso" });


            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover usuário " });

            }

        }


    }
}