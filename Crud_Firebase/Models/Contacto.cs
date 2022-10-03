using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crud_Firebase.Models
{
    public class Contacto
    {
        public string idContacto { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
}