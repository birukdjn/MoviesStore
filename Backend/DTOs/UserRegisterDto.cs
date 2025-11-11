using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public record UserRegisterDto(
        [Required]string Username, 
        [Required][EmailAddress]string Email, 
        [Required][MinLength(8)]string Password,
        string? Avatar );

}
