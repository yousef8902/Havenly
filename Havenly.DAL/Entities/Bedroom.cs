using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Bedroom
{
    [Key]
    public long BedroomID { get; set; }

    public long PropertyID { get; set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; set; }

    public int RoomNumber { get; set; }

    [StringLength(50)]
    public string RoomName { get; set; }

    public ICollection<Bed> Beds { get; set; }
     
    public int BedCount { get; set; }// mariam
}