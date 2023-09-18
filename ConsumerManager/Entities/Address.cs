using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ConsumerManager.Entities
{
  public record Address
  {
    public required int Id { get; init; }
    
    [MaxLength(80)]
    public required string Line1 { get; init; }

    [AllowNull]
    [MaxLength(80)]
    public string Line2 { get; init; }
    
    [MaxLength(50)]
    public required string Town{ get; init; }
    
    [AllowNull]
    [MaxLength(50)]
    public string County { get; set;}

    [MaxLength(10)]
    public required string Postcode { get; init; }

    [MaxLength(2)]
    public required string Country { get; init; }

    public virtual required Customer Customer { get; init; }

    public virtual DateTime CreatedAt { get; init; }
    public virtual DateTime LastUpdatedAt { get; init; }
  }
}
