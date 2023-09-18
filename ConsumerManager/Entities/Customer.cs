using System.Text.Json.Serialization;

namespace ConsumerManager.Entities
{
  public record Customer
  {
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Forename { get; init; }
    public required string Surename { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }

    [JsonIgnore]
    public required virtual ICollection<Address> Addresses { get; init; }
    public virtual DateTime CreatedAt { get; init; }
    public virtual DateTime LastUpdatedAt { get; init; }
  }
}
 