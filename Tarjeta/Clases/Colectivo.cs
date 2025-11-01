namespace Tarjeta.Clases
{
    public class Colectivo
    {
        public string Linea { get; set; }
        public Colectivo(string linea)
        {
            Linea = linea;
        }

        public Boleto? PagarCon(Tarjeta tarjeta, decimal monto = 1580)
        {
            // Usar el método PagarBoleto que puede ser sobrescrito por subclases
            if (tarjeta.PagarBoleto(monto))
            {
                // Determinar el monto real cobrado según el tipo de tarjeta
                decimal montoReal = monto;
                
                if (tarjeta is MedioBoleto)
                {
                    montoReal = monto / 2;
                }
                else if (tarjeta is BEG)
                {
                    montoReal = 0;
                }
                
                return new Boleto(
                    tarjeta.Tipo,         // Tipo de tarjeta
                    Linea,                // Línea del colectivo
                    montoReal,            // Total abonado
                    tarjeta.Saldo,        // Saldo restante
                    tarjeta.Numero        // ID de la tarjeta
                );
            }
            
            // Si no hay saldo suficiente
            return null;
        }
        public override string ToString()
        {
            return $"Línea: {Linea}";
        }
    }
}