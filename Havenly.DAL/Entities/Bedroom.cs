using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Bedroom
{
    [Key]
    public long BedroomID { get; private set; }

    public long PropertyID { get; private set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; private set; }

    public int RoomNumber { get; private set; }

    [StringLength(50)]
    public string RoomName { get; private set; }

    public int BedCount { get; set; }// mariam
    public ICollection<Bed> Beds { get; private set; }

    public void Create(long bedroomId, long propertyId, int roomNumber,int bedcnt, string roomName = null)
    {
        BedroomID = bedroomId;
        PropertyID = propertyId;
        RoomNumber = roomNumber;
        RoomName = roomName;
       BedCount=bedcnt;
        Beds = new List<Bed>();
    }

    public void Update(int roomNumber, string roomName)
    {
        RoomNumber = roomNumber;
        RoomName = roomName;
    }
}

     
  
}

