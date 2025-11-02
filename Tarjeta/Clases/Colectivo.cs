namespace Tarjeta.Clases
{
    public class Colectivo
    {
        public string Linea { get; set; }
        public string Empresa { get; set; }
        public bool EsInterurbana { get; set; }
        public decimal Precio { get; set; }

        private const decimal TARIFA_URBANA = 700;
        private const decimal TARIFA_INTERURBANA = 3000;

        public Colectivo(string linea, string empresa, bool esInterurbana = false)
        {
            Linea = linea;
            Empresa = empresa;
            EsInterurbana = esInterurbana;
            Precio = esInterurbana ? TARIFA_INTERURBANA : TARIFA_URBANA;
        }

        public Boleto? PagarCon(Tarjeta tarjeta)
        {
            // Usar el método PagarBoleto que puede ser sobrescrito por subclases
            return tarjeta.PagarBoleto(this, DateTime.Now);
            // Calcular monto real según el tipo de tarjeta
            decimal monto = Precio;

            if (tarjeta is MedioBoleto)
                monto /= 2;
            else if (tarjeta is BEG)
                monto = 0;

            // Intentar pagar el boleto
            if (tarjeta.PagarBoleto(monto))
            {
                return new Boleto(
                    tarjeta.Tipo,   // Tipo de tarjeta
                    Linea,          // Línea del colectivo
                    monto,          // Total abonado
                    tarjeta.Saldo,  // Saldo restante
                    tarjeta.Numero  // ID de la tarjeta
                );
            }

            // Si no hay saldo suficiente
            return null;
        }

        public override string ToString()
        {
            return $"Línea: {Linea} ({(EsInterurbana ? "Interurbana" : "Urbana")})";
        }
    }
}
