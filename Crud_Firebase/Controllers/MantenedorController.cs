using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

using Crud_Firebase.Models; //referencia a la carpeta donde estan los modelos (entidades)

namespace Crud_Firebase.Controllers
{
    public class MantenedorController : Controller
    {

        IFirebaseClient clientFB; //Crear cliente

        //Primer metodo que se ejecuta al momento de utilizar la clase de "MantenedorController" 
       public MantenedorController()
        {
            //Configuracion del cliente
            IFirebaseConfig configuracion = new FirebaseConfig
            {
                //llave secreta Firebase
                AuthSecret = "4kVzU8nnJYmdEuzorXJnzMkfz4kSNwIA0wAmtLkz",
                //Ruta (url) Firebase
                BasePath = "https://bdfirebase-ad9ed-default-rtdb.firebaseio.com/"
            };

            clientFB = new FirebaseClient(configuracion); //se guarde la configuracion de Firebase
        }

        // GET: Mantenedor
        public ActionResult Index()
        {
            //Crear diccionario
            Dictionary<string, Contacto> _dicContacto = new Dictionary<string, Contacto>();

            FirebaseResponse respuesta = clientFB.Get("contactos"); //Traer del cliente todos los contactos existentes

            if (respuesta.StatusCode == System.Net.HttpStatusCode.OK)
                //Leer atributo Body donde estan todos los resultados para asi almacenarlos en la lista de tipo diccionario
                _dicContacto = JsonConvert.DeserializeObject<Dictionary<string, Contacto>>(respuesta.Body);

            //Devolver lista a la vista
            List<Contacto> _listContacto = new List<Contacto>();

            //Recorrer cada uno de los item del diccionario "_dicContacto"
            //Almacenarlo en "_listContacto"
            foreach (KeyValuePair<string, Contacto> elemento in _dicContacto)
            {
                _listContacto.Add(new Contacto()
                {
                    //Agregando las propiedades
                    idContacto = elemento.Key,
                    Nombre = elemento.Value.Nombre,
                    Correo = elemento.Value.Correo,
                    Telefono = elemento.Value.Telefono
                });
            }
            return View(_listContacto);
        }

        //GET: Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: Create
        [HttpPost]
        public ActionResult Create(Contacto oContacto)
        {
            string idGenerado = Guid.NewGuid().ToString("N"); //Guid: Retorna  llaves aleatorias que nunca se repiten (servicio de C#)
            //Crear atributo "contactos" y se concatena con el ID y le pasamos todo el objeto de "oContacto Nombre, Correo, Telefono)"
            SetResponse respuesta = clientFB.Set("contactos/" + idGenerado, oContacto); //set: ingresar o crear 

            //Validar
            if(respuesta.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Mantenedor");
            }
            else
            {
                return View();
            }
            
        }

        //GET: Edit
        public ActionResult Edit(string idContactoo)
        {
            FirebaseResponse respuesta = clientFB.Get("contactos/" + idContactoo); //Traer el contacto en especifico que se desea
            //Convertir automaticamente lo que tiene en su contenido "<Contacto>" en un objeto de nuestra clase "Contacto"
            Contacto oContacto = respuesta.ResultAs<Contacto>();
            //Editar la propiedad "idContacto" con el "idContac" del metodo de "Editar"
            oContacto.idContacto = idContactoo;

            return View(oContacto);
        }

        //POST: Edit
        [HttpPost]
        public ActionResult Edit(Contacto oContac)
        {
            string idContac = oContac.idContacto;
            oContac.idContacto = null; //idContacto es null para que asi no se envie el "idContacto"

            FirebaseResponse respuesta = clientFB.Update("contactos/" + idContac, oContac);

            //Validar
            if (respuesta.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Mantenedor");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Delete(string idContactoo)
        {
            FirebaseResponse respuesta = clientFB.Delete("contactos/" + idContactoo);
            return RedirectToAction("Index", "Mantenedor");
        }
    }
}