using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barshop.Models
{
    public class Pedido
    {
        public int idpedido { get; set; }
        public DateTime fecha { get; set; }

        

        public string dni { get; set; }
        public string nombre { get; set; }

        public string direccion { get; set; }
        public string telefono { get; set; }
        public decimal monto { get; set; }

        public string estado { get; set; }

        public int cod_estado { get; set; }


     
    }
    public class Estado {
        public int idestado { get; set; }
        public string estado_actual { get; set; }
    }

    
}

