using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace Barshop.Models
{
    public class Producto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el codigo")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo Numeros")]
        public int codigo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el nombre")]
        [StringLength(30, ErrorMessage = "Longitud minima de 3 caracteres", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z\s]+$")]

        public string nombre { get; set; }


        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal precio { get; set; }


        public string descripcion { get; set; }

        public int idcategoria { get; set; }

        public string Foto { get; set; }
    }
}