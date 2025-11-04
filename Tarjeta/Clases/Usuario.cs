namespace Tarjeta.Clases
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public Tarjeta Tarjeta { get; set; }
        public List<Boleto> HistorialBoletos { get; set; }
        public int ViajesEsteMes { get; set; } = 0;
        public DateTime UltimoViaje { get; set; } = DateTime.MinValue;

        public Usuario(string nombre, Tarjeta tarjeta, List<Boleto>? historialBoletos = null)
        {
            Nombre = nombre;
            Tarjeta = tarjeta;
            HistorialBoletos = historialBoletos ?? new List<Boleto>();
        }
        
         public void RegistrarViaje(DateTime fecha)
        {
            if (UltimoViaje.Month != fecha.Month || UltimoViaje.Year != fecha.Year)
                ViajesEsteMes = 0;

            ViajesEsteMes++;
            UltimoViaje = fecha;
        }

        public override string ToString()
        {
            // Formato esperado por los tests: "Usuario: {Nombre}, Tarjeta Nº: {Numero}, Saldo: ${Saldo:F2}"
            return $"Usuario: {Nombre}, Tarjeta Nº: {Tarjeta.Numero}, Saldo: ${Tarjeta.Saldo:F2}";
        }
    }
}