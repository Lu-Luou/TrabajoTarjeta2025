namespace Tarjeta.Clases
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial)
        {
            Tipo = "FranquiciaCompleta";
        }
        
        // No cambia nada, paga el monto completo
    }
}
