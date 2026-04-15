using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TimeCrm.Models;

using System.ComponentModel.DataAnnotations;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    [ValidateNever]
    [JsonIgnore]
    public Project Project { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    [ValidateNever]
    [JsonIgnore]
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}