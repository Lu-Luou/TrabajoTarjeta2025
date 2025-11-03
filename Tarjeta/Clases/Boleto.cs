namespace Tarjeta.Clases
{
    public class Boleto
    {
        public DateTime FechaHora { get; set; }
        public string TipoTarjeta { get; set; }
        public string Linea { get; set; }
        public decimal TotalAbonado { get; set; }
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

        public override string ToString()
        {
            string infoTrasbordo = EsTrasbordo ? " (Trasbordo gratuito)" : "";
            return $"Fecha: {FechaHora}, Tarjeta: {TipoTarjeta}, Línea: {Linea}, Total abonado: {TotalAbonado}, Saldo restante: {SaldoRestante}, ID: {IDTarjeta}{infoTrasbordo}";
        }
    }
}
