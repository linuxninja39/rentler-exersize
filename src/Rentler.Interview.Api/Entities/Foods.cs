using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentler.Interview.Api.Entities;

public class Foods
{
    [Key]
    public int FoodId { get; set; }

    public string Brand { get; set; }
    public string? Description { get; set; }
   
    public int? ServingSize { get; set; }
    public int? Calories { get; set; }
    public int? Fat { get; set; }
    public int? Carbohydrates { get; set; }
    public int? Protein { get; set; }
    public int? Sodium { get; set; }
    public int? Potassium { get; set; }
    public int? Cholesterol { get; set; }
}