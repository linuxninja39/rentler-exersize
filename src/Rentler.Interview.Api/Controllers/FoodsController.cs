using System.Drawing;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentler.Interview.Api.Configuration;
using Rentler.Interview.Api.Dtos;
using Rentler.Interview.Api.Entities;

namespace Rentler.Interview.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FoodsController : ControllerBase
{
    private readonly ILogger<FoodsController> _logger;
    private readonly DbSet<Foods> _foodDbSet;
    private readonly FoodContext _foodsContext;

    public FoodsController(ILogger<FoodsController> logger, FoodContext foodContext, FoodContext foodsContext)
    {
        _logger = logger;
        _foodsContext = foodsContext;
        _foodDbSet = foodContext.Foods;
    }

    [HttpGet]
    public PagedResponseDto<FoodsPreview> Get([FromQuery] int page = 0, [FromQuery] int size = 10)
    {
        var q = (Foods f) => new FoodsPreview { Brand = f.Brand, FoodId = f.FoodId, Calories = f.Calories };
        var total = _foodDbSet
            .Select(q)
            .Count();
        var content = _foodDbSet
            .Select(q)
            .Skip(page)
            .Take(size);

        return new PagedResponseDto<FoodsPreview>
        {
            TotalRecords = total,
            Size = size,
            Page = page,
            Results = content
        };
    }

    [HttpGet("Search")]
    public PagedResponseDto<Foods> GetSearch(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 0,
        [FromQuery] int size = 10
    )
    {
        var queryFunc = (Foods f) =>
            f.Description != null && f.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        var count = _foodDbSet
            .Where(queryFunc)
            .Count();
        var results = _foodDbSet
            .Where(queryFunc)
            .Take(size)
            .Skip(page).ToList();

        return new PagedResponseDto<Foods>
        {
            TotalRecords = count,
            Size = results.Count,
            Page = page,
            Results = results
        };
    }


    [HttpGet("{id}")]
    public IActionResult GetOne(int id)
    {
        var food = _foodDbSet.Find(id);
        return food == null ? NotFound() : Ok(food);
    }

    [HttpPost]
    public Foods CreateFood(Foods foods)
    {
        _foodDbSet.Add(foods);
        _foodsContext.SaveChanges();
        return foods;
    }


    [HttpPut]
    public IActionResult UpdateFood(Foods foods)
    {
        var dbFood = _foodDbSet.Find(foods.FoodId);
        if (dbFood == null) return NotFound();
        foreach (var prop in typeof(Foods).GetProperties())
        {
            var source = prop.GetValue(foods);
            var target = prop.GetValue(dbFood);
            var newVal = source ?? target;
            prop.SetValue(dbFood, newVal);
        }

        var r = _foodDbSet.Update(dbFood);

        return Ok(r.Entity);
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteFood(int id)
    {
        var food = _foodDbSet.Find(id);
        if (food == null) return NotFound();
        _foodDbSet.Remove(food);
        _foodsContext.SaveChanges();
        return Ok();
    }
}