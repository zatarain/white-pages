using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
    public string? County { get; set; }

    [MaxLength(10)]
    public string? Postcode { get; set; }

    [MaxLength(2)]
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
    public string? County { get; init; }

    [MaxLength(10)]
    [Required]
    public string? Postcode { get; init; }

    [MaxLength(2)]
    public string? Country { get; init; }

    [Required]
    public int? CustomerId { get; init; }
  }
}
