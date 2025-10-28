using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class ColectivoTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string linea = "Linea 1";

            // Act
            var colectivo = new Colectivo(linea);

            // Assert
            Assert.AreEqual(linea, colectivo.Linea);
        }

        [Test]
        public void PagarCon_SufficientBalance_ReturnsBoletoAndDeductsSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 2000);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580;

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Linea 1", boleto.Linea);
            Assert.AreEqual(monto, boleto.Monto);
            Assert.AreEqual(420, tarjeta.Saldo); // 2000 - 1580
        }

        [Test]
        public void PagarCon_InsufficientBalance_ReturnsNull()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 1000);
            // Arrange - Saldo insuficiente que excede el límite negativo
            var tarjeta = new TarjetaClase("123", 0);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1500; // Excede el límite de -1200

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNull(boleto);
            Assert.AreEqual(0, tarjeta.Saldo); // No change
        }

        [Test]
        public void PagarCon_DefaultMonto()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 2000);
            var colectivo = new Colectivo("Linea 1");

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.Monto);
            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var colectivo = new Colectivo("Linea 2");

            // Act
            string result = colectivo.ToString();

            // Assert
            Assert.AreEqual("Línea: Linea 2", result);
        }

        [Test]
        public void PagarCon_SaldoInsuficientePeroPermiteSaldoNegativo_GeneraBoleto()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 500); // Saldo menor al costo del pasaje
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580;

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Linea 1", boleto.Linea);
            Assert.AreEqual(1580, boleto.Monto);
            Assert.AreEqual(-1080, tarjeta.Saldo); // Saldo negativo permitido
        }

        [Test]
        public void PagarCon_ExcedeLimiteNegativo_NoGeneraBoleto()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 0); // Sin saldo
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1500; // Excedería el límite de -1200

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNull(boleto);
            Assert.AreEqual(0, tarjeta.Saldo); // Saldo no cambia
        }
    }
}

