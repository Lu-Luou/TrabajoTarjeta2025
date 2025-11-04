    namespace Tarjeta.Clases
{
    public class BEG : Tarjeta
    {
        // Hacer públicos para que los tests puedan inspeccionarlos/modificarlos
        public int ViajesGratisHoy { get; set; }
        public DateTime FechaUltimoConteo { get; set; }
        private const int MAX_VIAJES_GRATIS = 2;

        public BEG(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial)
        {
            Tipo = "BEG";
            ViajesGratisHoy = 0;
            // Usar el sistema de tiempo de la tarjeta para ser compatible con TiempoFalso en tests
            FechaUltimoConteo = this.Tiempo.Now().Date;
        }

        public decimal CobrarViaje(decimal tarifa)
        {
            // Usar el sistema de tiempo de la tarjeta para comparar días
            DateTime hoy = this.Tiempo.Now().Date;
            if (FechaUltimoConteo.Date != hoy)
            {
                ViajesGratisHoy = 0;
                FechaUltimoConteo = hoy;
            }

            decimal monto = tarifa;
            if (ViajesGratisHoy < MAX_VIAJES_GRATIS)
            {
                monto = 0; // Viaje gratuito
                ViajesGratisHoy++;
            }
            else
            {
                // Usar DescontarSaldo para respetar el límite negativo
                bool pudo = DescontarSaldo(monto);
                if (!pudo)
                {
                    // Indicar fallo mediante un valor especial
                    return -1m;
                }
            }

            return monto;
        }

        public override bool PagarBoleto(decimal monto)
        {
            decimal cobrado = CobrarViaje(monto);
            if (cobrado < 0) // indicación de fallo
                return false;
            // Si cobrado == 0 (viaje gratis) o >0 (ya se descontó en CobrarViaje), consideramos la operación exitosa
            return true;
        }
    }
}