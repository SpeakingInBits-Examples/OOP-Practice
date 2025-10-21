using System.ComponentModel.DataAnnotations;

namespace OOP_Practice.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
}
