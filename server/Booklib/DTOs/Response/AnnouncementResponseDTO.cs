using System;

namespace Booklib.DTOs.Response;

public class AnnouncementResponseDTO
    {
        public Guid AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Category { get; set; }
        public Guid? BookId { get; set; }
        public string? BookTitle { get; set; }
    }