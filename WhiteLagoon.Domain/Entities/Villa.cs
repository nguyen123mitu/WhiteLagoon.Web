using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace WhiteLagoon.Domain.Entities;

public class Villa
{
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    [Display(Name="Price per night")]
    [Range(10,10000)]
    public double Price { get; set; }
    public int Sqft { get; set; }
    [NotMapped]
    public IFormFile? Image { get; set; }
    [Range(1,10)]
    public int Occupancy { get; set; }
    [Display(Name="Image Url")]

    public string? ImageUrl { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public IEnumerable<Amenity> VillaAmenity { get; set; }
    [NotMapped]
    public bool IsAvailable { get; set; } = true;
}
