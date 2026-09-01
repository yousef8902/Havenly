using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class PropertyImage
{


    [Key]
    public long ImageID { get; private set; }

    public long PropertyID { get; set; }

    [ForeignKey(nameof(PropertyID))]
    public Property Property { get;  set; }

    public bool? IsPrimary { get; set; } = false;// mariam -- for UI
    public string ImagePath { get;  set; }

    public void Create(long imageId, long propertyId, string imagePath, bool isPrimary = false)
    {
        ImageID = imageId;
        PropertyID = propertyId;
        ImagePath = imagePath;
        IsPrimary = isPrimary;
    }

    public void UpdatePath(string imagePath)
    {
        ImagePath = imagePath;
    }


    

}