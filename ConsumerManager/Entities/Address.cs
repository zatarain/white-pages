using System.Diagnostics.CodeAnalysis;

namespace ConsumerManager.Entities
{
  public record Address
  {
    public required int Id { get; init; }
    public required string Line1 { get; init; }

    [AllowNull]
    public string Line2 { get; init; }
    public required string Town{ get; init; }
    
    [AllowNull]
    public string County { get; set;}
    public required string Postcode { get; init; }
    public required string Country { get; init; }

    public virtual required Customer Customer { get; init; }

    public virtual DateTime CreatedAt { get; init; }
    public virtual DateTime LastUpdatedAt { get; init; }
  }
}
