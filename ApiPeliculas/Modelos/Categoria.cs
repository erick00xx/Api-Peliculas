using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    // MODELO CREADO PARA LA BASE DE DATOS
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Fecha de Creacion")] // para vistas como web razor en forms
        public DateTime FechaCreacion { get; set; }
    }
}
