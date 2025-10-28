using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class MedioBoletoTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string numero = "123456";
            decimal saldoInicial = 2000;

            // Act
            var tarjeta = new MedioBoleto(numero, saldoInicial);

            // Assert
            Assert.AreEqual(numero, tarjeta.Numero);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_PagaMitadDelMonto()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(1210, tarjeta.Saldo); // 2000 - 790 (mitad de 1580)
        }

        [Test]
        public void PagarBoleto_SaldoInsuficienteParaMitad_RetornaFalse()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 500);
            decimal monto = 1580; // Necesita 790, solo tiene 500

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(500, tarjeta.Saldo); // No cambia
        }

        [Test]
        public void PagarBoleto_ConSaldoJustoParaMitad_Exitoso()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 790);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(0, tarjeta.Saldo); // 790 - 790 = 0
        }

        [Test]
        public void Colectivo_PagarConMedioBoleto_GeneraBoletoConMitadDelMonto()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580;

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(790, boleto.Monto); // Mitad del monto
            Assert.AreEqual(1210, tarjeta.Saldo);
        }

        [Test]
        public void Colectivo_PagarConMedioBoleto_SaldoInsuficiente_ReturnNull()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 500);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580; // Necesita 790

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNull(boleto);
            Assert.AreEqual(500, tarjeta.Saldo); // No cambia
        }
    }
}
