namespace AppHotelWeb.Models
{
    public class ConexionAPI
    {
        public HttpClient Iniciar()
        {
            //Variable para manejar el objeto cliente 
            var client = new HttpClient();

            //Se indica la URL del dominio donde esta publicada la API 
            client.BaseAddress = new Uri("http://ApiProyecto10.somee.com");

            return client;

        }//Fin metodo 
    }
}
