using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class FranquiciaCompletaTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string numero = "123456";
            decimal saldoInicial = 2000;

            // Act
            var tarjeta = new FranquiciaCompleta(numero, saldoInicial);

            // Assert
            Assert.AreEqual(numero, tarjeta.Numero);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_PagaMontoCompleto()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(420, tarjeta.Saldo); // 2000 - 1580
        }

        [Test]
        public void PagarBoleto_SaldoInsuficiente_RetornaFalse()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 1000);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(1000, tarjeta.Saldo); // No cambia
        }

        [Test]
        public void Colectivo_PagarConFranquiciaCompleta_GeneraBoletoConMontoCompleto()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580;

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.Monto); // Monto completo
            Assert.AreEqual(420, tarjeta.Saldo);
        }
    }
}
