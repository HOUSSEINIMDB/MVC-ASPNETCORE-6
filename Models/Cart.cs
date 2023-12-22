using System.ComponentModel.DataAnnotations;

namespace WebMVCapp1.Models;

public class Cart
{
    [Key]
    public int CartId { get; set;}
    public int? ClientId { get; set; }
    public ICollection<GameItem> CartItems { get; set; } = new List<GameItem>();
}