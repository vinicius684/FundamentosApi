namespace ApiFuncional.Models
{
    public class JwtSettings
    {
        public string? Segredo { get; set; }//chave
        public int ExpiracaoHoras { get; set; }
        public string? Emissor { get; set; }//Aplicação que esta emitindo o token
        public string? Audiencia { get;}//onde esse token é válido
    }
}
