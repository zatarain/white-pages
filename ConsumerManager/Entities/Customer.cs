using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsumerManager.Entities
{
  public record Customer
  {
    public int? Id { get; set; }

    [MaxLength(20)]
    public required string Title { get; set; }
    
    [MaxLength(50)]
    public required string Forename { get; set; }
    
    [MaxLength(50)]
    public required string Surname { get; set; }
    
    [MaxLength(75)]
    public required string Email { get; set; }
    
    [MaxLength(15)]
    public required string Phone { get; set; }

    public required bool IsActive { get; set; }

    public virtual ICollection<Address>? Addresses { get; set; }
    
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime LastUpdatedAt { get; set; }
  }

  public record CreateCustomerContract
  {
    [MaxLength(20)]
    public required string Title { get; init; }

    [MaxLength(50)]
    public required string Forename { get; init; }

    [MaxLength(50)]
    public required string Surname { get; init; }

    [MaxLength(75)]
    public required string Email { get; init; }

    [MaxLength(15)]
    public required string Phone { get; init; }

    //public required virtual ICollection<Address> Addresses { get; set; }
  }
}
 