using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsumerManager.Entities
{
  public record Customer
  {
    public int? Id { get; set; }

    [MaxLength(20)]
    public string? Title { get; set; }

    [MaxLength(50)]
    public string? Forename { get; set; }

    [MaxLength(50)]
    public string? Surname { get; set; }

    [MaxLength(75)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(15)]
    [Phone]
    public string? Phone { get; set; }

    [DefaultValue(true)]
    public bool IsActive { get; set; }

    [DefaultValue(0)]
    public int MainAddressId { get; set; }

    public virtual ICollection<Address>? Addresses { get; set; }

    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime LastUpdatedAt { get; set; }
  }

  public record CreateCustomerRequest
  {
    [MaxLength(20)]
    [Required]
    public string? Title { get; init; }

    [MaxLength(50)]
    [Required]
    public string? Forename { get; init; }

    [MaxLength(50)]
    [Required]
    public string? Surname { get; init; }

    [MaxLength(75)]
    [EmailAddress]
    [Required]
    public string? Email { get; init; }

    [MaxLength(15)]
    [Phone]
    [Required]
    public string? Phone { get; init; }

    //public required virtual ICollection<Address> Addresses { get; set; }
  }
}
