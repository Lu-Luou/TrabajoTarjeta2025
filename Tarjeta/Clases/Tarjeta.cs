namespace Tarjeta.Clases
{
   public class Tarjeta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        private const decimal LIMITE_SALDO = 40000;
        private const decimal LIMITE_NEGATIVO = -1200;

        public Tarjeta(string numero, decimal saldoInicial = 0)
        {
            Numero = numero;
            Saldo = saldoInicial;
        }

        public bool Recargar(decimal monto)
        {
            decimal[] cargasAceptadas = [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];
            
            // Verificar si el monto está en las cargas aceptadas
            if (!Array.Exists(cargasAceptadas, x => x == monto))
            {
                return false;
            }
            
            // Verificar si no se excede del límite
            if (Saldo + monto > LIMITE_SALDO)
            {
                return false;
            }
            
            Saldo += monto;
            return true;
        }

        public bool DescontarSaldo(decimal monto)
        {
            // Permitir saldo negativo hasta el límite establecido
            if (Saldo - monto < LIMITE_NEGATIVO)
            {
                return false;
            }
            
            Saldo -= monto;
            return true;
        }
        public bool PagarBoleto(decimal monto)
        {
            return DescontarSaldo(monto);
        }

        public virtual bool PagarBoleto(decimal monto)
        {
            return DescontarSaldo(monto);
        }

        public override string ToString()
        {
            return $"Tarjeta Nº: {Numero}, Saldo: ${Saldo:F2}";
        }
    }
}