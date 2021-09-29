﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiNovo.Data;
using WebApiNovo.Models;

namespace WebApiNovo.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var product = await context
                .Products.Include(x => x
                .Category).AsNoTracking()
                .ToListAsync();
            return product;

        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id)
        {
            var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return product;

        }

        [HttpGet] // products/categories/1
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
            return products;

        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model)

        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Products.Add(model);
                    context.SaveChanges();
                    return Ok(model);

                }

                catch (Exception e)
                {
                    var exception = e;
                    return BadRequest(ModelState);
                }

            }
            else
            {
                return BadRequest(ModelState);
            }

        }



        


    }
}