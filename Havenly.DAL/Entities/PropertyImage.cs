using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class PropertyImage
{
    [Key]
    public long ImageID { get; set; }

    public long PropertyID { get; set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; set; }

    [Required]
    public string ImagePath { get; set; }
}