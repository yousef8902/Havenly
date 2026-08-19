using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Havenly.DAL.Enums;

namespace Havenly.DAL.Entities;

public class Bed
{
    [Key]
    public long BedID { get; set; }

    public long BedroomID { get; set; }
    [ForeignKey(nameof(BedroomID))]
    public Bedroom Bedroom { get; set; }

    [Required]
    public BedType BedType { get; set; }

    public int Quantity { get; set; }
}