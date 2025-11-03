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
        public void PagarBoleto_SaldoInsuficiente_PermiteSaldoNegativo()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 1000);
            decimal monto = 1580;

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado); // Permite saldo negativo
            Assert.AreEqual(-580, tarjeta.Saldo); // Saldo se vuelve negativo
        }

        [Test]
        public void Colectivo_PagarConFranquiciaCompleta_GeneraBoletoConMontoCompleto()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");
            decimal monto = 700;

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(700, boleto.TotalAbonado); // Monto completo
            Assert.AreEqual(1300, tarjeta.Saldo);
        }

        [Test]
        public void PuedeUsarse_SiempreRetornaTrue()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0); // Lunes mañana
            var fechaFinDeSemana = new DateTime(2024, 10, 19, 22, 0, 0); // Sábado noche

            // Act
            bool puedeUsarseDiaSemana = tarjeta.PuedeUsarse(fecha);
            bool puedeUsarseFinSemana = tarjeta.PuedeUsarse(fechaFinDeSemana);

            // Assert
            Assert.IsTrue(puedeUsarseDiaSemana);
            Assert.IsTrue(puedeUsarseFinSemana);
        }

        [Test]
        public void CalcularTarifa_SiempreRetornaTarifaCompleta()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);
            decimal tarifaBase = 700;

            // Act
            decimal tarifaCalculada = tarjeta.CalcularTarifa(tarifaBase);

            // Assert
            Assert.AreEqual(700, tarifaCalculada); // No hay descuento
        }

        [Test]
        public void Tipo_SeteaCorrectamente()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);

            // Assert
            Assert.AreEqual("FranquiciaCompleta", tarjeta.Tipo);
        }

        [Test]
        public void Franquicia_AsignadaCorrectamente()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);

            // Assert
            Assert.IsNotNull(tarjeta.Franquicia);
            Assert.IsInstanceOf<FranquiciaCompleta>(tarjeta.Franquicia);
        }

        [Test]
        public void PagarBoleto_UsaFranquicia_ConColectivo()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            var boleto = tarjeta.PagarBoleto(colectivo, fecha);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("FranquiciaCompleta", boleto.TipoTarjeta);
            Assert.AreEqual(700, boleto.TotalAbonado); // Tarifa completa
            Assert.AreEqual(1300, boleto.SaldoRestante);
        }
    }
}
