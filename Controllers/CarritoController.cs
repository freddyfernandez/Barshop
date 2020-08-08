using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Barshop.Models;
using System.IO;
using System.Web.Mvc.Ajax;
using Barshop.ModelsApi;
using Rotativa;
using System.Web.WebPages;

namespace Barshop.Controllers


{
    public class CarritoController : Controller
    {

        ValuesController bd = new ValuesController();

       
        // GET: Carrito
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx"].ConnectionString);
        List<Producto> temporal = new List<Producto>();
       
        public ActionResult Index(string filtro=" ")
        {
            
            
            if (Session["persona"] == null)
            {

                List<Registro> detalle = new List<Registro>();
                Session["persona"] = detalle;
            }
            if (Session["carrito"] == null)
            {
                List<Registro> detalle = new List<Registro>();
                Session["carrito"] = detalle;

            }
            if (Session["pedido"] == null)
            {
                List<Pedido> detalle = new List<Pedido>();
                Session["pedido"] = detalle;

            }

            List<Registro> contador = (List<Registro>)Session["carrito"];
            ViewBag.contador = contador.Count;


            cn.Open();
            string sql = "select idproducto,nombreproducto,preciounidad from  tb_producto where nombreproducto like '%" + filtro + "%' ";//sintaxis sql nativa
            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {

                Producto obj = new Producto();
                obj.codigo = dr.GetInt32(0);
                obj.nombre = dr.GetString(1);
                obj.precio = dr.GetDecimal(2);
                temporal.Add(obj);

            }
            cn.Close();
            ViewBag.filtro = filtro;
           
            return View(temporal);
        }
        public ActionResult Seleccionar(int? id = null)
        {
            //definir lista y agregar al carrito
            List<Registro> detalle = (List<Registro>)Session["carrito"];
            ViewBag.contador = detalle.Count;

            return View(productos().Where(x => x.codigo == id).FirstOrDefault());//productos()

        }

        [HttpPost]
        public ActionResult Seleccionar(int id,string cantidad)
        {

            if (cantidad == "")
            {
                return RedirectToAction("Seleccionar");
            }
            Producto reg = productos().Where(x => x.codigo == id).FirstOrDefault();
            /*first_of_default convierte el listado a un dato especifico*/

            Registro it = new Registro();

            it.idproducto = reg.codigo;
            it.nombreproducto = reg.nombre;
            it.preciounidad = reg.precio;

            it.cantidad = Convert.ToInt32(cantidad);
            it.monto = reg.precio* Convert.ToInt32(cantidad);

       
          


            //definir lista y agregar al carrito
            List<Registro> detalle = (List<Registro>)Session["carrito"];


            ViewBag.contador = detalle.Count;

            detalle.Add(it);
            
            Session["carrito"] = detalle;


            return RedirectToAction("Index");
        }

        // GET: Servicio Personas
        public ActionResult personas()
        {
            return View(bd.Get());
        }

        public ActionResult BuscarPersona(int ? filtro = null)
        {
            List<Registro> contador = (List<Registro>)Session["carrito"];
            ViewBag.contador = contador.Count;
            return View(bd.Get().Where(x => x.dni == (filtro).ToString()).FirstOrDefault());


        }
        [HttpPost]
        public ActionResult BuscarPersona(string filtro)
        {
            
            Persona reg = bd.Get().Where(x => x.dni == filtro).FirstOrDefault();
            if (reg == null) {
                string mensaje = "No existe DNI consulte la Base de datos";
                ViewBag.mensaje = mensaje;
                return View();
            }
            Registro it = new Registro();

            it.dni = reg.dni;
            it.nombre = reg.nombre;
            List<Registro> detalle1 = (List<Registro>)Session["persona"];
            detalle1.Add(it);

            Session["persona"] = detalle1;
           
            return RedirectToAction("Pago");
        }


   

