using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class BoletoTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string linea = "Linea 1";
            decimal monto = 1580;

            // Act
            var boleto = new Boleto(linea, monto);

            // Assert
            Assert.AreEqual(linea, boleto.Linea);
            Assert.AreEqual(monto, boleto.Monto);
            Assert.IsNotNull(boleto.FechaHora);
            Assert.AreEqual(DateTime.Now.Date, boleto.FechaHora.Date); // Approximate
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var boleto = new Boleto("Linea 2", 2000);

            // Act
            string result = boleto.ToString();

            // Assert
            StringAssert.Contains("Boleto - Línea: Linea 2, Monto: $2000.00, Fecha y Hora:", result);
        }
    }
}

