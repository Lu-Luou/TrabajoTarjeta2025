using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class BEGTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string numero = "123456";
            decimal saldoInicial = 1000;

            // Act
            var tarjeta = new BEG(numero, saldoInicial);

            // Assert
            Assert.AreEqual(numero, tarjeta.Numero);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
            Assert.AreEqual("BEG", tarjeta.Tipo);
            Assert.AreEqual(0, tarjeta.ViajesGratisHoy);
        }

        [Test]
        public void CobrarViaje_PrimerViaje_Gratis()
        {
            // Arrange
            var tarjeta = new BEG("123", 1000);
            decimal tarifa = 700;

            // Act
            decimal montoCargado = tarjeta.CobrarViaje(tarifa);

            // Assert
            Assert.AreEqual(0, montoCargado); // Viaje gratis
            Assert.AreEqual(1000, tarjeta.Saldo); // Saldo no cambia
            Assert.AreEqual(1, tarjeta.ViajesGratisHoy);
        }

        [Test]
        public void CobrarViaje_SegundoViaje_Gratis()
        {
            // Arrange
            var tarjeta = new BEG("123", 1000);
            decimal tarifa = 700;

            // Act
            tarjeta.CobrarViaje(tarifa); // Primer viaje
            decimal montoCargado = tarjeta.CobrarViaje(tarifa); // Segundo viaje

            // Assert
            Assert.AreEqual(0, montoCargado); // Segundo viaje también gratis
            Assert.AreEqual(1000, tarjeta.Saldo); // Saldo no cambia
            Assert.AreEqual(2, tarjeta.ViajesGratisHoy);
        }

        [Test]
        public void CobrarViaje_TercerViaje_CobraTarifaCompleta()
        {
            // Arrange
            var tarjeta = new BEG("123", 1000);
            decimal tarifa = 700;

            // Act
            tarjeta.CobrarViaje(tarifa); // Primer viaje - gratis
            tarjeta.CobrarViaje(tarifa); // Segundo viaje - gratis
            decimal montoCargado = tarjeta.CobrarViaje(tarifa); // Tercer viaje - paga

            // Assert
            Assert.AreEqual(700, montoCargado); // Cobra tarifa completa
            Assert.AreEqual(300, tarjeta.Saldo); // 1000 - 700
            Assert.AreEqual(2, tarjeta.ViajesGratisHoy); // No incrementa más allá de 2
        }

        [Test]
        public void CobrarViaje_NuevoDia_ReseteaContadorViajesGratis()
        {
            // Arrange
            var tarjeta = new BEG("123", 2000);
            decimal tarifa = 700;
            var fechaAyer = DateTime.Today.AddDays(-1);

            // Simular que el último conteo fue ayer
            tarjeta.FechaUltimoConteo = fechaAyer;

            // Act
            decimal montoCargado = tarjeta.CobrarViaje(tarifa);

            // Assert
            Assert.AreEqual(0, montoCargado); // Viaje gratis (contador reseteado)
            Assert.AreEqual(1, tarjeta.ViajesGratisHoy);
            Assert.AreEqual(DateTime.Today, tarjeta.FechaUltimoConteo);
        }

        [Test]
        public void PagarBoleto_UsaCobrarViaje_Internamente()
        {
            // Arrange
            var tarjeta = new BEG("123", 1000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));
            tarjeta.Tiempo = tiempoFalso;

            // Act
            bool resultado = tarjeta.PagarBoleto(700);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(1000, tarjeta.Saldo); // No se descuenta (viaje gratis)
        }

        [Test]
        public void Colectivo_PagarConBEG_PrimerosDosViajes_Gratis()
        {
            // Arrange
            var tarjeta = new BEG("123", 1000);
            var colectivo = new Colectivo("102", "Empresa A");

            // Act - Primer viaje
            var boleto1 = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(0, boleto1.TotalAbonado); // Boleto gratuito
            Assert.AreEqual(1000, boleto1.SaldoRestante); // Saldo no cambia
        }

        [Test]
        public void Colectivo_PagarConBEG_TercerViaje_CobraTarifa()
        {
            // Arrange
            var tarjeta = new BEG("123", 1000);
            var colectivo = new Colectivo("102", "Empresa A");

            // Act - Tres viajes
            colectivo.PagarCon(tarjeta); // Viaje 1 - gratis
            colectivo.PagarCon(tarjeta); // Viaje 2 - gratis
            var boleto3 = colectivo.PagarCon(tarjeta); // Viaje 3 - paga

            // Assert
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(700, boleto3.TotalAbonado); // Cobra tarifa completa
            Assert.AreEqual(300, boleto3.SaldoRestante);
        }
    }
}
