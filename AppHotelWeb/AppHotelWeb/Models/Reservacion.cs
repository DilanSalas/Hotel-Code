using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppHotelWeb.Models
{
    public class Reservacion
    { 
        [Key]
        public int Id { get; set; }

       
        public string EmailCliente { get; set; }

        public string NombreCliente { get; set; }
        public string Tipo { get; set; }

        public int Costo { get; set; }

        public int Noche { get; set; }

        public decimal Prima { get; set; }

        public decimal MontoTotal { get; set; }

        public decimal MontoTotalCol { get; set; }

        public int CantidadPersonas { get; set; }

        public decimal Mensualidad { get; set; }

        public decimal Descuento { get; set; }

        public decimal DescuentoCol { get; set; }

        public decimal Impuesto { get; set; }

        public decimal ImpuestoCol { get; set; }

        public int idUsuario { get; set; }

        public string tipoPago { get; set; }

        [Column("IdCheque", TypeName = "int")]
        public int? IdCheque { get; set; }

        [Column("Banco", TypeName = "varchar(100)")]
        public string? Banco { get; set; }


    }//Fin class
}//Fin namespace
