namespace Tarjeta.Clases
{
   public class Tarjeta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoPendiente { get; set; } = 0;
        private const decimal LIMITE_SALDO = 56000;
        private const decimal LIMITE_NEGATIVO = -1200;

        public Tarjeta(string numero, decimal saldoInicial = 0)
        {
            Numero = numero;
            Saldo = saldoInicial;
        }

        public bool Recargar(decimal monto)
        {
             decimal[] cargasAceptadas = [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];
            
            if (!Array.Exists(cargasAceptadas, x => x == monto))
                 return false;

            if (Saldo + monto > LIMITE_SALDO)
                return false;

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

        public virtual bool PagarBoleto(decimal monto)
        {
            // Primero intentamos descontar el monto normalmente
             if (!DescontarSaldo(monto))
                return false;

            // Si había saldo pendiente, se acredita lo que entra hasta el límite
            if (SaldoPendiente > 0 && Saldo < LIMITE_SALDO)
                {
                    decimal espacioDisponible = LIMITE_SALDO - Saldo;
                    decimal acreditado = Math.Min(espacioDisponible, SaldoPendiente);

            // Se acredita un poco del pendiente
            Saldo += acreditado;
            SaldoPendiente -= acreditado;
        }

            return true;
        }

        public void AcreditarCarga(decimal monto)
        {
            // Si al sumar el monto se supera el límite
            if (Saldo + monto > LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - Saldo;

                // Se acredita solo lo que entra
                Saldo += espacioDisponible;

                 // Y el excedente queda como pendiente
                SaldoPendiente += (monto - espacioDisponible);
            }
            else
            {
                // Todo entra sin problemas
                Saldo += monto;
            }
        }

        public override string ToString()
        {
            return $"Tarjeta Nº: {Numero}, Saldo: ${Saldo:F2}";
        }
    }
}