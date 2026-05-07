using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.DTOs;

public class UpdateTarefaDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Pendente";

    public string Prioridade { get; set; } = "Média";
}
