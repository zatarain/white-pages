using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsumerManager.Entities
{
  public record Customer
  {
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Forename { get; init; }
    public required string Surname { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required bool IsActive { get; init; }
    public required int MainAddressId { get; init; }
    public virtual ICollection<Address>? Addresses { get; init; }
    public virtual DateTime CreatedAt { get; init; }
    public virtual DateTime LastUpdatedAt { get; init; }
  }

  public record CreateCustomerRequest
  {
    [MaxLength(20)]
    [Required]
    public required string Title { get; init; }

    [MaxLength(50)]
    [Required]
    public required string Forename { get; init; }

    [MaxLength(50)]
    [Required]
    public required string Surname { get; init; }

    [MaxLength(75)]
    [EmailAddress]
    [Required]
    public required string Email { get; init; }

    [MaxLength(15)]
    [Phone]
    [Required]
    public required string Phone { get; init; }
  }
}
