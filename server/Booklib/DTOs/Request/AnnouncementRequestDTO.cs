using System;
using System.ComponentModel.DataAnnotations;

namespace Booklib.DTOs.Request;

 public class AnnouncementRequestDTO
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
        
        public string? Category { get; set; }
        
        public Guid? BookId { get; set; }
    }
