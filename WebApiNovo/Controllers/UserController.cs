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
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Get([FromRoute] int id, [FromServices] DataContext context)
        {
            var user = context
                  .Users
                  .AsNoTracking()
                  .Where(u => u.Id == id)
                  .FirstOrDefault();

            return user;
        }

        [HttpGet("All")]
        [AllowAnonymous]
        public async Task<ActionResult> Get([FromServices] DataContext context)
        {
            var users = context
                  .Users
                  .AsNoTracking()
                  .ToList();

            return Ok(users);
        }

        // Para criar usuário manager
        //[HttpPost]
        //[Route("manager")]
        ////[AllowAnonymous]
        //[Authorize(Roles = "manager")]
        //public async Task<ActionResult<User>> Post(
        // [FromServices] DataContext context,
        // [FromBody] User model)
        //{
        //    // Verifica se o dados são válidos
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        context.Users.Add(model);
        //        await context.SaveChangesAsync();
        //        return model;

        //    }
        //    catch (Exception e)
        //    {

        //        var exception = e;
        //        return BadRequest(new { message = "Não foi possivel criar o usuário" });


        //    }


        //}


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
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
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);


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
            // Varifica se os dados são válidos
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