using System.ComponentModel.DataAnnotations;

namespace IF133.Tugas1.Api.Entities;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name wajib diisi.")]
    [StringLength(100, ErrorMessage = "Name maksimal 100 karakter.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description maksimal 500 karakter.")]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Price tidak boleh negatif.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock tidak boleh negatif.")]
    public int Stock { get; set; }
}
