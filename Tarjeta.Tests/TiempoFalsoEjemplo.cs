using Tarjeta.Clases;
using NUnit.Framework;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class TiempoFalsoEjemplo
    {
        [Test]
        public void Ejemplo_UsoTiempoFalso_PruebaBasica()
        {
            // Arrange: Crear tarjeta y configurar tiempo falso
            var tarjeta = new Tarjeta.Clases.Tarjeta("123456", 5000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0)); // Lunes 8:00 AM
            tarjeta.Tiempo = tiempoFalso;

            // Act: Cargar saldo
            tarjeta.Recargar(3000);

            // Assert
            Assert.AreEqual(8000, tarjeta.Saldo);
            Assert.AreEqual(new DateTime(2024, 10, 14, 8, 0, 0), tiempoFalso.Now());
        }

    [Test]
        public void Ejemplo_TiempoFalso_AvanzarMinutos()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));
            var tarjeta = new MedioBoleto("123456", 5000);
            tarjeta.Tiempo = tiempoFalso;
            var colectivo = new Colectivo("102", "Empresa A");

            // Act: Primer viaje
            var boleto1 = tarjeta.PagarBoleto(colectivo, tiempoFalso.Now());

            // Intentar viajar inmediatamente (debería fallar por los 5 minutos)
            var boleto2 = tarjeta.PagarBoleto(colectivo, tiempoFalso.Now());
            Assert.IsNull(boleto2);

            // Avanzar 5 minutos
            tiempoFalso.AgregarMinutos(5);

            // Segundo viaje (ahora debería funcionar)
            var boleto3 = tarjeta.PagarBoleto(colectivo, tiempoFalso.Now());

            // Assert
            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto3);
        }

    [Test]
        public void Ejemplo_TiempoFalso_AvanzarDias()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14)); // Lunes
            var tarjeta = new Tarjeta.Clases.Tarjeta("123456", 5000);
            tarjeta.Tiempo = tiempoFalso;

            // Act: Avanzar al día siguiente
            tiempoFalso.AgregarDias(1);

            // Assert
            Assert.AreEqual(new DateTime(2024, 10, 15), tiempoFalso.Now()); // Martes
        }

    [Test]
        public void Ejemplo_TiempoFalso_EstablecerFechaEspecifica()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso();
            var tarjeta = new Tarjeta.Clases.Tarjeta("123456", 5000);
            tarjeta.Tiempo = tiempoFalso;

            // Act: Establecer una fecha específica
            tiempoFalso.EstablecerFecha(new DateTime(2025, 1, 15, 14, 30, 0));

            // Assert
            Assert.AreEqual(new DateTime(2025, 1, 15, 14, 30, 0), tiempoFalso.Now());
        }

    [Test]
        public void Ejemplo_TiempoReal_vs_TiempoFalso()
        {
            // Con tiempo real (predeterminado)
            var tarjetaReal = new Tarjeta.Clases.Tarjeta("111111", 5000);
            var tiempoReal1 = tarjetaReal.Tiempo.Now();
            System.Threading.Thread.Sleep(100);
            var tiempoReal2 = tarjetaReal.Tiempo.Now();
            Assert.IsTrue(tiempoReal2 > tiempoReal1); // El tiempo avanza automáticamente

            // Con tiempo falso (controlado)
            var tarjetaFalsa = new Tarjeta.Clases.Tarjeta("222222", 5000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));
            tarjetaFalsa.Tiempo = tiempoFalso;
            
            var tiempo1 = tarjetaFalsa.Tiempo.Now();
            System.Threading.Thread.Sleep(100);
            var tiempo2 = tarjetaFalsa.Tiempo.Now();
            Assert.AreEqual(tiempo1, tiempo2); // El tiempo NO avanza hasta que lo indiquemos

            tiempoFalso.AgregarMinutos(10);
            var tiempo3 = tarjetaFalsa.Tiempo.Now();
            Assert.AreEqual(10, (tiempo3 - tiempo2).TotalMinutes); // Ahora sí avanzó 10 minutos
        }
    }
}
