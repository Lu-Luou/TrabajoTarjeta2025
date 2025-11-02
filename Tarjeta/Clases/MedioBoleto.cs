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

        public override Boleto? PagarBoleto(Colectivo colectivo, DateTime fechaHora)
        {
            DateTime ahora = fechaHora;

            // Verificar si pasaron 5 minutos desde el último viaje
            if (UltimoViaje.HasValue)
            {
                TimeSpan tiempoTranscurrido = ahora - UltimoViaje.Value;
                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return null; // No se pudo pagar (no pasaron 5 minutos)
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
            decimal monto = colectivo.Precio;
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
            if (!DescontarSaldo(montoCobrar))
                return null; // No se pudo pagar (saldo insuficiente)

            var boleto = new Boleto(
                tipoTarjeta: Tipo,
                linea: colectivo.Linea,
                totalAbonado: montoCobrar,
                saldoRestante: Saldo,
                idTarjeta: Numero
            );

            boletos.Add(boleto);

            UltimoViaje = ahora;
            if (ViajesConDescuentoHoy < MAX_VIAJES_CON_DESCUENTO_POR_DIA)
            {
                ViajesConDescuentoHoy++;
            }

            return boleto;
        }
    }
}