        public ActionResult Comprar()
        {
            List<Registro> detalle = (List<Registro>)Session["carrito"];
            decimal mt = 0;
           
            foreach (Registro it in detalle)
            {
                mt += it.monto;
            }
            
            ViewBag.mt = mt;
            ViewBag.contador = detalle.Count;
            return View(detalle);
        }
        public ActionResult Elimina(int? id = null)
        {
            List<Registro> detalle = (List<Registro>)Session["carrito"];
            foreach (Registro it in detalle)
            {
                if (it.idproducto == id)
                {
                    detalle.Remove(it);
                    break;
                }
            }
            Session["carrito"] = detalle;
            return RedirectToAction("Comprar");
        }
        public ActionResult Pago(int? id = null)
        {
            List<Registro> detalle = (List<Registro>)Session["carrito"];
            List<Registro> detalle1 = (List<Registro>)Session["persona"];
            decimal mt = 0;
            string d = "";
            string n = "";
            foreach (Registro it in detalle)
            {
                mt += it.monto;
              

            }
            foreach (Registro it in detalle1)
            {
           
                d = it.dni;
                n = it.nombre;

            }

            ViewBag.doc = d;
            ViewBag.per = n;
            ViewBag.mt = mt;
            ViewBag.contador = detalle.Count;
            return View(detalle);
        }
        [HttpPost]
        public ActionResult Pago(string dni,string nombre,string direccion_actual,string telefono)
        {
            
            
            int id = Autogenerar();
            cn.Open();
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                /*creamos una nueva sesion de la clase pedido*/
                List<Pedido> listapedido = (List<Pedido>)Session["pedido"];


                SqlCommand cmd = new SqlCommand("insert into tb_pedido values(@id,@f,@dni,@nom,@dir,@tel,@monto,@estado)", cn, tr);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@f", SqlDbType.DateTime).Value = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                cmd.Parameters.Add("@dni", SqlDbType.VarChar, 8).Value = dni;
                cmd.Parameters.Add("@nom", SqlDbType.VarChar, 50).Value = nombre;
                cmd.Parameters.Add("@dir", SqlDbType.VarChar, 80).Value = direccion_actual;
                cmd.Parameters.Add("@tel", SqlDbType.VarChar, 12).Value = telefono;
                cmd.Parameters.Add("@monto", SqlDbType.Decimal).Value = Monto();
                cmd.Parameters.Add("@estado", SqlDbType.Int).Value = 1;
                cmd.ExecuteNonQuery();
                // tr.Commit();
                //detalle del pedido
                List<Registro> detalle = (List<Registro>)Session["carrito"];
                foreach (Registro it in detalle)
                {
                    cmd = new SqlCommand("insert into tb_detapedido values(@id,@prod,@pre,@q)", cn, tr);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@prod", SqlDbType.Int).Value = it.idproducto;
                    cmd.Parameters.Add("@pre", SqlDbType.Decimal).Value = it.preciounidad;
                    cmd.Parameters.Add("@q", SqlDbType.Int).Value = it.cantidad;

                    cmd.ExecuteNonQuery();

                }
                tr.Commit();
                

                /*guardamos los datos insertados en una lista de tipo pedido*/
                Pedido nuevopedido = new Pedido();
                nuevopedido.idpedido = id; 
                nuevopedido.nombre = nombre;
                nuevopedido.direccion = direccion_actual;
                nuevopedido.monto = Monto();
                listapedido.Add(nuevopedido);

                /*Sesion pedido Guarda los datos que contiene la lista*/
                Session["pedido"] = listapedido;



            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                tr.Rollback();
            }
            finally
            {
                cn.Close();
            }

            ViewBag.dni = dni;
            ViewBag.nombre = nombre;
            ViewBag.direccion_actual = direccion_actual;
            ViewBag.telefono = telefono;
            
            return RedirectToAction("Reporte");
        }
        int Autogenerar()
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("sp_autogenera", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            int n = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            cn.Close();
            return n;
        }

        decimal Monto()
        {
            List<Registro> detalle = (List<Registro>)Session["carrito"];
            decimal mt = 0;
            foreach (Registro it in detalle)
            {
                mt += it.monto;
            }
            return mt;
        }

      

