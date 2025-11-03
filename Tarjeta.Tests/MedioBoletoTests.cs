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
        */

        [Test]
        public void PuedeUsarse_PrimerViaje_SiemprePermitido()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0);

            // Act
            bool puedeUsarse = tarjeta.PuedeUsarse(fecha);

            // Assert
            Assert.IsTrue(puedeUsarse);
        }

        [Test]
        public void PuedeUsarse_ViajeInmediatoDespues_NoPermitido()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));
            tarjeta.Tiempo = tiempoFalso;

            // Primer viaje
            tarjeta.PagarBoleto(700);

            // Act - Intentar viajar inmediatamente
            bool puedeUsarse = tarjeta.PuedeUsarse(tiempoFalso.Now());

            // Assert
            Assert.IsFalse(puedeUsarse);
        }

        [Test]
        public void PuedeUsarse_ViajeDespuesDe5Minutos_Permitido()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));
            tarjeta.Tiempo = tiempoFalso;

            // Primer viaje
            tarjeta.PagarBoleto(700);

            // Avanzar 5 minutos
            tiempoFalso.AgregarMinutos(5);

            // Act
            bool puedeUsarse = tarjeta.PuedeUsarse(tiempoFalso.Now());

            // Assert
            Assert.IsTrue(puedeUsarse);
        }

        [Test]
        public void CalcularTarifa_PrimerosDosViajes_MitadPrecio()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            decimal tarifaCompleta = 700;

            // Act
            decimal tarifaCalculada1 = tarjeta.CalcularTarifa(tarifaCompleta);
            tarjeta.PagarBoleto(tarifaCompleta); // Primer viaje
            decimal tarifaCalculada2 = tarjeta.CalcularTarifa(tarifaCompleta);

            // Assert
            Assert.AreEqual(350, tarifaCalculada1); // 700 / 2
            Assert.AreEqual(350, tarifaCalculada2); // 700 / 2
        }

        [Test]
        public void CalcularTarifa_TercerViaje_PrecioCompleto()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            decimal tarifaCompleta = 700;

            // Act
            tarjeta.PagarBoleto(tarifaCompleta); // Primer viaje
            tarjeta.PagarBoleto(tarifaCompleta); // Segundo viaje
            decimal tarifaCalculada3 = tarjeta.CalcularTarifa(tarifaCompleta);

            // Assert
            Assert.AreEqual(700, tarifaCalculada3); // Precio completo
        }

        [Test]
        public void PagarBoleto_ConTiempoFalso_ControlaTiempoPrecisamente()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 2000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0));
            tarjeta.Tiempo = tiempoFalso;
            decimal tarifa = 700;

            // Act - Primer viaje
            bool resultado1 = tarjeta.PagarBoleto(tarifa);
            Assert.IsTrue(resultado1);
            Assert.AreEqual(1650, tarjeta.Saldo); // 2000 - 350

            // Intentar segundo viaje inmediatamente (debería fallar)
            bool resultado2 = tarjeta.PagarBoleto(tarifa);
            Assert.IsFalse(resultado2);
            Assert.AreEqual(1650, tarjeta.Saldo); // Saldo no cambia

            // Avanzar exactamente 5 minutos
            tiempoFalso.AgregarMinutos(5);

            // Segundo viaje (debería funcionar)
            bool resultado3 = tarjeta.PagarBoleto(tarifa);
            Assert.IsTrue(resultado3);
            Assert.AreEqual(1300, tarjeta.Saldo); // 1650 - 350
        }

        [Test]
        public void PagarBoleto_NuevoDia_ReseteaContadorViajesConDescuento()
        {
            // Arrange
            var tarjeta = new MedioBoleto("123", 10000);
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 8, 0, 0)); // Lunes
            tarjeta.Tiempo = tiempoFalso;
            decimal tarifa = 700;

            // Act - Dos viajes el lunes
            tarjeta.PagarBoleto(tarifa);
            tiempoFalso.AgregarMinutos(5);
            tarjeta.PagarBoleto(tarifa);

            // Verificar que el tercer viaje del mismo día cobra completo
            tiempoFalso.AgregarMinutos(5);
            tarjeta.PagarBoleto(tarifa); // Tercer viaje lunes - precio completo

            // Cambiar al día siguiente
            tiempoFalso.EstablecerFecha(new DateTime(2024, 10, 15, 8, 0, 0)); // Martes

            // Act - Primer viaje del martes debería tener descuento nuevamente
            tiempoFalso.AgregarMinutos(5); // Para evitar restricción de 5 minutos
            tarjeta.PagarBoleto(tarifa); // Primer viaje martes - mitad precio

            // Assert - Verificar que el contador se reseteó
            Assert.AreEqual(1, tarjeta.ViajesConDescuentoHoy);
        }
    }
}
