using System;
using System.ComponentModel.DataAnnotations;

namespace Booklib.Models.Entities;

public class Announcement
{
    [Key]
    public Guid AnnouncementId { get; set; } = Guid.NewGuid(); // Fixed typo: AnnoucementId → AnnouncementId

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
}
