using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BVS;
public class Log
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LogID { get; set; }
    public string? Time { get; set; } 
    public string? Code { get; set; } 
    public string? Description { get; set;} 
    public string? DBObjectID { get; set; }
}