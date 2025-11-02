namespace Tarjeta.Clases
{
    public class Tarjeta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoPendiente { get; set; } = 0;
        private const decimal LIMITE_SALDO = 56000;
        private const decimal LIMITE_NEGATIVO = -1200;

        // Tipos posibles: "Normal", "MedioBoleto", "MedioBoletoUniversitario", "BEG", "FranquiciaCompleta"
        public string Tipo { get; set; }

        public Tarjeta(string numero, decimal saldoInicial = 0, string tipo = "Normal")
        {
            Numero = numero;
            Saldo = saldoInicial;
            Tipo = tipo;
        }

        public bool Recargar(decimal monto)
        {
            decimal[] cargasAceptadas = [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];

            if (!Array.Exists(cargasAceptadas, x => x == monto))
                return false;

            if (Saldo + monto > LIMITE_SALDO)
                return false;

            Saldo += monto;
            return true;
        }

        public bool DescontarSaldo(decimal monto)
        {
            // Permitir saldo negativo hasta el límite establecido
            if (Saldo - monto < LIMITE_NEGATIVO)
                return false;

            Saldo -= monto;
            return true;
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
            return tipo switch
            {
                "MedioBoleto" or "MedioBoletoUniversitario" => monto * 0.5m,
                "BEG" or "FranquiciaCompleta" => 0m,
                _ => monto // Normal
            };
        }

        public void AcreditarCarga(decimal monto)
        {
            // Si al sumar el monto se supera el limite
            if (Saldo + monto > LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - Saldo;

                // Se acredita solo lo que entra
                Saldo += espacioDisponible;

                // Y el excedente queda como pendiente
                SaldoPendiente += (monto - espacioDisponible);
            }
            else
            {
                // Todo entra sin problemas
                Saldo += monto;
            }
        }

        public override string ToString()
        {
            return $"Tarjeta Nº: {Numero}, Tipo: {Tipo}, Saldo: ${Saldo:F2}";
        }
    }
}