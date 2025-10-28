using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class BoletoGratuitoEstudiantilTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string numero = "123456";
            decimal saldoInicial = 1000;

            // Act
            var tarjeta = new BoletoGratuitoEstudiantil(numero, saldoInicial);

            // Assert
            Assert.AreEqual(numero, tarjeta.Numero);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_NoDescuentaSaldo()
        {
            // Arrange
            var tarjeta = new BoletoGratuitoEstudiantil("123", 1000);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(1000, tarjeta.Saldo); // No cambia el saldo
        }

        [Test]
        public void PagarBoleto_ConSaldoCero_SiguePermitiendo()
        {
            // Arrange
            var tarjeta = new BoletoGratuitoEstudiantil("123", 0);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(0, tarjeta.Saldo); // Sigue en 0
        }

        [Test]
        public void PagarBoleto_MultiplesViajes_NoDescuentaSaldo()
        {
            // Arrange
            var tarjeta = new BoletoGratuitoEstudiantil("123", 1000);

            // Act & Assert - Primer viaje
            bool resultado1 = tarjeta.PagarBoleto(1580);
            Assert.IsTrue(resultado1);
            Assert.AreEqual(1000, tarjeta.Saldo);

            // Act & Assert - Segundo viaje
            bool resultado2 = tarjeta.PagarBoleto(1580);
            Assert.IsTrue(resultado2);
            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void Colectivo_PagarConBEG_GeneraBoletoGratuito()
        {
            // Arrange
            var tarjeta = new BoletoGratuitoEstudiantil("123", 500);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580;

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.Monto); // Boleto gratuito
            Assert.AreEqual(500, tarjeta.Saldo); // Saldo no cambia
        }

        [Test]
        public void Colectivo_PagarConBEG_SinSaldo_SigueGenerandoBoleto()
        {
            // Arrange
            var tarjeta = new BoletoGratuitoEstudiantil("123", 0);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580;

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.Monto);
            Assert.AreEqual(0, tarjeta.Saldo);
        }
    }
}
