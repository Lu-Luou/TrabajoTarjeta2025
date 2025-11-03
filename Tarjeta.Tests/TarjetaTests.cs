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
        }

        [Test]
        public void PagarBoleto_SufficientBalance_ReturnsTrueAndDeductsSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 2000);
            decimal monto = 1580;

            // Act
            bool result = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(420, tarjeta.Saldo); // 2000 - 1580
        }

        [Test]
        public void PagarBoleto_InsufficientBalance_PermiteSaldoNegativo()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 1000);
            decimal monto = 1580;

            // Act
            bool result = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(result); // Permite saldo negativo
            Assert.AreEqual(-580, tarjeta.Saldo); // Saldo se vuelve negativo
        }

        [Test]
        public void PagarBoleto_ExactBalance_ReturnsTrueAndSaldoBecomesZero()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 1580);
            decimal monto = 1580;

            // Act
            bool result = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_ConColectivo_SaldoSuficiente_CreaBoletoCorrecto()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            var boleto = tarjeta.PagarBoleto(colectivo, fecha);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.AreEqual("102", boleto.Linea);
            Assert.AreEqual(700, boleto.TotalAbonado); // Tarifa urbana
            Assert.AreEqual(1300, boleto.SaldoRestante);
            Assert.AreEqual("123", boleto.IDTarjeta);
        }

        [Test]
        public void PagarBoleto_ConColectivo_SaldoInsuficiente_RetornaNull()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 100); // Menos que la tarifa
            var colectivo = new Colectivo("102", "Empresa A");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            var boleto = tarjeta.PagarBoleto(colectivo, fecha);

            // Assert
            Assert.IsNull(boleto);
            Assert.AreEqual(100, tarjeta.Saldo); // Saldo no cambia
        }

        [Test]
        public void PagarBoleto_ConColectivoInterurbano_CobraTarifaCorrecta()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 5000);
            var colectivo = new Colectivo("102", "Empresa A", true); // Interurbano
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            var boleto = tarjeta.PagarBoleto(colectivo, fecha);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000, boleto.TotalAbonado); // Tarifa interurbana
            Assert.AreEqual(2000, boleto.SaldoRestante);
        }

        [Test]
        public void PagarBoleto_ConUsuarioYColeYFecha_CreaBoletoCorrecto()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 2000);
            var usuario = new Usuario("Juan", tarjeta);
            var colectivo = new Colectivo("102", "Empresa A");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            var boleto = tarjeta.PagarBoleto(usuario, colectivo, fecha);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.AreEqual("102", boleto.Linea);
            Assert.AreEqual(700, boleto.TotalAbonado);
            Assert.AreEqual(1300, boleto.SaldoRestante);
            Assert.AreEqual("123", boleto.IDTarjeta);
            Assert.AreEqual(fecha, boleto.FechaHora);
        }

        [Test]
        public void AcreditarCarga_SaldoPendienteMayorQueEspacioDisponible_AcreditaSoloEspacioDisponible()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 55000); // Límite es 56000
            tarjeta.SaldoPendiente = 2000;

            // Act
            tarjeta.AcreditarCarga(1500);

            // Assert
            Assert.AreEqual(56000, tarjeta.Saldo); // Límite alcanzado
            Assert.AreEqual(500, tarjeta.SaldoPendiente); // 2000 - 1500 = 500 pendiente
        }

        [Test]
        public void AcreditarCarga_SaldoPendienteMenorOIgualQueEspacioDisponible_AcreditaTodoPendiente()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 55000);
            tarjeta.SaldoPendiente = 500;

            // Act
            tarjeta.AcreditarCarga(1000);

            // Assert
            Assert.AreEqual(55500, tarjeta.Saldo); // 55000 + 500
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Tiempo_PorDefecto_EsTiempoReal()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");

            // Assert
            Assert.IsInstanceOf<TiempoReal>(tarjeta.Tiempo);
        }

        [Test]
        public void Tiempo_PuedeSerConfigurado()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");
            var tiempoFalso = new TiempoFalso();

            // Act
            tarjeta.Tiempo = tiempoFalso;

            // Assert
            Assert.AreSame(tiempoFalso, tarjeta.Tiempo);
        }

        [Test]
        public void Franquicia_PorDefecto_EsNull()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");

            // Assert
            Assert.IsNull(tarjeta.Franquicia);
        }

        [Test]
        public void Franquicia_PuedeSerAsignada()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");
            var franquicia = new FranquiciaCompleta("123");

            // Act
            tarjeta.Franquicia = franquicia;

            // Assert
            Assert.AreSame(franquicia, tarjeta.Franquicia);
        }
    }
}