namespace Tarjeta.Clases
{
    public class MedioBoleto : Tarjeta
    {
        private DateTime? UltimoViaje { get; set; }
        private DateTime? FechaInicioConteo { get; set; }
        private int ViajesConDescuentoHoy { get; set; }
        private const int MINUTOS_ENTRE_VIAJES = 5;
        private const int MAX_VIAJES_CON_DESCUENTO_POR_DIA = 2;

        public MedioBoleto(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial) 
        {
            UltimoViaje = null;
            FechaInicioConteo = null;
            ViajesConDescuentoHoy = 0;
        }

        public override bool PagarBoleto(decimal monto)
        {
            DateTime ahora = DateTime.Now;

            // Verificar si pasaron 5 minutos desde el último viaje
            if (UltimoViaje.HasValue)
            {
                TimeSpan tiempoTranscurrido = ahora - UltimoViaje.Value;
                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return false; // No han pasado 5 minutos
                }
            }

            // Verificar si es un nuevo día y resetear el contador
            if (FechaInicioConteo.HasValue && FechaInicioConteo.Value.Date != ahora.Date)
            {
                ViajesConDescuentoHoy = 0;
                FechaInicioConteo = ahora;
            }
            else if (!FechaInicioConteo.HasValue)
            {
                FechaInicioConteo = ahora;
            }

            // Determinar el monto a cobrar
            decimal montoCobrar;
            if (ViajesConDescuentoHoy < MAX_VIAJES_CON_DESCUENTO_POR_DIA)
            {
                // Aplicar descuento (mitad del precio)
                montoCobrar = monto / 2;
            }
            else
            {
                // Cobrar precio completo (tercer viaje del día en adelante)
                montoCobrar = monto;
            }

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
