using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsumerManager.Entities
{
  public record Customer
  {
    public required int Id { get; init; }

    [MaxLength(20)]
    public required string Title { get; init; }
    
    [MaxLength(50)]
    public required string Forename { get; init; }
    
    [MaxLength(50)]
    public required string Surename { get; init; }
    
    [MaxLength(75)]
    public required string Email { get; init; }
    
    [MaxLength(15)]
    public required string Phone { get; init; }

    [JsonIgnore]
    public required virtual ICollection<Address> Addresses { get; init; }
    
    public virtual DateTime CreatedAt { get; init; }
    public virtual DateTime LastUpdatedAt { get; init; }
  }
}
 