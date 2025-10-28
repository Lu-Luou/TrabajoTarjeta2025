namespace Tarjeta.Clases
{
    public class MedioBoleto : Tarjeta
    {
        public MedioBoleto(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial) {}

        public override bool PagarBoleto(decimal monto)
        {
            decimal mitad = monto / 2;
            return base.PagarBoleto(mitad);
        }
    }
}
