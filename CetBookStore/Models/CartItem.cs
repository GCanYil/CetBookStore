using System.ComponentModel.DataAnnotations;

namespace CetBookStore.Models;

public class CartItem
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    public int BookId { get; set; }
    
    [Range(1,100)]
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }
    
    public virtual Book? Book { get; set; }
}