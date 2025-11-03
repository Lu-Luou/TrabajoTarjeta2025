namespace Tarjeta.Clases
{
    public class TiempoFalso : Tiempo
    {
        private DateTime tiempo;

        public TiempoFalso()
        {
            // Crea un objeto DateTime con la fecha 14/10/2024 (Lunes) con la hora en 00:00:00
            tiempo = new DateTime(2024, 10, 14);
        }

        public TiempoFalso(DateTime fechaInicial)
        {
            tiempo = fechaInicial;
        }

        public override DateTime Now()
        {
            return tiempo;
        }

        public void AgregarDias(int cantidad)
        {
            tiempo = tiempo.AddDays(cantidad);
        }

        public void AgregarMinutos(int cantidad)
        {
            tiempo = tiempo.AddMinutes(cantidad);
        }

        public void AgregarHoras(int cantidad)
        {
            tiempo = tiempo.AddHours(cantidad);
        }

        public void EstablecerFecha(DateTime nuevaFecha)
        {
            tiempo = nuevaFecha;
        }
    }
}
