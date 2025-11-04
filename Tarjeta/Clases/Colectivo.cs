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

        // Constructor de conveniencia usado por algunos tests (solo línea)
        public Colectivo(string linea)
        {
            Linea = linea;
            Empresa = string.Empty;
            EsInterurbana = false;
            Precio = TARIFA_URBANA;
        }

        public Boleto? PagarCon(Tarjeta tarjeta)
        {
            // Usar la lógica específica por tipo de tarjeta sin pre-aplicar descuentos
            decimal precioActual = Precio;

            // BEG: usar su propio método para contabilizar viajes gratuitos
            if (tarjeta is BEG beg)
            {
                decimal cobrado = beg.CobrarViaje(precioActual);
                var boletoBeg = new Boleto(Linea, cobrado)
                {
                    SaldoRestante = tarjeta.Saldo,
                    TipoTarjeta = tarjeta.Tipo,
                };
                return boletoBeg;
            }

            // MedioBoleto: dejar que la propia clase aplique el descuento,
            // pero necesitamos conocer cuánto se cobró para el boleto.
            if (tarjeta is MedioBoleto mb)
            {
                decimal montoEsperado = mb.CalcularTarifa(precioActual);
                bool ok = tarjeta.PagarBoleto(precioActual); // pasar precio completo para evitar aplicar descuento dos veces
                if (!ok) return null;

                var boleto = new Boleto(Linea, montoEsperado)
                {
                    SaldoRestante = tarjeta.Saldo,
                    TipoTarjeta = tarjeta.Tipo,
                };
                return boleto;
            }

            // Tarjeta normal o franquicia completa: pagar con el precio actual
            bool resultado = tarjeta.PagarBoleto(precioActual);
            if (!resultado) return null;

            var boletoNormal = new Boleto(Linea, precioActual)
            {
                SaldoRestante = tarjeta.Saldo,
                TipoTarjeta = tarjeta.Tipo,
            };
            return boletoNormal;
        }

        // Sobrecarga usada por tests: pagar con monto explícito
        public Boleto? PagarCon(Tarjeta tarjeta, decimal monto)
        {
            // Para tarjeta normal: no permitir si saldo < monto
            if (tarjeta.GetType() == typeof(Tarjeta) && tarjeta.Saldo < monto)
                return null;

            // BEG tiene su propia lógica (primeros dos viajes gratis)
            if (tarjeta is BEG beg)
            {
                decimal cobrado = beg.CobrarViaje(monto);
                var boletoBeg = new Boleto(Linea, cobrado)
                {
                    SaldoRestante = tarjeta.Saldo,
                    TipoTarjeta = tarjeta.Tipo,
                };
                return boletoBeg;
            }

            // MedioBoleto calcula la tarifa mediante CalcularTarifa, pero su propio
            // método PagarBoleto aplica esa lógica internamente. Para evitar
            // aplicar el descuento doble, llamamos a PagarBoleto(monto) y usamos
            // CalcularTarifa para conocer cuánto se cobró.
            if (tarjeta is MedioBoleto mb)
            {
                decimal montoEsperado = mb.CalcularTarifa(monto);
                bool ok = tarjeta.PagarBoleto(monto);
                if (!ok) return null;

                var boleto = new Boleto(Linea, montoEsperado)
                {
                    SaldoRestante = tarjeta.Saldo,
                    TipoTarjeta = tarjeta.Tipo,
                };
                return boleto;
            }

            // Franquicias completas u otras tarjetas: intentar pagar directamente
            bool resultado = tarjeta.PagarBoleto(monto);
            if (!resultado) return null;

            var boletoNormal = new Boleto(Linea, monto)
            {
                SaldoRestante = tarjeta.Saldo,
                TipoTarjeta = tarjeta.Tipo,
            };
            return boletoNormal;
        }

        public override string ToString()
        {
            return $"Línea: {Linea} ({(EsInterurbana ? "Interurbana" : "Urbana")})";
        }
    }
}
