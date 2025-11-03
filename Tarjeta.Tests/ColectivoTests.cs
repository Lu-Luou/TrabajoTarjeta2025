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
            string linea = "102";
            string empresa = "Empresa A";

            // Act
            var colectivo = new Colectivo(linea, empresa);

            // Assert
            Assert.AreEqual(linea, colectivo.Linea);
            Assert.AreEqual(empresa, colectivo.Empresa);
            Assert.IsFalse(colectivo.EsInterurbana);
            Assert.AreEqual(700, colectivo.Precio); // Tarifa urbana por defecto
        }

        [Test]
        public void Constructor_Interurbano_EstableceTarifaCorrecta()
        {
            // Arrange
            string linea = "102";
            string empresa = "Empresa A";

            // Act
            var colectivo = new Colectivo(linea, empresa, true);

            // Assert
            Assert.IsTrue(colectivo.EsInterurbana);
            Assert.AreEqual(3000, colectivo.Precio); // Tarifa interurbana
        }

        [Test]
        public void PagarCon_TarjetaNormal_SaldoSuficiente_CreaBoletoCorrecto()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.AreEqual("102", boleto.Linea);
            Assert.AreEqual(700, boleto.TotalAbonado);
            Assert.AreEqual(1300, boleto.SaldoRestante);
            Assert.AreEqual("123", boleto.IDTarjeta);
        }

        [Test]
        public void PagarCon_TarjetaNormal_SaldoInsuficiente_RetornaNull()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 100); // Menos que la tarifa
            var colectivo = new Colectivo("102", "Empresa A");

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNull(boleto);
            Assert.AreEqual(100, tarjeta.Saldo); // Saldo no cambia
        }

        [Test]
        public void PagarCon_MedioBoleto_AplicaDescuento()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
            Assert.AreEqual(350, boleto.TotalAbonado); // 700 / 2
            Assert.AreEqual(1650, boleto.SaldoRestante);
        }

        [Test]
        public void PagarCon_BEG_PrimerosDosViajes_Gratis()
        {
            // Arrange
            var tarjeta = new BEG("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");

            // Act - Primer viaje gratis
            var boleto1 = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(0, boleto1.TotalAbonado); // Viaje gratis
            Assert.AreEqual(2000, boleto1.SaldoRestante); // Saldo no cambia
        }

        [Test]
        public void PagarCon_BEG_TercerViaje_CobraTarifaCompleta()
        {
            // Arrange
            var tarjeta = new BEG("123", 2000);
            var colectivo = new Colectivo("102", "Empresa A");

            // Act - Tres viajes
            colectivo.PagarCon(tarjeta); // Viaje 1 - gratis
            colectivo.PagarCon(tarjeta); // Viaje 2 - gratis
            var boleto3 = colectivo.PagarCon(tarjeta); // Viaje 3 - paga

            // Assert
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(700, boleto3.TotalAbonado); // Tarifa completa
            Assert.AreEqual(1300, boleto3.SaldoRestante);
        }

        [Test]
        public void PagarCon_Interurbano_CobraTarifaInterurbana()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123", 5000);
            var colectivo = new Colectivo("102", "Empresa A", true); // Interurbano

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000, boleto.TotalAbonado); // Tarifa interurbana
            Assert.AreEqual(2000, boleto.SaldoRestante);
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var colectivo = new Colectivo("102", "Empresa A");

            // Act
            string result = colectivo.ToString();

            // Assert
            Assert.AreEqual("Línea: 102 (Urbana)", result);
        }

        [Test]
        public void ToString_Interurbano_IncluyeTipo()
        {
            // Arrange
            var colectivo = new Colectivo("102", "Empresa A", true);

            // Act
            string result = colectivo.ToString();

            // Assert
            Assert.AreEqual("Línea: 102 (Interurbana)", result);
        }
    }
}

