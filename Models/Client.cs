using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebMVCapp1.Models;

public class Client:Person
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClientId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public Cart? CartofCient { get; set; }
    
}