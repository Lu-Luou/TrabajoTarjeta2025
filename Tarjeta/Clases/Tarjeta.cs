namespace Tarjeta.Clases
{
   public class Tarjeta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoPendiente { get; set; } = 0;
        private const decimal LIMITE_SALDO = 56000;
        private const decimal LIMITE_NEGATIVO = -1200;
        private const decimal TARIFA_BASE = 1580;

        public string Tipo { get; set; } = "Normal";

        private DateTime UltimoPago = DateTime.MinValue;
        private string? UltimaLinea = null;

        public IFranquicia? Franquicia { get; set; }

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

        private decimal CalcularTarifaUsoFrecuente(Usuario usuario, decimal tarifa)
        {
            int viajes = usuario.ViajesEsteMes;

            if (viajes < 30)
                return tarifa;

            if (viajes < 60)
                return tarifa * 0.80m;

            if (viajes < 80)
                return tarifa * 0.75m;

            return tarifa;
        }
         private bool PuedeTrasbordo(Colectivo cole, DateTime fecha)
        {
            if (UltimaLinea == null) return false;
            if (UltimaLinea == cole.Linea) return false;

            // domingo no aplica
            if (fecha.DayOfWeek == DayOfWeek.Sunday) return false;

            // horario 7 a 22
            if (!(fecha.TimeOfDay >= new TimeSpan(7, 0, 0) &&
                  fecha.TimeOfDay <= new TimeSpan(22, 0, 0)))
                return false;

            // dentro de una hora
            return (fecha - UltimoPago).TotalMinutes <= 60;
        }
        public Boleto PagarBoleto(Usuario usuario, Colectivo cole, DateTime fecha)
        {
            decimal tarifa = TARIFA_BASE;

            // 1. Franquicia
            if (Franquicia != null)
            {
                if (!Franquicia.PuedeUsarse(fecha))
                    throw new Exception("La franquicia no puede usarse en este horario.");

                tarifa = Franquicia.CalcularTarifa(tarifa);
            }
            else
            {
                // 2. Uso frecuente (solo tarjeta normal)
                if (Tipo == "Normal")
                    tarifa = CalcularTarifaUsoFrecuente(usuario, tarifa);
            }

            // 3. Trasbordo
            if (PuedeTrasbordo(cole, fecha))
            {
                tarifa = 0;
            }

            // 4. Descontar
            if (!DescontarSaldo(tarifa))
                throw new Exception("Saldo insuficiente.");

            // 5. Registrar viaje
            usuario.RegistrarViaje(fecha);

            // 6. Actualizar estado de trasbordo
            UltimoPago = fecha;
            UltimaLinea = cole.Linea;

            // 7. Crear boleto
            Boleto boleto = new Boleto(Tipo, cole.Linea, tarifa, Saldo, Numero);
            usuario.HistorialBoletos.Add(boleto);

            return boleto;
        }


        public override string ToString()
        {
            return $"Tarjeta Nº: {Numero}, Saldo: ${Saldo:F2}";
        }
    }
}