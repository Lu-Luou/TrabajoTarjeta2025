namespace Tarjeta.Clases
{
    public class TiempoReal : Tiempo
    {
        public override DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
