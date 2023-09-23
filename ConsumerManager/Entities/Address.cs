using ConsumerManager.Entities.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ConsumerManager.Entities
{
  public record Address
  {
    public int? Id { get; set; }

    [MaxLength(80)]
    public string? Line1 { get; set; }

    [AllowNull]
    [MaxLength(80)]
    public string? Line2 { get; set; }

    [MaxLength(50)]
    public string? Town { get; set; }

    [AllowNull]
    [MaxLength(50)]
    [DefaultValue("")]
    public string? County { get; set; }

    [MaxLength(10)]
    [ValidPostcode]
    public string? Postcode { get; set; }

    [MaxLength(2)]
    [DefaultValue("GB")]
    public string? Country { get; set; }

    public int? CustomerId { get; set; }

    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime LastUpdatedAt { get; set; }
  }

  public record CreateAddressRequest
  {
    [MaxLength(80)]
    [Required]
    public string? Line1 { get; init; }

    [AllowNull]
    [MaxLength(80)]
    public string? Line2 { get; init; }

    [MaxLength(50)]
    [Required]
    public string? Town { get; init; }

    [AllowNull]
    [MaxLength(50)]
    [DefaultValue("")]
    public string? County { get; init; }

    [MaxLength(10)]
    [ValidPostcode]
    [Required]
    public string? Postcode { get; init; }

    [MaxLength(2)]
    [DefaultValue("GB")]
    public string? Country { get; init; }

    [Required]
    public int? CustomerId { get; init; }
  }
}
