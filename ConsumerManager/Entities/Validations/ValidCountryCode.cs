using System.ComponentModel.DataAnnotations;

namespace ConsumerManager.Entities.Validations
{
  public class ValidCountryCode : ValidationAttribute
  {
    private readonly HashSet<string> countries = new() {
      "GB",
      "MX",
      "US",
    };

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
      var country = value ?? "GB";
      if (!countries.Contains(country))
      {
        return new ValidationResult($"Invalid for country code '{country}'");
      }
      return ValidationResult.Success;
    }
  }
}
