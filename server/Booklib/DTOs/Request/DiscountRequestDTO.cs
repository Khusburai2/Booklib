using System;
using System.ComponentModel.DataAnnotations;

namespace Booklib.DTOs.Request;

public class DiscountRequestDTO
    {

    [Required]
    public Guid BookId { get; set; }

    [Required, Range(0, 100)]
    public decimal Percentage { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow; // Default to UTC

    [Required]
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7); // Default to UTC

    public bool IsOnSale { get; set; } = false;

    // Add this constructor to ensure UTC conversion
    public DiscountRequestDTO()
    {
        if (StartDate.Kind == DateTimeKind.Unspecified)
            StartDate = DateTime.SpecifyKind(StartDate, DateTimeKind.Utc);
        
        if (EndDate.Kind == DateTimeKind.Unspecified)
            EndDate = DateTime.SpecifyKind(EndDate, DateTimeKind.Utc);
    }
}
    
