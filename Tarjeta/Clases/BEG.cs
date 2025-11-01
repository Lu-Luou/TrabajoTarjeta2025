    namespace Tarjeta.Clases
{
    public class BEG : Tarjeta
    {
        private int ViajesGratisHoy { get; set; }
        private DateTime FechaUltimoConteo { get; set; }
        private const int MAX_VIAJES_GRATIS = 2;

        public BEG(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial)
        {
            Tipo = "BEG";
            ViajesGratisHoy = 0;
            FechaUltimoConteo = DateTime.Today;
        }

        public decimal CobrarViaje(decimal tarifa)
        {
            if (FechaUltimoConteo.Date != DateTime.Today)
            {
                ViajesGratisHoy = 0;
                FechaUltimoConteo = DateTime.Today;
            }

            decimal monto = tarifa;
            if (ViajesGratisHoy < MAX_VIAJES_GRATIS)
            {
                monto = 0; // Viaje gratuito
                ViajesGratisHoy++;
            }
            else
            {
                Saldo -= monto;
            }

            return monto;
        }
    }
}