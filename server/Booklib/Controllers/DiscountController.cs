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
    public class DiscountController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DiscountController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Discount/GetAll
        [HttpGet("GetAll")]
        public ActionResult<ICollection<DiscountResponseDTO>> GetAllDiscounts()
        {
            var discounts = _context.Discounts
                .Include(d => d.Book)
                .ToList();
            
            if (discounts == null || discounts.Count == 0)
                return NotFound("No discounts found");
                
            var response = discounts.Select(discount => MapToResponseDTO(discount)).ToList();
            return Ok(response);
        }

        // GET: Discount/GetActive
        [HttpGet("GetActive")]
        public ActionResult<ICollection<DiscountResponseDTO>> GetActiveDiscounts()
        {
            var now = DateTime.UtcNow;
            var discounts = _context.Discounts
                .Include(d => d.Book)
                .Where(d => d.StartDate <= now && d.EndDate >= now)
                .ToList();

            if (discounts.Count == 0)
                return NotFound("No active discounts found");

            return Ok(discounts.Select(d => MapToResponseDTO(d)).ToList());
        }

        // GET: Discount/GetById/{id}
        [HttpGet("GetById/{id}")]
        public ActionResult<DiscountResponseDTO> GetDiscount(Guid id)
        {
            var discount = _context.Discounts
                .Include(d => d.Book)
                .FirstOrDefault(d => d.DiscountId == id);
            
            if (discount == null)
                return NotFound("Discount not found");
                
            return Ok(MapToResponseDTO(discount));
        }

        [HttpPost("Create")]
public IActionResult CreateDiscount([FromBody] DiscountRequestDTO discountDTO)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var book = _context.Books.Find(discountDTO.BookId);
    if (book == null)
        return BadRequest("Book not found");

    // Check for existing active discounts
    var existingActiveDiscount = _context.Discounts
        .FirstOrDefault(d => d.BookId == discountDTO.BookId 
                            && d.EndDate > DateTime.UtcNow 
                            && d.IsOnSale);
                            
    if (existingActiveDiscount != null)
    {
        // Either return error
        return Conflict("Book already has an active discount");
        
        // OR automatically remove the existing discount
        /*
        _context.Discounts.Remove(existingActiveDiscount);
        book.OnSale = false;
        book.DiscountPrice = null;
        book.DiscountEndDate = null;
        */
    }

    // Validate dates
    if (discountDTO.StartDate >= discountDTO.EndDate)
        return BadRequest("End date must be after start date");

    // Ensure UTC dates
    var startDate = discountDTO.StartDate.Kind == DateTimeKind.Unspecified 
        ? DateTime.SpecifyKind(discountDTO.StartDate, DateTimeKind.Utc) 
        : discountDTO.StartDate.ToUniversalTime();

    var endDate = discountDTO.EndDate.Kind == DateTimeKind.Unspecified 
        ? DateTime.SpecifyKind(discountDTO.EndDate, DateTimeKind.Utc) 
        : discountDTO.EndDate.ToUniversalTime();

    var discount = new Discount
    {
        BookId = discountDTO.BookId,
        Percentage = discountDTO.Percentage,
        StartDate = startDate,
        EndDate = endDate,
        IsOnSale = discountDTO.IsOnSale
    };

    // Update book's sale status
    if (discountDTO.IsOnSale)
    {
        book.OnSale = true;
        book.DiscountPrice = book.Price * (1 - discountDTO.Percentage / 100);
        book.DiscountEndDate = endDate;
    }

    _context.Discounts.Add(discount);
    _context.SaveChanges();
    
    return CreatedAtAction(nameof(GetDiscount), 
        new { id = discount.DiscountId }, 
        MapToResponseDTO(discount));
}

        // PUT: Discount/Update/{id}
        [HttpPut("Update/{id}")]
        public IActionResult UpdateDiscount(Guid id, [FromBody] DiscountRequestDTO discountDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var discount = _context.Discounts.Find(id);
            if (discount == null)
                return NotFound("Discount not found");

            // Validate dates
            if (discountDTO.StartDate >= discountDTO.EndDate)
                return BadRequest("End date must be after start date");

            // Ensure UTC dates
            discount.StartDate = discountDTO.StartDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(discountDTO.StartDate, DateTimeKind.Utc) 
                : discountDTO.StartDate.ToUniversalTime();

            discount.EndDate = discountDTO.EndDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(discountDTO.EndDate, DateTimeKind.Utc) 
                : discountDTO.EndDate.ToUniversalTime();

            discount.Percentage = discountDTO.Percentage;
            discount.IsOnSale = discountDTO.IsOnSale;

            // Update related book if needed
            var book = _context.Books.Find(discountDTO.BookId);
            if (book != null && discountDTO.IsOnSale)
            {
                book.OnSale = true;
                book.DiscountPrice = book.Price * (1 - discountDTO.Percentage / 100);
                book.DiscountEndDate = discount.EndDate;
            }

            _context.SaveChanges();
            return Ok("Discount updated successfully");
        }

        // DELETE: Discount/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteDiscount(Guid id)
        {
            var discount = _context.Discounts.Find(id);
            if (discount == null)
                return NotFound("Discount not found");

            // Remove sale status from book if this was the active discount
            var book = _context.Books.Find(discount.BookId);
            if (book != null && discount.IsOnSale)
            {
                book.OnSale = false;
                book.DiscountPrice = null;
                book.DiscountEndDate = null;
            }

            _context.Discounts.Remove(discount);
            _context.SaveChanges();
            return Ok("Discount deleted successfully");
        }

        // Helper method to map Discount entity to DiscountResponseDTO
        private DiscountResponseDTO MapToResponseDTO(Discount discount)
        {
            return new DiscountResponseDTO
            {
                DiscountId = discount.DiscountId,
                BookId = discount.BookId,
                Percentage = discount.Percentage,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsOnSale = discount.IsOnSale,
                BookTitle = discount.Book?.Title ?? string.Empty
            };
        }
    }
}