namespace TimeCrm.DTOs;

using System.ComponentModel.DataAnnotations;

public class TimeEntryDto
{
    [Required]
    public DateOnly Date { get; set; }

    [Range(0.01, 24)]
    public decimal Hours { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int TaskId { get; set; }
}