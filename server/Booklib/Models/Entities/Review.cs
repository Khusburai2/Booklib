using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booklib.Models.Entities;

public class Review
{
[Key]
        public Guid ReviewId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MemberId { get; set; }

        // [ForeignKey("MemberId")]
        // public Member Member { get; set; }

        [Required]
        public Guid BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
}
