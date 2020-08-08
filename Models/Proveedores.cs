using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Barshop.Models
{
    public class Proveedores
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el codigo")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo Numeros")]
        public int idProveedor { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el ruc")]
        [StringLength(11, ErrorMessage = "Longitud minima de 11 caracteres", MinimumLength = 11)]
        public string rucProveedor { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese la razon social")]
        public string razSocial { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese la direccion")]
        public string direccion { get; set; }
    }
}