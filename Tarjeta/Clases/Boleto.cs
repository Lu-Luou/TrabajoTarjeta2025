namespace Tarjeta.Clases
{
    public class Boleto
    {
        public DateTime FechaHora { get; set; }
        public string TipoTarjeta { get; set; }
        public string Linea { get; set; }
        public decimal TotalAbonado { get; set; }
        // Alias/compatibilidad con tests existentes
        public decimal Monto
        {
            get => TotalAbonado;
            set => TotalAbonado = value;
        }
        public decimal SaldoRestante { get; set; }
        public string IDTarjeta { get; set; }

        // Nuevo campo para marcar trasbordos
        public bool EsTrasbordo { get; set; }

        public Boleto(
            string tipoTarjeta,
            string linea,
            decimal totalAbonado,
            decimal saldoRestante,
            string idTarjeta,
            bool esTrasbordo = false,
            DateTime? fechaHora = null)
        {
            FechaHora = fechaHora ?? DateTime.Now;
            TipoTarjeta = tipoTarjeta;
            Linea = linea;
            TotalAbonado = totalAbonado;
            SaldoRestante = saldoRestante;
            IDTarjeta = idTarjeta;
            EsTrasbordo = esTrasbordo;
        }

        // Constructor de conveniencia usado por tests (línea + monto)
        public Boleto(string linea, decimal monto)
        {
            FechaHora = DateTime.Now;
            TipoTarjeta = string.Empty;
            Linea = linea;
            TotalAbonado = monto;
            SaldoRestante = 0m;
            IDTarjeta = string.Empty;
            EsTrasbordo = false;
        }

        public override string ToString()
        {
            string infoTrasbordo = EsTrasbordo ? " (Trasbordo gratuito)" : "";
            return $"Fecha: {FechaHora}, Tarjeta: {TipoTarjeta}, Línea: {Linea}, Total abonado: {TotalAbonado}, Saldo restante: {SaldoRestante}, ID: {IDTarjeta}{infoTrasbordo}";
        }
    }
}
