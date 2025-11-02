namespace Tarjeta.Clases
{
    public class Tarjeta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoPendiente { get; set; } = 0;
        private const decimal LIMITE_SALDO = 56000;
        private const decimal LIMITE_NEGATIVO = -1200;

        public string Tipo { get; set; }

        // Registro de boletos emitidos
        protected List<Boleto> boletos = new List<Boleto>();

        public Tarjeta(string numero, decimal saldoInicial = 0)
        {
            Numero = numero;
            Saldo = saldoInicial;
            Tipo = "Normal";
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

            if (!DescontarSaldo(monto))
                return null; // No se pudo pagar (saldo insuficiente)

            var boleto = new Boleto(
                tipoTarjeta: Tipo,
                linea: colectivo.Linea,
                totalAbonado: monto,
                saldoRestante: Saldo,
                idTarjeta: Numero
            );

            boletos.Add(boleto);
            return boleto;
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

        public void AcreditarCarga(decimal monto)
        {
            if (Saldo + monto > LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - Saldo;
                Saldo += espacioDisponible;
                SaldoPendiente += (monto - espacioDisponible);
            }
            else
            {
                Saldo += monto;
            }
        }

        public override string ToString()
        {
            return $"Tarjeta Nº: {Numero}, Saldo: ${Saldo:F2}";
        }
    }
}
