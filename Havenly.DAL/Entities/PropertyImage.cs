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

    [Required]
    public bool? IsPrimary { get; set; }// mariam -- for UI
    public string ImagePath { get;  set; }

    public void Create(long imageId, long propertyId, string imagePath)
    {
        ImageID = imageId;
        PropertyID = propertyId;
        ImagePath = imagePath;
    }

    public void UpdatePath(string imagePath)
    {
        ImagePath = imagePath;
    }


    

}