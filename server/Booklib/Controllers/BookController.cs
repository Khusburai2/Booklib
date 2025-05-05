using System;
using Booklib.Data;
using Booklib.DTOs.Request;
using Booklib.DTOs.Response;
using Booklib.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booklib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BookController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Book/GetAll
        [HttpGet("GetAll")]
        public ActionResult<ICollection<BookResponseDTO>> GetAllBooks()
        {
            var books = _context.Books.ToList();
            
            if (books == null || books.Count == 0)
                return NotFound("No books found");
                
            var response = books.Select(book => MapToResponseDTO(book)).ToList();
            return Ok(response);
        }

        // GET: Book/GetFiltered
        [HttpGet("GetFiltered")]
        public ActionResult GetFilteredBooks(
            [FromQuery] string? genre,
            [FromQuery] string? search,
            [FromQuery] string? author,
            [FromQuery] bool? onSale,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? language,
            [FromQuery] string? format,
            [FromQuery] string? publisher,
            [FromQuery] int? minRating,
            [FromQuery] string? sortBy = "title", // Default sort by title
            [FromQuery] string? sortOrder = "asc", // Default ascending order
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Books.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(genre))
                query = query.Where(b => b.Genre == genre);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => 
                    b.Title.Contains(search) || 
                    b.Description.Contains(search) ||
                    b.ISBN.Contains(search));

            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author.Contains(author));

            if (onSale.HasValue)
                query = query.Where(b => b.OnSale == onSale.Value);

            if (minPrice.HasValue)
                query = query.Where(b => b.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(b => b.Price <= maxPrice.Value);

            if (!string.IsNullOrEmpty(language))
                query = query.Where(b => b.Language == language);

            if (!string.IsNullOrEmpty(format))
                query = query.Where(b => b.Format == format);

            if (!string.IsNullOrEmpty(publisher))
                query = query.Where(b => b.Publisher == publisher);

            if (minRating.HasValue)
            {
                // Assuming you'll add ratings later
                // query = query.Where(b => b.AverageRating >= minRating.Value);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "title" => sortOrder == "desc" 
                    ? query.OrderByDescending(b => b.Title)
                    : query.OrderBy(b => b.Title),
                "price" => sortOrder == "desc" 
                    ? query.OrderByDescending(b => b.Price)
                    : query.OrderBy(b => b.Price),
                "year" => sortOrder == "desc" 
                    ? query.OrderByDescending(b => b.YearPublished)
                    : query.OrderBy(b => b.YearPublished),
                "dateadded" => sortOrder == "desc" 
                    ? query.OrderByDescending(b => b.AddedDate)
                    : query.OrderBy(b => b.AddedDate),
                _ => query.OrderBy(b => b.Title) // Default sort
            };

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var books = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (books.Count == 0)
                return NotFound("No books match the criteria");

            var response = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages,
                Books = books.Select(book => MapToResponseDTO(book)).ToList()
            };

            return Ok(response);
        }

        // GET: Book/GetById/{id}
        [HttpGet("GetById/{id}")]
        public ActionResult<BookResponseDTO> GetBook(Guid id)
        {
            var book = _context.Books.Find(id);
            
            if (book == null)
                return NotFound("Book not found");
                
            return Ok(MapToResponseDTO(book));
        }

        // POST: Book/Create
        [HttpPost("Create")]
        public IActionResult CreateBook([FromBody] BookRequestDTO bookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Books.Any(b => b.Title == bookDTO.Title))
                return Conflict("Book title already exists");

            var book = new Book
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                ImageURL = bookDTO.ImageURL,
                ISBN = bookDTO.ISBN,
                Description = bookDTO.Description,
                Genre = bookDTO.Genre,
                Price = bookDTO.Price,
                YearPublished = bookDTO.YearPublished,
                Publisher = bookDTO.Publisher,
                Language = bookDTO.Language,
                Format = bookDTO.Format,
                StockQuantity = bookDTO.StockQuantity,
                IsAvailable = bookDTO.IsAvailable,
                OnSale = bookDTO.OnSale,
                DiscountPrice = bookDTO.DiscountPrice,
                DiscountEndDate = bookDTO.DiscountEndDate
            };

            _context.Books.Add(book);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, MapToResponseDTO(book));
        }

        // PUT: Book/Update/{id}
        [HttpPut("Update/{id}")]
        public IActionResult UpdateBook(Guid id, [FromBody] BookRequestDTO bookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBook = _context.Books.Find(id);
            if (existingBook == null)
                return NotFound("Book not found");

            if (_context.Books.Any(b => b.Title == bookDTO.Title && b.BookId != id))
                return Conflict("Book title already exists");

            // Update properties
            existingBook.Title = bookDTO.Title;
            existingBook.Author = bookDTO.Author;
            existingBook.ImageURL = bookDTO.ImageURL;
            existingBook.ISBN = bookDTO.ISBN;
            existingBook.Description = bookDTO.Description;
            existingBook.Genre = bookDTO.Genre;
            existingBook.Price = bookDTO.Price;
            existingBook.YearPublished = bookDTO.YearPublished;
            existingBook.Publisher = bookDTO.Publisher;
            existingBook.Language = bookDTO.Language;
            existingBook.Format = bookDTO.Format;
            existingBook.StockQuantity = bookDTO.StockQuantity;
            existingBook.IsAvailable = bookDTO.IsAvailable;
            existingBook.OnSale = bookDTO.OnSale;
            existingBook.DiscountPrice = bookDTO.DiscountPrice;
            existingBook.DiscountEndDate = bookDTO.DiscountEndDate;

            _context.SaveChanges();
            return Ok("Book updated successfully");
        }

        // DELETE: Book/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteBook(Guid id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
                return NotFound("Book not found");

            _context.Books.Remove(book);
            _context.SaveChanges();
            return Ok("Book deleted successfully");
        }

        // PATCH: Book/UpdateStock/{id}
        [HttpPatch("UpdateStock/{id}")]
        public IActionResult UpdateStock(Guid id, [FromBody] int quantity)
        {
            var book = _context.Books.Find(id);
            if (book == null)
                return NotFound("Book not found");

            book.StockQuantity = quantity;
            _context.SaveChanges();
            return Ok("Stock updated successfully");
        }

        // Add Author Specific Endpoints
        [HttpGet("GetByAuthor/{authorName}")]
        public ActionResult GetBooksByAuthor(string authorName)
        {
            var books = _context.Books
                .Where(b => b.Author.Contains(authorName))
                .ToList();

            if (books.Count == 0)
                return NotFound($"No books found for author: {authorName}");

            var response = books.Select(book => MapToResponseDTO(book)).ToList();
            return Ok(response);
        }

        [HttpGet("GetAuthors")]
        public ActionResult<ICollection<string>> GetAllAuthors()
        {
            var authors = _context.Books
                .Select(b => b.Author)
                .Distinct()
                .ToList();

            return authors.Count == 0
                ? NotFound("No authors found")
                : Ok(authors);
        }

        // Helper method to map Book entity to BookResponseDTO
        private BookResponseDTO MapToResponseDTO(Book book)
        {
            return new BookResponseDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ImageURL = book.ImageURL,
                ISBN = book.ISBN,
                Description = book.Description,
                Genre = book.Genre,
                Price = book.Price,
                YearPublished = book.YearPublished,
                Publisher = book.Publisher,
                Language = book.Language,
                Format = book.Format,
                StockQuantity = book.StockQuantity,
                IsAvailable = book.IsAvailable,
                OnSale = book.OnSale,
                DiscountPrice = book.DiscountPrice,
                DiscountEndDate = book.DiscountEndDate,
                AddedDate = book.AddedDate
            };
        }
    }
}