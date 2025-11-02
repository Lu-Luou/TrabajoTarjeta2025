namespace Tarjeta.Clases
{
    public class Colectivo
    {
        public string Linea { get; set; }
        public decimal Precio { get; set; } = 1580;

        public Colectivo(string linea)
        {
            Linea = linea;
        }

        public Boleto? PagarCon(Tarjeta tarjeta)
        {
            // Usar el método PagarBoleto que puede ser sobrescrito por subclases
            return tarjeta.PagarBoleto(this, DateTime.Now);
        }
        public override string ToString()
        {
            return $"Línea: {Linea}";
        }
    }
}