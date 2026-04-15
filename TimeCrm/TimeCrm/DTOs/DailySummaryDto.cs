namespace TimeCrm.DTOs;

public class DailySummaryDto
{
    public DateOnly Date { get; set; }
    public decimal TotalHours { get; set; }
    public string Color { get; set; } = string.Empty;
}