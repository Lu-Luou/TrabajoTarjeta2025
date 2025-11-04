namespace Tarjeta.Clases
{
    public class Tarjeta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoPendiente { get; set; } = 0;
    private const decimal LIMITE_SALDO = 56000;
    private const decimal LIMITE_NEGATIVO = -1200;
    // Límite usado para recargas (iteración 1 tests)
    private const decimal LIMITE_RECARGA = 40000;
    private const decimal TARIFA_BASE = 700;

        public string Tipo { get; set; } = "Normal";

        private DateTime UltimoPago = DateTime.MinValue;
        private string? UltimaLinea = null;

        public IFranquicia? Franquicia { get; set; }

        // Sistema de tiempo (por defecto usa tiempo real)
        public Tiempo Tiempo { get; set; } = new TiempoReal();

        // Registro de boletos emitidos
        protected List<Boleto> boletos = new List<Boleto>();

        public Tarjeta(string numero, decimal saldoInicial = 0)
        {
            Numero = numero;
            Saldo = saldoInicial;
        }

        public bool Recargar(decimal monto)
        {
            decimal[] cargasAceptadas = new decimal[] { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

            if (!Array.Exists(cargasAceptadas, x => x == monto))
                return false;
            // Para la operación Recargar (tests de iteración 1) hay un límite distinto
            if (Saldo + monto > LIMITE_RECARGA)
                return false;

            Saldo += monto;
            return true;
        }

        public bool DescontarSaldo(decimal monto)
        {
            if (Saldo - monto < LIMITE_NEGATIVO)
                return false;

            Saldo -= monto;
            return true;
        }

        // Nuevo método principal: pagar boleto (considerando trasbordo)
        public virtual Boleto? PagarBoleto(Colectivo colectivo, DateTime fechaHora)
        {
            bool esTrasbordo = EsTrasbordoValido(colectivo, fechaHora);

            decimal monto = esTrasbordo ? 0 : colectivo.Precio;

            // Para tarjeta normal (no derivadas), no permitir que pase a saldo negativo al pagar con colectivo
            if (Tipo == "Normal" && Saldo < monto)
                return null;

                // 4. Dejar que la implementación concreta de la tarjeta aplique sus reglas
                // (MedioBoleto, BEG, etc.) respetando el DateTime proporcionado.
                if (!PagarBoleto(monto, fechaHora))
                    return null;

                var boleto = new Boleto(
                    tipoTarjeta: Tipo,
                    linea: colectivo.Linea,
                    totalAbonado: monto,
                    saldoRestante: Saldo,
                    idTarjeta: Numero,
                    esTrasbordo: esTrasbordo,
                    fechaHora: fechaHora
                );

                boletos.Add(boleto);
                return boleto;
        }
        /// <summary>
        /// Aplica el pago del boleto considerando el tipo de franquicia y restricciones horarias.
        /// </summary>
        public virtual bool PagarBoleto(decimal monto)
        {
            return PagarBoleto(monto, Tiempo.Now());
        }

        /// <summary>
        /// Aplica el pago del boleto considerando el tipo de franquicia y restricciones horarias.
        /// </summary>
        public virtual bool PagarBoleto(decimal monto, DateTime fecha)
        {
            // Validar si aplica restricción horaria
            if (EsFranquiciaConHorario(Tipo))
            {
                if (!EsHorarioValido(fecha))
                {
                    throw new InvalidOperationException(
                        "No se puede usar esta franquicia fuera del horario permitido (lunes a viernes de 6 a 22)."
                    );
                }
            }

            // Aplicar descuentos según tipo de franquicia
            monto = CalcularMontoFinal(monto, Tipo);

            // Intentar descontar el saldo
            if (!DescontarSaldo(monto))
                return false;

            // Si había saldo pendiente, se acredita lo que entra hasta el límite
            if (SaldoPendiente > 0 && Saldo < LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - Saldo;
                decimal acreditado = Math.Min(espacioDisponible, SaldoPendiente);

                // Se acredita un poco del pendiente
                Saldo += acreditado;
                SaldoPendiente -= acreditado;
            }

            return true;
        }

        // Determina si el viaje actual es trasbordo según las reglas
        private bool EsTrasbordoValido(Colectivo nuevoColectivo, DateTime fechaHora)
        {
            var ultimo = boletos.LastOrDefault();
            if (ultimo == null) return false;

            // Condición 1: Dentro de una hora desde el último viaje
            TimeSpan diferencia = fechaHora - ultimo.FechaHora;
            bool dentroDeUnaHora = diferencia.TotalMinutes <= 60;

            // Condición 2: Línea distinta
            bool distintaLinea = nuevoColectivo.Linea != ultimo.Linea;

            // Condición 3: Días y horarios válidos (lunes a sábado, 7 a 22)
            bool diaHabil = fechaHora.DayOfWeek != DayOfWeek.Sunday;
            bool dentroHorario = fechaHora.Hour >= 7 && fechaHora.Hour < 22;

            return dentroDeUnaHora && distintaLinea && diaHabil && dentroHorario;
        }

        private static bool EsFranquiciaConHorario(string tipo)
        {
            // Solo estas franquicias tienen restricción horaria
            return tipo == "MedioBoleto" ||
                   tipo == "MedioBoletoUniversitario" ||
                   tipo == "BEG" ||
                   tipo == "FranquiciaCompleta";
        }

        private static bool EsHorarioValido(DateTime fecha)
        {
            bool horario = fecha.Hour >= 6 && fecha.Hour < 22;
            bool diaHabil = fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday;
            return horario && diaHabil;
        }

        private static decimal CalcularMontoFinal(decimal monto, string tipo)
        {
            // No aplicar descuentos globales aquí; cada franquicia/derivado debe
            // implementar su propia lógica (por ejemplo BEG o MedioBoleto).
            return monto;
        }

        public void AcreditarCarga(decimal monto)
        {
            // Si existe saldo pendiente, usar el monto para reducir el pendiente
            if (SaldoPendiente > 0)
            {
                // Aplicar hasta el monto disponible para cubrir el pendiente
                decimal aplicado = Math.Min(monto, SaldoPendiente);

                // No superar el límite de saldo al acreditar
                decimal espacioDisponible = LIMITE_SALDO - Saldo;
                decimal aAgregar = Math.Min(aplicado, espacioDisponible);

                Saldo += aAgregar;
                SaldoPendiente -= aplicado;

                return;
            }

            // Si no hay pendiente, acreditar normalmente respetando el límite
            if (Saldo + monto > LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - Saldo;
                Saldo += espacioDisponible;
                // El excedente queda como pendiente
                SaldoPendiente += (monto - espacioDisponible);
            }
            else
            {
                Saldo += monto;
            }
        }

        private decimal CalcularTarifaUsoFrecuente(Usuario usuario, decimal tarifa)
        {
            int viajes = usuario.ViajesEsteMes;

            if (viajes < 30)
                return tarifa;

            if (viajes < 60)
                return tarifa * 0.80m;

            if (viajes < 80)
                return tarifa * 0.75m;

            return tarifa;
        }
         private bool PuedeTrasbordo(Colectivo cole, DateTime fecha)
        {
            if (UltimaLinea == null) return false;
            if (UltimaLinea == cole.Linea) return false;

            // domingo no aplica
            if (fecha.DayOfWeek == DayOfWeek.Sunday) return false;

            // horario 7 a 22
            if (!(fecha.TimeOfDay >= new TimeSpan(7, 0, 0) &&
                  fecha.TimeOfDay <= new TimeSpan(22, 0, 0)))
                return false;

            // dentro de una hora
            return (fecha - UltimoPago).TotalMinutes <= 60;
        }
        public Boleto PagarBoleto(Usuario usuario, Colectivo cole, DateTime fecha)
        {
            decimal tarifa = TARIFA_BASE;

            // 1. Franquicia
            if (Franquicia != null)
            {
                if (!Franquicia.PuedeUsarse(fecha))
                    throw new Exception("La franquicia no puede usarse en este horario.");

                tarifa = Franquicia.CalcularTarifa(tarifa);
            }
            else
            {
                // 2. Uso frecuente (solo tarjeta normal)
                if (Tipo == "Normal")
                    tarifa = CalcularTarifaUsoFrecuente(usuario, tarifa);
            }

            // 3. Trasbordo
            if (PuedeTrasbordo(cole, fecha))
            {
                tarifa = 0;
            }

            // 4. Descontar
            if (!DescontarSaldo(tarifa))
                throw new Exception("Saldo insuficiente.");

            // 5. Registrar viaje
            usuario.RegistrarViaje(fecha);

            // 6. Actualizar estado de trasbordo
            UltimoPago = fecha;
            UltimaLinea = cole.Linea;

            // 7. Crear boleto (incluir la fecha proporcionada)
            Boleto boleto = new Boleto(Tipo, cole.Linea, tarifa, Saldo, Numero, esTrasbordo: false, fechaHora: fecha);
            usuario.HistorialBoletos.Add(boleto);

            return boleto;
        }


        public override string ToString()
        {
            // Algunos tests esperan el formato sin el campo Tipo
            return $"Tarjeta Nº: {Numero}, Saldo: ${Saldo:F2}";
        }
    }
}
