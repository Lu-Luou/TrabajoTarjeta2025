namespace Tarjeta.Clases
{
    public class FranquiciaCompleta : Tarjeta, IFranquicia
    {
        public FranquiciaCompleta(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial)
        {
            Tipo = "FranquiciaCompleta";
        }
        
        // No cambia nada, paga el monto completo
        public bool PuedeUsarse(DateTime fecha)
        {
            return true; // Siempre puede usarse
        }

        public decimal CalcularTarifa(decimal tarifa)
        {
            return tarifa; // Paga el monto completo
        }
    }
}
