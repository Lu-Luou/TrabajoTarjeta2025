namespace Tarjeta.Clases
{
    public interface IFranquicia
    {
        bool PuedeUsarse(DateTime fecha);
        decimal CalcularTarifa(decimal tarifa);
    }
}