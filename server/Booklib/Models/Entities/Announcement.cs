using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booklib.Models.Entities;

public class Announcement
    {
        [Key]
        public Guid AnnouncementId { get; set; } = Guid.NewGuid();
        
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

        // Optional category for announcement
        public string? Category { get; set; }
        
        // Optional reference to a book
        public Guid? BookId { get; set; }
        
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
