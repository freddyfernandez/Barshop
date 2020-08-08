using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Barshop.Models;

namespace Barshop.Controllers
{
    public class PedidoController : Controller
    {

        //conexion
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx"].ConnectionString);

        List<Pedido> temporal = new List<Pedido>();

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
                reg.cod_estado = dr.GetInt32(7);
                


                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();


            return temporal;

        }

        //listado de los tipo de estado de pedido
        IEnumerable<Estado> ListadoEstado()
        {
            List<Estado> Lista = new List<Estado>();
            cn.Open();
            SqlCommand cmd = new SqlCommand("select * from estado", cn);
            
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {

                Estado obj = new Estado();//el objeto categoria puede acceder a toda la memoria de la clase
                obj.idestado = dr.GetInt32(0);
                obj.estado_actual = dr.GetString(1);//= dr[""nombreCategoria].ToString();
                Lista.Add(obj);

            }
            cn.Close();
            dr.Close();

            return Lista;
        }

        public ActionResult Paginacion(string filtro = "", int pagactual = 1)
        {
           
            cn.Open();
            SqlCommand cmd = new SqlCommand("select p.idpedido,p.fecpedido,p.nomcli,p.diractual,p.telefono,p.monto,e.estado" +
                " from tb_pedido p inner join estado e on p.idestado = e.idestado where CAST(p.idpedido AS NVARCHAR) like '%" + filtro + "%' order by idpedido ", cn);
           
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {

                Pedido reg = new Pedido();
                reg.idpedido = dr.GetInt32(0);
                reg.fecha = dr.GetDateTime(1);
             
                reg.nombre = dr.GetString(2);
                reg.direccion = dr.GetString(3);
                reg.telefono = dr.GetString(4);
                reg.monto = dr.GetDecimal(5);
                reg.estado = dr.GetString(6);

                temporal.Add(reg);
            }
          
            cn.Close();
            //paginacion:mostrar el numero de fila por pagina
            int filas = 3;
            //definir paginas
            int registros = temporal.Count();
            int pags = registros % filas > 0 ? registros / filas + 1 : registros / filas;
            //si el numero de filas es par entonces muestra la mitad de registros

            //almacenar numero de paginas
            ViewBag.pags = pags;
            ViewBag.filtro = filtro;
            return View(temporal.Skip((pagactual - 1) * filas).Take(filas));
           

        }

        //Action result para editar 
        public ActionResult Edit(int id)
        {

            Pedido reg = listadoPedidos().Where(x => x.idpedido == id).FirstOrDefault();

            //De la clases paises y vendedor  Se evalua sus respectivas id de pais
            ViewBag.estado = new SelectList(ListadoEstado(), "idestado", "estado_actual", reg.cod_estado);


            return View(reg);

        }

        [HttpPost]
        public ActionResult Edit(Pedido reg)
        {
            List<SqlParameter> lista = new List<SqlParameter>()
            {
                //variables del procedimiento almacenado
                new SqlParameter()
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Value = reg.idpedido,
                },

                new SqlParameter()
                 {
                     ParameterName = "@dni",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.dni,
                 },

            
                  new SqlParameter()
                 {
                     ParameterName = "@est",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.cod_estado,
                 }



            };

            //ejecutar
            ViewBag.mensaje = CRUD("sp_actualizar_pedido", lista);

            //la pagina sera refrescada; reenvia la lista de paises y el pais elegido
            ViewBag.estado = new SelectList(ListadoEstado(), "idestado", "estado_actual", reg.cod_estado);

            return View(reg);
        }


        /*PROCESO CRUD: PARA INSTANCIAR PROCEDURES CREATE ,EDIT, DELETE*/
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