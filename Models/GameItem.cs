using System.ComponentModel.DataAnnotations;

namespace WebMVCapp1.Models;

public class GameItem
{
    [Key] 
    public String SerialNumber { get; set; }
    public int? GameId { get; set; }
    public int? CartId { get; set; }
}