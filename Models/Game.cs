using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMVCapp1.Models;

public class Game
{
    [Key] 
    [DatabaseGenerated((DatabaseGeneratedOption.Identity))]
    public int GameId { get; set; }
    [Required] public String GameTitle { get; set; }
    [Required] public String ImageUrl { get; set; }
    [Required] public String GameDescription{ get; set; }
    public ICollection<GameItem> GameItems { get;} = new List<GameItem>();
}
