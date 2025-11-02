namespace Tarjeta.Clases
{
    public class MedioBoleto : Tarjeta, IFranquicia
    {
        private DateTime? UltimoViaje { get; set; }
        private DateTime? FechaInicioConteo { get; set; }
        private int ViajesConDescuentoHoy { get; set; }
        private const int MINUTOS_ENTRE_VIAJES = 5;
        private const int MAX_VIAJES_CON_DESCUENTO_POR_DIA = 2;

        public MedioBoleto(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial) 
        {
            Tipo = "MedioBoleto";
            UltimoViaje = null;
            FechaInicioConteo = null;
            ViajesConDescuentoHoy = 0;
        }

        public bool PuedeUsarse(DateTime fecha)
        {
            // Verificar si pasaron 5 minutos desde el último viaje
            if (UltimoViaje.HasValue)
            {
                TimeSpan tiempoTranscurrido = fecha - UltimoViaje.Value;
                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return false; // No han pasado 5 minutos
                }
            }

            // Verificar si es un nuevo día y resetear el contador
            if (FechaInicioConteo.HasValue && FechaInicioConteo.Value.Date != fecha.Date)
            {
                ViajesConDescuentoHoy = 0;
                FechaInicioConteo = fecha;
            }
            else if (!FechaInicioConteo.HasValue)
            {
                FechaInicioConteo = fecha;
            }

            return true; // Puede usarse si pasa las verificaciones
        }

        public decimal CalcularTarifa(decimal tarifa)
        {
            if (ViajesConDescuentoHoy < MAX_VIAJES_CON_DESCUENTO_POR_DIA)
            {
                return tarifa / 2; // Aplicar descuento (mitad del precio)
            }
            else
            {
                return tarifa; // Cobrar precio completo
            }
        }

        public override bool PagarBoleto(decimal monto)
        {
            DateTime ahora = DateTime.Now;

            if (!PuedeUsarse(ahora))
            {
                return false;
            }

            // Determinar el monto a cobrar usando CalcularTarifa
            decimal montoCobrar = CalcularTarifa(monto);

            // Intentar pagar
            bool resultado = base.PagarBoleto(montoCobrar);

            if (resultado)
            {
                UltimoViaje = ahora;
                if (ViajesConDescuentoHoy < MAX_VIAJES_CON_DESCUENTO_POR_DIA)
                {
                    ViajesConDescuentoHoy++;
                }
            }

            return resultado;
        }
    }
}
