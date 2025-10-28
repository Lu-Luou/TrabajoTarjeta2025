using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class TarjetaTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string numero = "123456789";
            decimal saldoInicial = 1000;

            // Act
            var tarjeta = new TarjetaClase(numero, saldoInicial);

            // Assert
            Assert.AreEqual(numero, tarjeta.Numero);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void Constructor_DefaultSaldoIsZero()
        {
            // Arrange
            string numero = "123456789";

            // Act
            var tarjeta = new TarjetaClase(numero);

            // Assert
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void Recargar_ValidAmount_IncreasesSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 1000);
            decimal monto = 2000;

            // Act
            tarjeta.Recargar(monto);

            // Assert
            Assert.AreEqual(3000, tarjeta.Saldo);
        }

        [Test]
        public void Recargar_InvalidAmount_DoesNotChangeSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 1000);
            decimal monto = 1500; // Invalid

            // Act
            tarjeta.Recargar(monto);

            // Assert
            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void Recargar_ExceedsLimit_DoesNotChangeSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 38000);
            decimal monto = 3000; // Would exceed 40000

            // Act
            tarjeta.Recargar(monto);

            // Assert
            Assert.AreEqual(38000, tarjeta.Saldo);
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123456", 1234.56m);

            // Act
            string result = tarjeta.ToString();

            // Assert
            Assert.AreEqual("Tarjeta Nº: 123456, Saldo: $1234.56", result);
        }

        [Test]
        public void DescontarSaldo_PermiteSaldoNegativoHastaLimite()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 500);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.DescontarSaldo(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(-1080, tarjeta.Saldo);
        }

        [Test]
        public void DescontarSaldo_NoPermiteExcederLimiteNegativo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 0);
            decimal monto = 1500; // Excedería el límite de -1200

            // Act
            bool resultado = tarjeta.DescontarSaldo(monto);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void DescontarSaldo_EnLimiteExactoNegativo_NoPermiteDescuentoAdicional()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", -1200);
            decimal monto = 100; // Intentar descontar más

            // Act
            bool resultado = tarjeta.DescontarSaldo(monto);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(-1200, tarjeta.Saldo);
        }

        [Test]
        public void Recargar_ConSaldoNegativo_IncrementaCorrectamente()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", -500);
            decimal monto = 2000;

            // Act
            bool resultado = tarjeta.Recargar(monto);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(1500, tarjeta.Saldo);
        }

        [Test]
        public void DescontarSaldo_SegundoViajeConSaldoNegativo_NoPermitido()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 500);
            tarjeta.DescontarSaldo(1580); // Primer viaje: saldo queda en -1080

            // Act
            bool resultado = tarjeta.DescontarSaldo(1580); // Intento de segundo viaje

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(-1080, tarjeta.Saldo); // Saldo no cambia
        public void PagarBoleto_SufficientBalance_ReturnsTrueAndDeductsSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClass("123", 2000);
            decimal monto = 1580;

            // Act
            bool result = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(420, tarjeta.Saldo); // 2000 - 1580
        }

        [Test]
        public void PagarBoleto_InsufficientBalance_ReturnsFalseAndDoesNotChangeSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClass("123", 1000);
            decimal monto = 1580;

            // Act
            bool result = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1000, tarjeta.Saldo); // Saldo no cambia
        }

        [Test]
        public void PagarBoleto_ExactBalance_ReturnsTrueAndSaldoBecomesZero()
        {
            // Arrange
            var tarjeta = new TarjetaClass("123", 1580);
            decimal monto = 1580;

            // Act
            bool result = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, tarjeta.Saldo);
        }
    }
}

