using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Barshop.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Barshop.Controllers
{
    public class AdminController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx"].ConnectionString);

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logueo(string usuario, string password)
        {
            string user = "";
            string pass = "";
            IEnumerable<Trabajador> detalle = listadoTrabajadores();
            foreach (Trabajador reg in detalle)
            {
                user = reg.Usuario;
                pass = reg.Password;

                if (user == usuario && pass==password)
                {
                    return RedirectToAction("Index", "Intranet");
                    
                }
            }
            return View();

        }

        IEnumerable<Trabajador> listadoTrabajadores()
        {
            List<Trabajador> temporal = new List<Trabajador>();
            cn.Open();
            SqlCommand cmd = new SqlCommand("sp_listadotrabajador", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Trabajador reg = new Trabajador();
                reg.Codigo = dr.GetInt32(0);
                reg.DNI = dr.GetString(1);
                reg.Nombre = dr.GetString(2);
                reg.Usuario = dr.GetString(3);
                reg.Password = dr.GetString(4);
                reg.Fecha_Contrato = dr.GetDateTime(5);
                reg.Direccion = dr.GetString(6);

                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();

            return temporal;
        }

    }
}