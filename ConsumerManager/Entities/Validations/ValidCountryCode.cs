using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
      var request = context.ObjectInstance as CreateAddressRequest;
      var country = request?.Country ?? "GB";
      if (request is not null && !countries.Contains(country))
      {
        return new ValidationResult($"Invalid for country code '{country}'");
      }
      return ValidationResult.Success;
    }
  }
}
