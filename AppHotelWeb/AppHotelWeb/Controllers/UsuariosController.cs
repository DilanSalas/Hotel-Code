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
        // Para establecer la relación con la API
        private HttpClient client;

        // Variables para el manejo de la API de info del cliente
        private HttpClient clientInfoCliente = null;
        private ConexionAPI conexionAPI = null;

        public UsuariosController()
        {
            conexionAPI = new ConexionAPI();
            client = conexionAPI.Iniciar();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<Usuario> listaUsuarios = new List<Usuario>();
                HttpResponseMessage httpResponse = await client.GetAsync("/Usuarios/Listado");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var resultado = await httpResponse.Content.ReadAsStringAsync();
                    listaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
                    listaUsuarios = listaUsuarios.Where(u => u.Rol == "cliente").ToList();
                }

                return View(listaUsuarios);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar la lista de usuarios: " + ex.Message;
                return View(new List<Usuario>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar la vista de creación: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] Usuario user)
        {
            try
            {
                user.id = 0;

                if (user.Rol == "Administrador" && user.clave != "AdminPassword")
                {
                    TempData["Mensaje"] = "Contraseña incorrecta para administradores";
                    return View(user);
                }

                var subir = await client.PutAsJsonAsync<Usuario>("/Usuarios/Agregar", user);

                if (subir.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró almacenar el usuario";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al crear el usuario: " + ex.Message;
                return View(user);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = new Usuario();
                HttpResponseMessage requestMessage = await client.GetAsync($"/Usuarios/Buscar/{id}");

                if (requestMessage.IsSuccessStatusCode)
                {
                    var resultado = await requestMessage.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<Usuario>(resultado);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar los detalles del usuario: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                Usuario user = new Usuario();
                HttpResponseMessage responseMessage = await client.GetAsync($"/Usuarios/Buscar/{id}");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<Usuario>(resultado);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar el usuario para eliminar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                HttpResponseMessage responseMessage = await client.DeleteAsync($"Usuarios/Eliminar/{id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al eliminar el usuario: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar la vista de inicio de sesión: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] Usuario user)
        {
            try
            {
                var temp = await ValidarUsuario(user);

                if (temp != null)
                {
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, temp.Cedula.ToString()),
                        new Claim(ClaimTypes.Role, temp.Rol)
                    };

                    var claimsIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(userPrincipal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Mensaje"] = "Error, la cédula o contraseña son incorrectos...";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al iniciar sesión: " + ex.Message;
                return View(user);
            }
        }

        private async Task<Usuario> ValidarUsuario(Usuario temp)
        {
            try
            {
                Usuario autorizado = null;
                HttpResponseMessage response = await client.GetAsync($"/Usuarios/Buscar/{temp.Cedula.ToString()}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(resultado);

                    if (user != null && user.clave != null && user.clave.Equals(temp.clave))
                    {
                        autorizado = user;
                    }
                }

                return autorizado;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cerrar sesión: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
