using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booklib.Models.Entities;

public class Discount
{

[Key]
    public Guid DiscountId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid BookId { get; set; }

    [ForeignKey("BookId")]
    public Book Book { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal Percentage { get; set; }  

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsOnSale { get; set; } = false;
}