namespace Tarjeta.Clases
{
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        public BoletoGratuitoEstudiantil(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial)
        {
            Tipo = "BEG";
        }

        public override bool PagarBoleto(decimal monto)
        {
            // No se descuenta nada
            return true;
        }
    }
}
