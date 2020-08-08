using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Barshop.Models
{
    public class Trabajador
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el codigo")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo Numeros")]

        public int Codigo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el DNI")]
        [RegularExpression(@"^[0-9]{8}", ErrorMessage = "Solo 8 digitos")]
        public string DNI { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el nombre")]
        [StringLength(30, ErrorMessage = "Longitud minima de 3 caracteres", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el usuario")]
        public string Usuario { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese la contraseña")]
        public string Password { get; set; }
        [Display(Name = "Fecha Contrato")]
        public DateTime Fecha_Contrato { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese la direccion")]
        [StringLength(60, ErrorMessage = "Longitud minima de 3 caracteres", MinimumLength = 3)]
        public string Direccion { get; set; }
    }
}