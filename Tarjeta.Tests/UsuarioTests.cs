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

        [Test]
        public void RegistrarViaje_PrimerViaje_InicializaContadores()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");
            var usuario = new Usuario("Juan", tarjeta);
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            usuario.RegistrarViaje(fecha);

            // Assert
            Assert.AreEqual(1, usuario.ViajesEsteMes);
            Assert.AreEqual(fecha, usuario.UltimoViaje);
        }

        [Test]
        public void RegistrarViaje_MismoMes_IncrementaContador()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");
            var usuario = new Usuario("Juan", tarjeta);
            var fecha1 = new DateTime(2024, 10, 14, 8, 0, 0);
            var fecha2 = new DateTime(2024, 10, 15, 9, 0, 0);

            // Act
            usuario.RegistrarViaje(fecha1);
            usuario.RegistrarViaje(fecha2);

            // Assert
            Assert.AreEqual(2, usuario.ViajesEsteMes);
            Assert.AreEqual(fecha2, usuario.UltimoViaje);
        }

        [Test]
        public void RegistrarViaje_CambioDeMes_ReseteaContador()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");
            var usuario = new Usuario("Juan", tarjeta);
            var fechaOctubre = new DateTime(2024, 10, 14, 8, 0, 0);
            var fechaNoviembre = new DateTime(2024, 11, 1, 9, 0, 0);

            // Act
            usuario.RegistrarViaje(fechaOctubre);
            usuario.RegistrarViaje(fechaNoviembre);

            // Assert
            Assert.AreEqual(1, usuario.ViajesEsteMes); // Reseteado a 1
            Assert.AreEqual(fechaNoviembre, usuario.UltimoViaje);
        }

        [Test]
        public void RegistrarViaje_CambioDeAnio_ReseteaContador()
        {
            // Arrange
            var tarjeta = new TarjetaClase("123");
            var usuario = new Usuario("Juan", tarjeta);
            var fecha2024 = new DateTime(2024, 12, 31, 23, 59, 59);
            var fecha2025 = new DateTime(2025, 1, 1, 0, 0, 1);

            // Act
            usuario.RegistrarViaje(fecha2024);
            usuario.RegistrarViaje(fecha2025);

            // Assert
            Assert.AreEqual(1, usuario.ViajesEsteMes); // Reseteado a 1
            Assert.AreEqual(fecha2025, usuario.UltimoViaje);
        }
    }
}

