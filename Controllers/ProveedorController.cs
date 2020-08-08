using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Barshop.Models;
using Barshop.ServiceEmpresa;

namespace Barshop.Controllers
{
    public class ProveedorController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx"].ConnectionString);


        Service1Client servicio = new Service1Client();


        [HttpGet]
        public ActionResult Index() 
        {
            if (Session["consumo_proveedor"] == null)
            {

                List<Proveedores> detalle = new List<Proveedores>();
                Session["consumo_proveedor"] = detalle;
            }
            return View(listadoProveedores());
        }

        IEnumerable<Proveedores> listadoProveedores()
        {
            List<Proveedores> temporal = new List<Proveedores>();
            cn.Open();
            SqlCommand cmd = new SqlCommand("sp_listadoproveedor", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {

                Proveedores reg = new Proveedores();
                reg.idProveedor = dr.GetInt32(0);
                reg.rucProveedor = dr.GetString(1);
                reg.razSocial = dr.GetString(2);
                reg.direccion = dr.GetString(3);


                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();


            return temporal;

        }



        public ActionResult BuscarProveedor(int?filtro=null)
        {
            return View(servicio.proveedores().Where(x => x.ruc == filtro.ToString()).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult BuscarProveedor(string filtro)
        {
            /*se obtiene un objeto de tipo clase service*/
            Proveedor reg = servicio.proveedores().Where(x => x.ruc == filtro).FirstOrDefault();
            if (reg == null)
            {
                string mensaje = "No existe Ruc consulte la Base de datos Sunat";
                ViewBag.mensaje = mensaje;
                return View();
            }
            /*se obtiene un objeto de tipo clase model*/
            Proveedores it = new Proveedores();

            it.rucProveedor = reg.ruc;
            it.razSocial = reg.nombre;
            it.direccion = reg.direccion;
            List<Proveedores> detalle = (List<Proveedores>)Session["consumo_proveedor"];
            detalle.Add(it);
            Session["consumo_proveedor"] = detalle;

            return RedirectToAction("Create");

          
        
        }

        public ActionResult Create()
        {
            List<Proveedores> detalle = (List<Proveedores>)Session["consumo_proveedor"];

            string ruc = "",razon="",direccion="";
            foreach (Proveedores obj in detalle)
            {
                ruc = obj.rucProveedor;
                razon = obj.razSocial;
                direccion = obj.direccion;
            }
            ViewBag.ruc = ruc;
            ViewBag.rsocial = razon;
            ViewBag.direccion = direccion;

         
            if (!ModelState.IsValid)//si los datos ingresados no son validos retorna a la vista create
            {
               return View(new Proveedores());
            }
            
          
            return View(new Proveedores());

        }


        [HttpPost]
        public ActionResult Create(string rucProveedor ,string razSocial,string direccion)
        {


            List<SqlParameter> lista = new List<SqlParameter>()
            {
                //variables del procedimiento almacenado
            
                new SqlParameter()
                 {
                     ParameterName = "@ruc",
                     SqlDbType = SqlDbType.VarChar,
                     Value = rucProveedor,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@razSocial",
                     SqlDbType = SqlDbType.VarChar,
                     Value = razSocial,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@direccion",
                     SqlDbType = SqlDbType.VarChar,
                     Value = direccion,
                 }
            };
            //ejecutar
            ViewBag.mensaje = CRUD("sp_crear_proveedor", lista);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {

            Proveedores reg = listadoProveedores().Where(x => x.idProveedor == id).FirstOrDefault();

            return View(reg);

        }

        [HttpPost]
        public ActionResult Edit(Proveedores reg)
        {


            List<SqlParameter> lista = new List<SqlParameter>()
            {
                //variables del procedimiento almacenado
                new SqlParameter()
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Value = reg.idProveedor,
                },
                new SqlParameter()
                 {
                     ParameterName = "@ruc",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.rucProveedor,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@razSocial",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.razSocial,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@direccion",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.direccion,
                 }
            };
            //ejecutar
            ViewBag.mensaje = CRUD("sp_actualizar_proveedor", lista);

            return View(reg);
        }
        public ActionResult Delete(int id)
        {
            string mensaje = "";
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("delete from tb_proveedor where idProveedor=@codigo", cn);
                cmd.Parameters.AddWithValue("@codigo", id);


                cmd.ExecuteNonQuery();
                mensaje = "Registro eliminado";

            }
            catch (SqlException ex)
            {
                mensaje = ex.Message;

            }
            finally
            {
                cn.Close();
            }

            return RedirectToAction("Index");
        }


        string CRUD(string proceso, List<SqlParameter> pars)

        {
            string mensaje;
            cn.Open();
            SqlCommand cmd = new SqlCommand(proceso, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(pars.ToArray());
            try
            {
                int n = cmd.ExecuteNonQuery();
                mensaje = n + " registro actualizado";

            }
            catch (SqlException ex)
            {
                mensaje = ex.Message;


            }
            finally
            {
                cn.Close();

            }
            return mensaje;

        }
    }
}