        IEnumerable<Producto> productos()
        {
            List<Producto> temporal = new List<Producto>();
            cn.Open();

            SqlCommand cmd = new SqlCommand("sp_listadoproducto", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Producto reg = new Producto();
                reg.codigo = dr.GetInt32(0);
                reg.nombre = dr.GetString(1);
                reg.precio = dr.GetDecimal(2);
                reg.descripcion = dr.GetString(3);
                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();
     
            return temporal;
        }
        

        IEnumerable<Producto> articulos()
        {
            List<Producto> temporal = new List<Producto>();
            cn.Open();
            string sql = "select idproducto,nombreproducto,preciounidad from tb_producto order by nombreproducto";
            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Producto reg2 = new Producto();
                reg2.codigo = dr.GetInt32(0);
                reg2.nombre = dr.GetString(1);
                reg2.precio = dr.GetDecimal(2);
                temporal.Add(reg2);
            }
            dr.Close();
            cn.Close();
            return temporal;
        }

   
        public ActionResult Reporte() 
        {

            int id = 0;
            string direccion="", nombre="";
         
            List<Pedido> nuevopedido = (List<Pedido>)Session["pedido"];
            foreach (Pedido obj in nuevopedido)
            {
                id=obj.idpedido;
                nombre=obj.nombre;
                direccion = obj.direccion;
             


            }

            /*Asigno datos de entrada en la vista*/
            ViewBag.idpedido = id;
            ViewBag.nombre = nombre;
            ViewBag.direccion_actual = direccion;
            ViewBag.fecha = DateTime.Today;
            ViewBag.monto = Monto();
            List<Registro> detalle = (List<Registro>)Session["carrito"];
            Session.Clear();
            return View(detalle);
        }







        public ActionResult codigo()
        {
            Pedido reg = listadoPedidos().LastOrDefault();
            /*Asigno datos de entrada en la vista*/
            ViewBag.idpedido = reg.idpedido;
            ViewBag.nombre = reg.nombre;
            ViewBag.direccion_actual = reg.direccion;
            ViewBag.fecha = DateTime.Today;
            ViewBag.monto = reg.monto;

            IEnumerable<Registro> detalle = listadodetallePedidos();
        
            return View(detalle);

        }

        public ActionResult Print()
        {

            return new ActionAsPdf("codigo"){FileName="reporte.pdf"};
        }

        IEnumerable<Pedido> listadoPedidos()
        {
            List<Pedido> temporal = new List<Pedido>();
            cn.Open();
            SqlCommand cmd = new SqlCommand("select * from tb_pedido", cn);
           
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {

                Pedido reg = new Pedido();
                reg.idpedido = dr.GetInt32(0);
                reg.fecha = dr.GetDateTime(1);
                reg.dni = dr.GetString(2);
                reg.nombre = dr.GetString(3);
                reg.direccion = dr.GetString(4);
                reg.telefono = dr.GetString(5);
                reg.monto = dr.GetDecimal(6);


                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();


            return temporal;

        }
        IEnumerable<Registro> listadodetallePedidos()
        {
            List<Registro> temporal = new List<Registro>();
            cn.Open();
            string sql = " select p.idproducto,p.NombreProducto,dp.precio,dp.cantidad from tb_detapedido dp " +
                "inner join tb_producto p on dp.idproducto = p.IdProducto " +
                "where idpedido = (select MAX(idpedido) from tb_detapedido) ";

            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader dr = cmd.ExecuteReader();
         
            while (dr.Read())
            {

                Registro reg = new Registro();
                reg.idproducto = dr.GetInt32(0);
                reg.nombreproducto = dr.GetString(1);
                reg.preciounidad = dr.GetDecimal(2);
                reg.cantidad = dr.GetInt32(3);
                reg.monto= dr.GetInt32(3)*dr.GetDecimal(2); 




                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();


            return temporal;

        }
    }
}
