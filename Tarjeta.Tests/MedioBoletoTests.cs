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
        public void PagarBoleto_SaldoInsuficienteParaMitad_PermiteSaldoNegativo()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 500);
            decimal monto = 1580; // Necesita 790, solo tiene 500

            // Act
            bool resultado = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(resultado); // Permite saldo negativo
            Assert.AreEqual(-290, tarjeta.Saldo); // Paga 790 (mitad de 1580), queda -290
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
        public void Colectivo_PagarConMedioBoleto_SaldoInsuficiente_GeneraBoleto()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 500);
            var colectivo = new Colectivo("Linea 1");
            decimal monto = 1580; // Necesita 790

            // Act
            var boleto = colectivo.PagarCon(tarjeta, monto);

            // Assert
            Assert.IsNotNull(boleto); // Genera boleto con saldo negativo
            Assert.AreEqual(790, boleto.Monto); // Boleto de medio precio
            Assert.AreEqual(-290, tarjeta.Saldo); // Saldo negativo
        }

        [Test]
        public void PagarBoleto_Antesde5Minutos_NoPermiteViaje()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 5000);
            decimal monto = 1580;

            // Act - Primer viaje
            bool primerViaje = tarjeta.PagarBoleto(monto);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(4210, tarjeta.Saldo); // 5000 - 790

            // Act - Intentar segundo viaje inmediatamente
            bool segundoViaje = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsFalse(segundoViaje); // No debe permitir el viaje
            Assert.AreEqual(4210, tarjeta.Saldo); // El saldo no cambia
        }

        /* NOTA: Los siguientes tests están comentados porque requieren esperar 5 minutos reales.
         * Se pueden descomentar para verificaciones en ambiente real, pero no son prácticos 
         * para ejecución continua en CI/CD. */

        /*
        [Test]
        public void PagarBoleto_Despuesde5Minutos_PermiteViaje()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 5000);
            decimal monto = 1580;

            // Act - Primer viaje
            bool primerViaje = tarjeta.PagarBoleto(monto);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(4210, tarjeta.Saldo); // 5000 - 790

            // Simular que pasaron 5 minutos
            System.Threading.Thread.Sleep(5 * 60 * 1000 + 100); // 5 minutos y 100ms

            // Act - Segundo viaje después de 5 minutos
            bool segundoViaje = tarjeta.PagarBoleto(monto);

            // Assert
            Assert.IsTrue(segundoViaje);
            Assert.AreEqual(3420, tarjeta.Saldo); // 4210 - 790
        }

        [Test]
        public void PagarBoleto_TercerViajedelDia_CobraMontoCompleto()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 10000);
            decimal monto = 1580;

            // Act - Primer viaje (cobra mitad: 790)
            System.Threading.Thread.Sleep(100); // Pequeña pausa
            bool primerViaje = tarjeta.PagarBoleto(monto);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(9210, tarjeta.Saldo); // 10000 - 790

            // Esperar 5 minutos para segundo viaje
            System.Threading.Thread.Sleep(5 * 60 * 1000 + 100);
            
            // Act - Segundo viaje (cobra mitad: 790)
            bool segundoViaje = tarjeta.PagarBoleto(monto);
            Assert.IsTrue(segundoViaje);
            Assert.AreEqual(8420, tarjeta.Saldo); // 9210 - 790

            // Esperar 5 minutos para tercer viaje
            System.Threading.Thread.Sleep(5 * 60 * 1000 + 100);

            // Act - Tercer viaje (cobra completo: 1580)
            bool tercerViaje = tarjeta.PagarBoleto(monto);
            Assert.IsTrue(tercerViaje);
            Assert.AreEqual(6840, tarjeta.Saldo); // 8420 - 1580 (monto completo)
        }

        [Test]
        public void PagarBoleto_MasDeDosViajesPorDia_NoPermiteDescuentoEnTercero()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 10000);
            decimal monto = 1580;
            decimal saldoEsperadoDespuesDos = 10000 - 790 - 790; // Dos viajes con descuento
            decimal saldoEsperadoDespuesTres = saldoEsperadoDespuesDos - 1580; // Tercer viaje sin descuento

            // Act & Assert - Primer viaje
            System.Threading.Thread.Sleep(100);
            Assert.IsTrue(tarjeta.PagarBoleto(monto));
            
            // Esperar 5 minutos
            System.Threading.Thread.Sleep(5 * 60 * 1000 + 100);

            // Act & Assert - Segundo viaje
            Assert.IsTrue(tarjeta.PagarBoleto(monto));
            Assert.AreEqual(saldoEsperadoDespuesDos, tarjeta.Saldo);

            // Esperar 5 minutos
            System.Threading.Thread.Sleep(5 * 60 * 1000 + 100);

            // Act & Assert - Tercer viaje (debe cobrar monto completo)
            Assert.IsTrue(tarjeta.PagarBoleto(monto));
            Assert.AreEqual(saldoEsperadoDespuesTres, tarjeta.Saldo);
        }
        */
    }
}
