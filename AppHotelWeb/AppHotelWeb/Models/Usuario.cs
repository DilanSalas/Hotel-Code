using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppHotelWeb.Models
{
    public class Usuario
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "No se permite la cedula en blanco")]
        public int Cedula {  get; set; }

        [Required(ErrorMessage = "No se permite el nombre en blanco")]
        [StringLength(100)]
        public String nombreCompleto { get; set; }

        [Required(ErrorMessage = "No se permite el telefono en blanco")]
        public int telefono {  get; set; }

        [Required(ErrorMessage = "No se permite la direccion en blanco")]
        public string direccion {  get; set; }

        [Required(ErrorMessage = "No se permite el email en blanco")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set;}

        [Required(ErrorMessage = "Favor indicar el rol")]
        public String Rol { get; set; }

        [Column("Clave",TypeName= "varchar(100)")]
        public string? clave { get; set; }


    }
}
