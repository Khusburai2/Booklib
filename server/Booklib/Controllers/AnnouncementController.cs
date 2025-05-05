using Booklib.Data;
using Booklib.DTOs.Request;
using Booklib.DTOs.Response;
using Booklib.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booklib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AnnouncementController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Announcement
        [HttpGet]
        public ActionResult<IEnumerable<AnnouncementResponseDTO>> GetAnnouncements()
        {
            var announcements = _context.Announcements
                .Include(a => a.Book)
                .Select(a => MapToDTO(a))
                .ToList();

            if (!announcements.Any())
                return NotFound("No announcements found");

            return Ok(announcements);
        }

        // GET: api/Announcement/active
        [HttpGet("active")]
        public ActionResult<IEnumerable<AnnouncementResponseDTO>> GetActiveAnnouncements()
        {
            var now = DateTime.UtcNow;
            var announcements = _context.Announcements
                .Include(a => a.Book)
                .Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now)
                .Select(a => MapToDTO(a))
                .ToList();

            if (!announcements.Any())
                return NotFound("No active announcements found");

            return Ok(announcements);
        }

        // GET: api/Announcement/category/{category}
        [HttpGet("category/{category}")]
        public ActionResult<IEnumerable<AnnouncementResponseDTO>> GetAnnouncementsByCategory(string category)
        {
            var now = DateTime.UtcNow;
            var announcements = _context.Announcements
                .Include(a => a.Book)
                .Where(a => a.IsActive && 
                          a.Category == category &&
                          a.StartDate <= now && 
                          a.EndDate >= now)
                .Select(a => MapToDTO(a))
                .ToList();

            if (!announcements.Any())
                return NotFound($"No active announcements found for category: {category}");

            return Ok(announcements);
        }

        // GET: api/Announcement/{id}
        [HttpGet("{id}")]
        public ActionResult<AnnouncementResponseDTO> GetAnnouncement(Guid id)
        {
            var announcement = _context.Announcements
                .Include(a => a.Book)
                .FirstOrDefault(a => a.AnnouncementId == id);

            if (announcement == null)
                return NotFound("Announcement not found");

            return Ok(MapToDTO(announcement));
        }

        // GET: api/Announcement/book/{bookId}
        [HttpGet("book/{bookId}")]
        public ActionResult<IEnumerable<AnnouncementResponseDTO>> GetAnnouncementsByBook(Guid bookId)
        {
            var announcements = _context.Announcements
                .Include(a => a.Book)
                .Where(a => a.BookId == bookId)
                .Select(a => MapToDTO(a))
                .ToList();

            if (!announcements.Any())
                return NotFound($"No announcements found for book ID: {bookId}");

            return Ok(announcements);
        }

        // GET: api/Announcement/categories
        [HttpGet("categories")]
        public ActionResult<IEnumerable<string>> GetCategories()
        {
            var categories = _context.Announcements
                .Where(a => a.Category != null)
                .Select(a => a.Category)
                .Distinct()
                .ToList();

            return Ok(categories);
        }

        // POST: api/Announcement
        [HttpPost]
        public ActionResult<AnnouncementResponseDTO> CreateAnnouncement(AnnouncementRequestDTO announcementDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate dates
            if (announcementDTO.StartDate >= announcementDTO.EndDate)
                return BadRequest("End date must be after start date");

            // Validate book reference if provided
            if (announcementDTO.BookId.HasValue)
            {
                var book = _context.Books.Find(announcementDTO.BookId.Value);
                if (book == null)
                    return BadRequest("Referenced book does not exist");
            }

            // Ensure UTC dates
            var startDate = announcementDTO.StartDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(announcementDTO.StartDate, DateTimeKind.Utc)
                : announcementDTO.StartDate.ToUniversalTime();

            var endDate = announcementDTO.EndDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(announcementDTO.EndDate, DateTimeKind.Utc)
                : announcementDTO.EndDate.ToUniversalTime();

            var announcement = new Announcement
            {
                Title = announcementDTO.Title,
                Content = announcementDTO.Content,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = announcementDTO.IsActive,
                Category = announcementDTO.Category,
                BookId = announcementDTO.BookId
            };

            _context.Announcements.Add(announcement);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetAnnouncement),
                new { id = announcement.AnnouncementId },
                MapToDTO(announcement));
        }

        // PUT: api/Announcement/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateAnnouncement(Guid id, AnnouncementRequestDTO announcementDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var announcement = _context.Announcements.Find(id);
            if (announcement == null)
                return NotFound("Announcement not found");

            // Validate dates
            if (announcementDTO.StartDate >= announcementDTO.EndDate)
                return BadRequest("End date must be after start date");

            // Validate book reference if provided
            if (announcementDTO.BookId.HasValue)
            {
                var book = _context.Books.Find(announcementDTO.BookId.Value);
                if (book == null)
                    return BadRequest("Referenced book does not exist");
            }

            // Ensure UTC dates
            announcement.StartDate = announcementDTO.StartDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(announcementDTO.StartDate, DateTimeKind.Utc)
                : announcementDTO.StartDate.ToUniversalTime();

            announcement.EndDate = announcementDTO.EndDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(announcementDTO.EndDate, DateTimeKind.Utc)
                : announcementDTO.EndDate.ToUniversalTime();

            announcement.Title = announcementDTO.Title;
            announcement.Content = announcementDTO.Content;
            announcement.IsActive = announcementDTO.IsActive;
            announcement.Category = announcementDTO.Category;
            announcement.BookId = announcementDTO.BookId;

            _context.SaveChanges();
            return Ok("Announcement updated successfully");
        }

        // DELETE: api/Announcement/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteAnnouncement(Guid id)
        {
            var announcement = _context.Announcements.Find(id);
            if (announcement == null)
                return NotFound("Announcement not found");

            _context.Announcements.Remove(announcement);
            _context.SaveChanges();

            return Ok("Announcement deleted successfully");
        }

        // PATCH: api/Announcement/{id}/toggle-active
        [HttpPatch("{id}/toggle-active")]
        public IActionResult ToggleAnnouncementActive(Guid id)
        {
            var announcement = _context.Announcements.Find(id);
            if (announcement == null)
                return NotFound("Announcement not found");

            announcement.IsActive = !announcement.IsActive;
            _context.SaveChanges();

            return Ok($"Announcement is now {(announcement.IsActive ? "active" : "inactive")}");
        }

        private static AnnouncementResponseDTO MapToDTO(Announcement announcement)
        {
            return new AnnouncementResponseDTO
            {
                AnnouncementId = announcement.AnnouncementId,
                Title = announcement.Title,
                Content = announcement.Content,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate,
                IsActive = announcement.IsActive,
                Category = announcement.Category,
                BookId = announcement.BookId,
                BookTitle = announcement.Book?.Title
            };
        }
    }
}