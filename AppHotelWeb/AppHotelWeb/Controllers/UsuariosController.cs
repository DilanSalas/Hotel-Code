using AppHotelWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AppHotelWeb.Controllers
{
    public class UsuariosController : Controller
    {
       

        //Para establecer la relacion con la API
        private HttpClient client;

        //Variables para el manejo de la API de info del cliente
        private HttpClient clientInfoCliente = null;
        private ConexionAPI conexionAPI = null;
      
        public UsuariosController()
        {
           conexionAPI = new ConexionAPI();
            client = conexionAPI.Iniciar();


        }
        //se crea para que nos devuelva la lista de usuarios

        public async Task<IActionResult> Index()
        {
            // Se inicializa una lista vacía de Usuario
            List<Usuario> listaUsuarios = new List<Usuario>();

            // Se llama a la API externa de forma asíncrona
            HttpResponseMessage httpResponse = await client.GetAsync("/Usuarios/Listado");

            // Se verifica si la respuesta fue exitosa
            if (httpResponse.IsSuccessStatusCode)
            {
                // Se lee la respuesta JSON
                var resultado = await httpResponse.Content.ReadAsStringAsync();

                // Se deserializa el JSON en una lista de objetos Usuario
                listaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(resultado);

                // Se filtran los usuarios que sean clientes
                listaUsuarios = listaUsuarios.Where(u => u.Rol == "cliente").ToList();
            }

            // Se devuelve la vista con la lista de objetos Usuario
            return View(listaUsuarios);
        }


        //Métodos post y get de create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind] Usuario user)
        {
            user.id = 0;

            if (user.Rol == "Administrador")
            {
                // Condicional para que al registrarse solo los administradores sepan de esta contraseña
                // IMPORTANTE VER ESTA CONTRASEÑA PARA PODER SER ADMINISTRADOR
                if (user.clave != "AdminPassword")
                {
                    TempData["Mensaje"] = "Contraseña incorrecta para administradores";
                    return View(user);
                }
            }

            var subir = await client.PutAsJsonAsync<Usuario>("/Usuarios/Agregar", user);

            if (subir.IsSuccessStatusCode)
            {
                if (user.Rol == "Administrador")
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                TempData["Mensaje"] = "No se logró almacenar el usuario";
                return View(user);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = new Usuario();

            HttpResponseMessage requestMessage = await client.GetAsync($"/Usuarios/Buscar/{id}");

            if (requestMessage.IsSuccessStatusCode)
            {
                var resultado = requestMessage.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            return View(user);
        }//Fin metodo


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            Usuario user = new Usuario();


            HttpResponseMessage responseMessage = await client.GetAsync($"/Usuarios/Buscar/{id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                var resultado = responseMessage.Content.ReadAsStringAsync().Result;

                user = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            //se envia el libro al front end
            return View(user);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
       
            HttpResponseMessage responseMessage = await client.DeleteAsync($"Usuarios/Eliminar/{id}");



            return RedirectToAction("Index");
        }//Fin metodo 

        /// <summary>
        /// Metodo para extraer la informacion del cliente desde la API
        /// </summary>
        /// <param name="pCedula"></param>
   






        public IActionResult Login()
        {
            return View();
        }//Fin metod 





        /// <summary>
        /// Metodo para validar el correo y contraseña del usuario
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] Usuario user)
        {
            // Utiliza el método para validar los datos del usuario
            var temp = await ValidarUsuario(user);

            // Verifica si hay datos
            if (temp != null)
            {
                // Se crea una lista de claims para el usuario autenticado
                var userClaims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, temp.Cedula.ToString()),
                    new Claim(ClaimTypes.Role, temp.Rol)
                };

                // Se crea una identidad con los claims
                var claimsIdentity = new ClaimsIdentity(userClaims, "User Identity");

                // Se crea el principal con la identidad
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Se realiza la autenticación dentro del contexto HTTP
                await HttpContext.SignInAsync(userPrincipal);

                // Se redirige al usuario a la página principal
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Se indica al usuario que los datos son incorrectos
                TempData["Mensaje"] = "Error, la cédula o contraseña son incorrectos...";

                // Se deja al usuario dentro del formulario del login
                return View(user);
            }
        }

        private async Task<Usuario> ValidarUsuario(Usuario temp)
        {
            Usuario autorizado = null; // Variable para almacenar los datos del usuario

            // Se busca al usuario por medio de la cédula en la API
            HttpResponseMessage response = await client.GetAsync($"/Usuarios/Buscar/{temp.Cedula.ToString()}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Usuario>(resultado);

                if (user != null)
                {
                    if (user.clave != null && user.clave.Equals(temp.clave))
                    {
                        autorizado = user; // Se indica que está autorizado
                    }
                }
            }

            // Se retorna los datos del usuario autorizado
            return autorizado;
        }


        public async Task<IActionResult> Logout()
        {
            // Aqui se realiza el cierre de sesion
            await HttpContext.SignOutAsync();

            // Se ubica al usuario en la pagina de inicio
            return RedirectToAction("Index", "Home");
        }



    }//Fin class
}//Fin namespace
