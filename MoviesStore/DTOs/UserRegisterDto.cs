using System.ComponentModel.DataAnnotations;

namespace MoviesStore.DTOs
{
    public record UserRegisterDto(
        [Required]string Username, 
        [Required][EmailAddress]string Email, 
        [Required][MinLength(8)]string Password);
    
}
