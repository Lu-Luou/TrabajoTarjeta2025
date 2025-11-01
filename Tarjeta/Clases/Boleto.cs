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

         public Boleto(string tipoTarjeta, string linea, decimal totalAbonado, decimal saldoRestante, string idTarjeta)
        {
            FechaHora = DateTime.Now;
            TipoTarjeta = tipoTarjeta;
            Linea = linea;
            TotalAbonado = totalAbonado;
            SaldoRestante = saldoRestante;
            IDTarjeta = idTarjeta;
        }

        public override string ToString()
        {
            return $"Fecha: {FechaHora}, Tarjeta: {TipoTarjeta}, Línea: {LineaColectivo}, Total abonado: {TotalAbonado}, Saldo restante: {SaldoRestante}, ID: {IDTarjeta}";
        }
    }
}