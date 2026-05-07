using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.DTOs;

public class UpdateStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
