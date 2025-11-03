using NUnit.Framework;
using Tarjeta.Clases;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class TiempoTests
    {
        [Test]
        public void TiempoReal_Now_RetornaFechaActual()
        {
            // Arrange
            var tiempoReal = new TiempoReal();
            var antes = DateTime.Now;

            // Act
            var resultado = tiempoReal.Now();
            var despues = DateTime.Now;

            // Assert
            Assert.IsTrue(resultado >= antes);
            Assert.IsTrue(resultado <= despues);
        }

        [Test]
        public void TiempoFalso_ConstructorPorDefecto_IniciaEnFechaCorrecta()
        {
            // Arrange & Act
            var tiempoFalso = new TiempoFalso();

            // Assert
            var fechaEsperada = new DateTime(2024, 10, 14, 0, 0, 0);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_ConstructorConFecha_IniciaEnFechaEspecificada()
        {
            // Arrange
            var fechaInicial = new DateTime(2025, 1, 15, 14, 30, 45);

            // Act
            var tiempoFalso = new TiempoFalso(fechaInicial);

            // Assert
            Assert.AreEqual(fechaInicial, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_AgregarDias_AvanzaCorrectamente()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14));

            // Act
            tiempoFalso.AgregarDias(5);

            // Assert
            var fechaEsperada = new DateTime(2024, 10, 19);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_AgregarMinutos_AvanzaCorrectamente()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));

            // Act
            tiempoFalso.AgregarMinutos(90); // 1 hora 30 minutos

            // Assert
            var fechaEsperada = new DateTime(2024, 10, 14, 9, 30, 0);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_AgregarHoras_AvanzaCorrectamente()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));

            // Act
            tiempoFalso.AgregarHoras(3);

            // Assert
            var fechaEsperada = new DateTime(2024, 10, 14, 11, 0, 0);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_EstablecerFecha_CambiaFechaCompletamente()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14));
            var nuevaFecha = new DateTime(2025, 12, 25, 18, 45, 30);

            // Act
            tiempoFalso.EstablecerFecha(nuevaFecha);

            // Assert
            Assert.AreEqual(nuevaFecha, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_MultiplesOperaciones_SeAcumulanCorrectamente()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));

            // Act
            tiempoFalso.AgregarDias(1);
            tiempoFalso.AgregarHoras(2);
            tiempoFalso.AgregarMinutos(30);

            // Assert
            var fechaEsperada = new DateTime(2024, 10, 15, 10, 30, 0);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_AgregarDias_CruceDeMes()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 30));

            // Act
            tiempoFalso.AgregarDias(5);

            // Assert
            var fechaEsperada = new DateTime(2024, 11, 4);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_AgregarDias_CruceDeAnio()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 12, 30));

            // Act
            tiempoFalso.AgregarDias(5);

            // Assert
            var fechaEsperada = new DateTime(2025, 1, 4);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }

        [Test]
        public void TiempoFalso_AgregarMinutos_CruceDeHora()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 23, 50, 0));

            // Act
            tiempoFalso.AgregarMinutos(15);

            // Assert
            var fechaEsperada = new DateTime(2024, 10, 15, 0, 5, 0);
            Assert.AreEqual(fechaEsperada, tiempoFalso.Now());
        }
    }
}
