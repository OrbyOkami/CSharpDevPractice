namespace TimeCrm.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int ProjectId { get; set; }

    public bool IsActive { get; set; } = true;
}