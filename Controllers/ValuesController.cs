using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Barshop.ModelsApi;

namespace Barshop.Controllers
{

    
    public class ValuesController : ApiController
    {

        // GET api/values
        ReniecEntities bd = new ReniecEntities();

        // GET api/values/5
        public IEnumerable<Persona> Get()
        {
            return bd.Persona.ToList();
        }

        public Persona Get(string id)
        {
            return bd.Persona.ToList().Where(c => c.dni == id).FirstOrDefault();

        }

        // POST api/values
        public void Post(Persona reg) // PARA AGREGAR PERSONAS
        {
            try
            {
                Persona obj = new Persona();
                obj.dni = reg.dni;
                obj.nombre = reg.nombre;
                obj.direccion = reg.direccion;

                bd.Persona.Add(obj);
                bd.SaveChanges();


            }
            catch (Exception) { }

        }

        // PUT api/values/5
        public void Put(Persona reg)
        {
            bd.Entry(reg).State = System.Data.Entity.EntityState.Modified;    
        }

        // DELETE api/values/5
        public void Delete(string id)
        {
            Persona obj = new Persona();
            bd.Persona.Remove(obj);
            bd.SaveChanges();
        }


    }
}
