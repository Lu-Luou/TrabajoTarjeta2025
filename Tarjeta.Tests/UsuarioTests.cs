using NUnit.Framework;
using Tarjeta.Clases;
using TarjetaClase = Tarjeta.Clases.Tarjeta;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class UsuarioTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string nombre = "Juan";
            var tarjeta = new TarjetaClase("123", 1000);
            var historial = new List<Boleto> { new Boleto("Linea 1", 1580) };

            // Act
            var usuario = new Usuario(nombre, tarjeta, historial);

            // Assert
            Assert.AreEqual(nombre, usuario.Nombre);
            Assert.AreEqual(tarjeta, usuario.Tarjeta);
            Assert.AreEqual(historial, usuario.HistorialBoletos);
        }

        [Test]
        public void Constructor_DefaultHistorialIsEmptyList()
        {
            // Arrange
            string nombre = "Juan";
            var tarjeta = new TarjetaClase("123", 1000);

            // Act
            var usuario = new Usuario(nombre, tarjeta);

            // Assert
            Assert.IsNotNull(usuario.HistorialBoletos);
            Assert.AreEqual(0, usuario.HistorialBoletos.Count);
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123456", 1234.56m);
            var usuario = new Usuario("Juan", tarjeta);

            // Act
            string result = usuario.ToString();

            // Assert
            Assert.AreEqual("Usuario: Juan, Tarjeta Nº: 123456, Saldo: $1234.56", result);
        }
    }
}

