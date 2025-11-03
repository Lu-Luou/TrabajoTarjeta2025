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
            string tipoTarjeta = "Normal";
            string linea = "102";
            decimal totalAbonado = 700;
            decimal saldoRestante = 1300;
            string idTarjeta = "123456";

            // Act
            var boleto = new Boleto(tipoTarjeta, linea, totalAbonado, saldoRestante, idTarjeta);

            // Assert
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.Linea);
            Assert.AreEqual(totalAbonado, boleto.TotalAbonado);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(idTarjeta, boleto.IDTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
            Assert.AreEqual(DateTime.Now.Date, boleto.FechaHora.Date);
            Assert.IsFalse(boleto.EsTrasbordo);
        }

        [Test]
        public void Constructor_ConFechaHoraEspecifica_UsaFechaProporcionada()
        {
            // Arrange
            string tipoTarjeta = "Normal";
            string linea = "102";
            decimal totalAbonado = 700;
            decimal saldoRestante = 1300;
            string idTarjeta = "123456";
            var fechaEspecifica = new DateTime(2024, 10, 14, 8, 30, 0);

            // Act
            var boleto = new Boleto(tipoTarjeta, linea, totalAbonado, saldoRestante, idTarjeta, false, fechaEspecifica);

            // Assert
            Assert.AreEqual(fechaEspecifica, boleto.FechaHora);
        }

        [Test]
        public void Constructor_ConEsTrasbordoTrue_MarcaComoTrasbordo()
        {
            // Arrange
            string tipoTarjeta = "Normal";
            string linea = "102";
            decimal totalAbonado = 0; // Trasbordo gratuito
            decimal saldoRestante = 2000;
            string idTarjeta = "123456";

            // Act
            var boleto = new Boleto(tipoTarjeta, linea, totalAbonado, saldoRestante, idTarjeta, true);

            // Assert
            Assert.IsTrue(boleto.EsTrasbordo);
            Assert.AreEqual(0, boleto.TotalAbonado);
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var boleto = new Boleto("Normal", "102", 700, 1300, "123456");

            // Act
            string result = boleto.ToString();

            // Assert
            StringAssert.Contains("Fecha:", result);
            StringAssert.Contains("Tarjeta: Normal", result);
            StringAssert.Contains("Línea: 102", result);
            StringAssert.Contains("Total abonado: 700", result);
            StringAssert.Contains("Saldo restante: 1300", result);
            StringAssert.Contains("ID: 123456", result);
        }

        [Test]
        public void ToString_ConTrasbordo_IncluyeInformacionTrasbordo()
        {
            // Arrange
            var boleto = new Boleto("Normal", "102", 0, 2000, "123456", true);

            // Act
            string result = boleto.ToString();

            // Assert
            StringAssert.Contains("Total abonado: 0", result);
            StringAssert.Contains("Saldo restante: 2000", result);
        }
    }
}

