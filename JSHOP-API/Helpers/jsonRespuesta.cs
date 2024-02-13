namespace Api_ChatN.Helpers
{
    public enum  Estado { Exito=200, Mal=400, NoEncontrado=404, ErrorInterno=500, UsuarioExistente = 401};
    public class jsonRespuesta
    {
        public Estado estado { get; set; }
        public string Mensaje { get; set; }
    }
}
