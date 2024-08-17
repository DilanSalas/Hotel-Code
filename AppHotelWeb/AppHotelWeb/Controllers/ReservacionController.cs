using AppHotelWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppHotelWeb.Controllers
{
    public class ReservacionController : Controller
    {
        private ConexionAPI conexionAPI;
        private HttpClient client;

        public ReservacionController()
        {
            // Instancia de la API
            conexionAPI = new ConexionAPI();
            client = conexionAPI.Iniciar();
        }

        public async Task<IActionResult> Index()
        {
            // Aquí se va almacenar la lista de reservas que vamos a traer de la API
            List<Reservacion> reservas = new List<Reservacion>();

            // Usamos el método del listado de la API
            HttpResponseMessage httpResponse = await client.GetAsync("/Reservaciones/Listado");

            // Verificamos el resultado
            if (httpResponse.IsSuccessStatusCode)
            {
                // Leemos los datos en forma de JSON
                var resultado = await httpResponse.Content.ReadAsStringAsync();

                // Lo convertimos en el objeto que necesitamos
                reservas = JsonConvert.DeserializeObject<List<Reservacion>>(resultado);
            }

            return View(reservas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateR([Bind] Reservacion pReservacion)
        {
            if (pReservacion != null)
            {
                // Verificar si el usuario existe en la API
                //var usuarioExiste = await client.GetAsync($"/Usuarios/Buscar/{pReservacion.idUsuario}");

                if (pReservacion.tipoPago != "Cheque") {
                    // Si el usuario existe, proceder con la creación de la reserva
                    pReservacion.Id = 0;
                    pReservacion.EmailCliente = "String";
                    pReservacion.NombreCliente = "String";
                    pReservacion.IdCheque = 0;
                    pReservacion.Banco = "ND";
                    pReservacion.Costo = 0;
                    pReservacion.Descuento = 0;
                    pReservacion.DescuentoCol = 0;
                    pReservacion.Impuesto = 0;
                    pReservacion.ImpuestoCol = 0;
                    pReservacion.Mensualidad = 0;
                    pReservacion.MontoTotal = 0;
                    pReservacion.MontoTotalCol = 0;
                    pReservacion.Prima = 0;
                }
                else
                {
                    pReservacion.Id = 0;
                    pReservacion.EmailCliente = "String";
                    pReservacion.NombreCliente = "String";
                    pReservacion.Costo = 0;
                    pReservacion.Descuento = 0;
                    pReservacion.DescuentoCol = 0;
                    pReservacion.Impuesto = 0;
                    pReservacion.ImpuestoCol = 0;
                    pReservacion.Mensualidad = 0;
                    pReservacion.MontoTotal = 0;
                    pReservacion.MontoTotalCol = 0;
                    pReservacion.Prima = 0;

                }



                var subirReserva = client.PutAsJsonAsync<Reservacion>("/Reservaciones/Agregar", pReservacion);

                await subirReserva;

                var result = subirReserva.Result;

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home"); // Si la ejecución fue correcta, nos lleva al home
                }
                else
                {

                    TempData["Mensaje"] = "Error de conexión (API)"; // Si no es correcta, nos envía de nuevo el formulario con el mensaje para que lo llenemos de nuevo
                    return View(pReservacion);
                }
            }
            else
            {
                return View();
            }
        } // Fin método

        [HttpGet]
        public IActionResult CreateR()
        {
            
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            HttpResponseMessage httpResponse = await client.DeleteAsync($"/Reservaciones/Eliminar/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Reservacion");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
          
            Reservacion reservacion = new Reservacion();

            HttpResponseMessage httpResponse = await client.GetAsync($"/Reservaciones/Buscar/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var resultado = await httpResponse.Content.ReadAsStringAsync();

                reservacion = JsonConvert.DeserializeObject<Reservacion>(resultado);

                return View(reservacion);
            }
            else
            {
                TempData["Mensaje"] = "* La reserva no existe o problemas de red";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Reservacion reservacion = new Reservacion();
           
            HttpResponseMessage httpResponse = await client.GetAsync($"/Reservaciones/Buscar/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var resultado = await httpResponse.Content.ReadAsStringAsync();

                reservacion = JsonConvert.DeserializeObject<Reservacion>(resultado);

                return View(reservacion);
            }
            else
            {
                TempData["Mensaje"] = "* La reserva no existe o problemas de red";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([Bind] Reservacion pReservacion)
        {
            if (pReservacion != null)
            {
             
             
                


                HttpResponseMessage httpResponse = await client.PostAsJsonAsync($"/Reservaciones/Modificar/", pReservacion);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Reservacion");
                }
                else
                {
                    TempData["Mensaje"] = "* No se logró modificar la reserva";
                    return View(pReservacion);
                }
            }
            else
            {
                TempData["Mensaje"] = "* Por favor ingrese los datos";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            Reservacion reservacion = new Reservacion();

            HttpResponseMessage httpResponse = await client.GetAsync($"/Reservaciones/Buscar/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var resultado = await httpResponse.Content.ReadAsStringAsync();

                reservacion = JsonConvert.DeserializeObject<Reservacion>(resultado);

                return View(reservacion);
            }
            else
            {
                TempData["Mensaje"] = "* La reserva no existe o problemas de red";
                return View();
            }
        }
    }
}
