using ConsumerManager.Entities.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ConsumerManager.Entities
{
  public record Address
  {
    public int Id { get; init; }
    public required string Line1 { get; init; }
    public required string Line2 { get; init; }
    public required string Town { get; init; }
    public required string County { get; init; }
    public required string Postcode { get; init; }
    public required string Country { get; init; }
    public int CustomerId { get; init; }
    public virtual DateTime CreatedAt { get; init; }
    public virtual DateTime LastUpdatedAt { get; init; }
  }

  public record CreateAddressRequest
  {
    [MaxLength(80)]
    [Required]
    public required string Line1 { get; init; }

    [AllowNull]
    [MaxLength(80)]
    public string? Line2 { get; init; }

    [MaxLength(50)]
    [Required]
    public required string Town { get; init; }

    [AllowNull]
    [MaxLength(50)]
    public string? County { get; init; }

    [MaxLength(10)]
    [ValidPostcode]
    [Required]
    public required string Postcode { get; init; }

    [MaxLength(2)]
    [DefaultValue("GB")]
    public string? Country { get; init; }
  }
}
