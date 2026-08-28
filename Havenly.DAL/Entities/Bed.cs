using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Havenly.DAL.Enums;

namespace Havenly.DAL.Entities;

public class Bed
{
    [Key]
    public long BedID { get; private set; }

    public long BedroomID { get; private set; }
    [ForeignKey(nameof(BedroomID))]
    public Bedroom Bedroom { get; private set; }

    [Required]
    public BedType BedType { get; private set; }

    public int Quantity { get; private set; }

    public void Create(long bedId, long bedroomId, BedType bedType, int quantity)
    {
        BedID = bedId;
        BedroomID = bedroomId;
        BedType = bedType;
        Quantity = quantity;
    }

    public void Update(BedType bedType, int quantity)
    {
        BedType = bedType;
        Quantity = quantity;
    }
}
