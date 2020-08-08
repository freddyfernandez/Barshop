using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Barshop.Models
{
    public class Registro
    {
        public int idproducto { get; set; }
        public string nombreproducto { get; set; }
       
        public decimal preciounidad { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese la cantidad")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo Numeros")]
        public int cantidad { get; set; }
        public decimal monto { get; set; }

        public string dni { get; set; }

        public string nombre { get; set; }

        public string direccion { get; set; }

        public int contador { get; set; }
    }
}