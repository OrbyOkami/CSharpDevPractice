using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TimeCrm.Models;

using System.ComponentModel.DataAnnotations;

public class TimeEntry
{
    public int Id { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Range(0.01, 24)]
    public decimal Hours { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public int TaskId { get; set; }

    [ValidateNever]
    [JsonIgnore]
    public TaskItem Task { get; set; } = null!;

    public bool TaskWasActiveAtCreation { get; set; }
}