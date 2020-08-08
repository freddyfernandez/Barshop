using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Barshop.Models;
using System.IO; /*libreria de manejo de archivos*/

namespace Barshop.Controllers
{
    public class ProductoController : Controller
    {
       
        //conexion
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx"].ConnectionString);

        List<Producto> productos = new List<Producto>();
        // GET: Producto
        IEnumerable<Producto> listadoProductos()
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
                reg.Foto = dr.GetString(4);
                reg.idcategoria = dr.GetInt32(5);


                temporal.Add(reg);
            }
            dr.Close();
            cn.Close();


            return temporal;

        }

        //listados del nombre de la categoria
        IEnumerable<Categoria> ListadoCategorias()
        {
            List<Categoria> Lista = new List<Categoria>();
            cn.Open();
            SqlCommand cmd = new SqlCommand("sp_listadocategoria", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {

                Categoria obj = new Categoria();//el objeto categoria puede acceder a toda la memoria de la clase
                obj.codigo = dr.GetInt32(0);
                obj.nombre = dr.GetString(1);//= dr[""nombreCategoria].ToString();
                Lista.Add(obj);

            }
            cn.Close();
            dr.Close();

            return Lista;
        }

        public ActionResult Buscar(int filtro=-1) {

        
            

            Producto reg = listadoProductos().Where(x => x.codigo == filtro).FirstOrDefault();
            ViewBag.filtro = filtro;

            return View(reg);

        } 

        public ActionResult Paginacion(string filtro = "", int pagactual = 1)
        {
            cn.Open();
            string sql = "select idproducto,nombreproducto,preciounidad from  tb_producto where nombreproducto like '%" + filtro + "%' order by nombreproducto,idproducto desc";//sintaxis sql nativa
            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {

                Producto obj = new Producto();//el objeto pais puede acceder a toda la memoria de la clase
                obj.codigo = dr.GetInt32(0);
                obj.nombre = dr.GetString(1);
                obj.precio = dr.GetDecimal(2);
                productos.Add(obj);

            }
            cn.Close();

            //paginacion:mostrar el numero de fila por pagina
            int filas = 3;
            //definir paginas
            int registros = productos.Count();
            int pags = registros % filas > 0 ? registros / filas + 1 : registros / filas;
            //si el numero de filas es par entonces muestra la mitad de registros


            //almacenar numero de paginas
            ViewBag.pags = pags;
            ViewBag.filtro = filtro;
            return View(productos.Skip((pagactual - 1) * filas).Take(filas));
        }


        public ActionResult Create()
        {
            if (!ModelState.IsValid)//si los datos ingresados no son validos retorna a la vista create
            {
                return View(new Producto());  
            }
            //si es valido
            //enviar lista de categorias con sus respectivos datos de la clase categoria
            ViewBag.categorias = new SelectList(ListadoCategorias(), "codigo", "nombre");
            return View(new Producto());

        }
        //metodo create de tipo Post: cuando le hago clik en un boton submit; las cajas de texto se trasladan al sql
        [HttpPost]
        public ActionResult Create(Producto reg, HttpPostedFileBase f)
        {

            if (f == null)
            {
                ViewBag.mensaje = "Selecciona Foto";
                return View(reg);

            }
            if (Path.GetExtension(f.FileName) != ".jpg")
            {
                ViewBag.mensaje = "debe ser extension jpg";
                return View(reg);
            }

            List<SqlParameter> lista = new List<SqlParameter>()
            {
                //variables del procedimiento almacenado
             
                new SqlParameter()
                 {
                     ParameterName = "@nom",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.nombre,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@pre",
                     SqlDbType = SqlDbType.Decimal,
                     Value = reg.precio,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@des",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.descripcion
                 },
                 //para que el valor de la foto no sea un texto , se cambia  la ruta de la foto donde se va almacenar
                 new SqlParameter(){
                     ParameterName="@fot",SqlDbType=SqlDbType.VarChar,Value="~/imagenes/"+Path.GetFileName(f.FileName)
                 },



                  new SqlParameter()
                 {
                     ParameterName = "@cat",
                     SqlDbType = SqlDbType.Int,
                     Value = reg.idcategoria
                 }

                




            };

            //ejecutar
            ViewBag.mensaje = CRUD("sp_crear_producto", lista);

            //la pagina sera refrescada; reenvia la lista de paises y elemento seleccionado
            ViewBag.categorias = new SelectList(ListadoCategorias(), "codigo", "nombre", reg.idcategoria);
            f.SaveAs(Path.Combine(Server.MapPath("~/imagenes/"), Path.GetFileName(f.FileName)));
            return View(reg);
        }

        //Action result para editar 
        public ActionResult Edit(int id)
        {
            if (!ModelState.IsValid)//si los datos ingresados no son validos retorna a la vista create
            {
                return View(new Producto());
            }

            Producto reg = listadoProductos().Where(x => x.codigo == id).FirstOrDefault();

            //De la clases paises y vendedor  Se evalua sus respectivas id de pais
            ViewBag.categoria = new SelectList(ListadoCategorias(), "codigo", "nombre",reg.idcategoria);


            return View(reg);

        }

        [HttpPost]
        public ActionResult Edit(Producto reg, HttpPostedFileBase f)
        {

            if (f == null)
            {
                ViewBag.mensaje = "Selecciona Foto";
                return View(reg);

            }
            if (Path.GetExtension(f.FileName) != ".jpg")
            {
                ViewBag.mensaje = "debe ser extension jpg";
                return View(reg);
            }

            List<SqlParameter> lista = new List<SqlParameter>()
            {
                //variables del procedimiento almacenado
                new SqlParameter()
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Value = reg.codigo,
                },
                new SqlParameter()
                 {
                     ParameterName = "@nom",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.nombre,
                 },

                new SqlParameter()
                 {
                     ParameterName = "@pre",
                     SqlDbType = SqlDbType.Decimal,
                     Value = reg.precio,
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@des",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.descripcion,
                 },
                 new SqlParameter(){
                     ParameterName="@fot",
                     SqlDbType=SqlDbType.VarChar,
                     Value="~/imagenes/"+Path.GetFileName(f.FileName),
                 },
                 new SqlParameter()
                 {
                     ParameterName = "@cat",
                     SqlDbType = SqlDbType.VarChar,
                     Value = reg.idcategoria
                 }



            };
            f.SaveAs(Path.Combine(Server.MapPath("~/imagenes/"), Path.GetFileName(f.FileName)));
            //ejecutar
            ViewBag.mensaje = CRUD("sp_actualizar_producto", lista);

            ViewBag.categoria = new SelectList(ListadoCategorias(), "codigo", "nombre",reg.idcategoria);

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