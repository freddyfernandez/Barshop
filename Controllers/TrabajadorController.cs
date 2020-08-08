using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Barshop.Models;

namespace Barshop.Controllers
{
    public class TrabajadorController : Controller
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx"].ConnectionString);

        List<Trabajador> trabajadores = new List<Trabajador>();

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
        public ActionResult Index()
        {
            return View(listadoTrabajadores());
        }
        // GET: Trabajador
        public ActionResult Buscar(int filtro = -1)
        {
            Trabajador reg = listadoTrabajadores().Where(x => x.Codigo == filtro).FirstOrDefault();
            ViewBag.filtro = filtro;

            return View(reg);

        }
        public ActionResult Create()
        {
            return View(new Trabajador());

        }
        [HttpPost]
        public ActionResult Create(Trabajador reg)
        {
            List<SqlParameter> lista = new List<SqlParameter>()
            {
                 new SqlParameter()
                 {
                     ParameterName = "@dni",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.DNI,
                 },
                  new SqlParameter()
                  {
                      ParameterName = "@nom",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Nombre,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@usu",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Usuario,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@pass",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Password,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@fec",
                      SqlDbType = SqlDbType.Date,
                      Value = reg.Fecha_Contrato,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@direc",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Direccion,
                  }
            };
            ViewBag.mensaje = CRUD("sp_crear_trabajador", lista);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Trabajador reg = listadoTrabajadores().Where(x => x.Codigo == id).FirstOrDefault();
            return View(reg);
        }
        [HttpPost]
        public ActionResult Edit(Trabajador reg)
        {
            List<SqlParameter> lista = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Value = reg.Codigo,
                },
                 new SqlParameter()
                 {
                     ParameterName = "@dni",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.DNI,
                 },
                  new SqlParameter()
                  {
                      ParameterName = "@nom",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Nombre,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@usu",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Usuario,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@pass",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Password,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@fec",
                      SqlDbType = SqlDbType.Date,
                      Value = reg.Fecha_Contrato,
                  },
                  new SqlParameter()
                  {
                      ParameterName = "@direc",
                      SqlDbType = SqlDbType.VarChar,
                      Value = reg.Direccion,
                  }
            };
            ViewBag.mensaje = CRUD("sp_actualizar_trabajador", lista);
            return View(reg);
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
                mensaje = n + "registro actualizado";
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