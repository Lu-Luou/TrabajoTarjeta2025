using NUnit.Framework;
using Tarjeta.Clases;

namespace Tarjeta.Tests
{
    [TestFixture]
    public class TarjetaAdditionalTests
    {
        [Test]
        public void Recargar_ValidAndInvalidAmountsAndLimit()
        {
            var t = new Tarjeta.Clases.Tarjeta("0001", 10000);

            Assert.IsTrue(t.Recargar(2000));
            Assert.AreEqual(12000, t.Saldo);

            Assert.IsFalse(t.Recargar(123)); // invalid amount

            // Exceed recarga limit (LIMITE_RECARGA == 40000)
            t.Saldo = 39000;
            Assert.IsFalse(t.Recargar(2000));
            Assert.AreEqual(39000, t.Saldo);
        }

        [Test]
        public void DescontarSaldo_AllowsToLimitAndRejectBeyond()
        {
            var t = new Tarjeta.Clases.Tarjeta("0002", -1000);

            // Can descontar 200 to reach -1200 (limit)
            Assert.IsTrue(t.DescontarSaldo(200));
            Assert.AreEqual(-1200, t.Saldo);

            // Further descuento should be rejected
            Assert.IsFalse(t.DescontarSaldo(1));
            Assert.AreEqual(-1200, t.Saldo);
        }

        [Test]
        public void AcreditarCarga_UsesPendingFirst_And_HandlesOverflow()
        {
            var t = new Tarjeta.Clases.Tarjeta("0003", 55900);
            t.SaldoPendiente = 2000;

            // When there is pending, the credited amount reduces pending but limited by space
            t.AcreditarCarga(1500);
            Assert.AreEqual(56000, t.Saldo);
            Assert.AreEqual(500, t.SaldoPendiente);

            // No pending, but amount exceeds limit -> saldo fills and remainder becomes pendiente
            t = new Tarjeta.Clases.Tarjeta("0004", 55900);
            t.AcreditarCarga(2000);
            Assert.AreEqual(56000, t.Saldo);
            Assert.AreEqual(1900, t.SaldoPendiente);
        }

        [Test]
        public void CalcularUsoFrecuente_Tiers_AppliedViaPagarBoletoUsuario()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("u1", 100000);
            var usuario = new Usuario("Juan", tarjeta);
            var cole = new Colectivo("L1");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0); // weekday

            // viajes < 30 -> full tarifa
            usuario.ViajesEsteMes = 29;
            var b1 = tarjeta.PagarBoleto(usuario, cole, fecha);
            Assert.AreEqual(700, b1.TotalAbonado);

            // viajes == 30 -> 20% discount (0.8)
            usuario.ViajesEsteMes = 30;
            var b2 = tarjeta.PagarBoleto(usuario, cole, fecha.AddMinutes(5));
            Assert.AreEqual(700 * 0.8m, b2.TotalAbonado);

            // viajes == 60 -> 25% discount (0.75)
            usuario.ViajesEsteMes = 60;
            var b3 = tarjeta.PagarBoleto(usuario, cole, fecha.AddMinutes(10));
            Assert.AreEqual(700 * 0.75m, b3.TotalAbonado);

            // viajes == 80 -> no special discount (falls through to tarifa)
            usuario.ViajesEsteMes = 80;
            var b4 = tarjeta.PagarBoleto(usuario, cole, fecha.AddMinutes(15));
            Assert.AreEqual(700, b4.TotalAbonado);
        }

        [Test]
        public void Trasbordo_WithinOneHourAndDifferentLine_IsFree()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("t1", 10000);
            var usuario = new Usuario("Ana", tarjeta);
            var cole1 = new Colectivo("L1");
            var cole2 = new Colectivo("L2");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0); // Monday

            var primero = tarjeta.PagarBoleto(usuario, cole1, fecha);
            Assert.AreEqual(700, primero.TotalAbonado);

            // Second within 30 minutes on a different line -> free trasbordo
            var segundo = tarjeta.PagarBoleto(usuario, cole2, fecha.AddMinutes(30));
            Assert.AreEqual(0, segundo.TotalAbonado);
        }

        [Test]
        public void PagarBoleto_WithFranquiciaHorario_InvalidThrows()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("tx", 10000);
            // Simulate that this tarjeta is of a franquicia type with horario
            tarjeta.Tipo = "MedioBoleto";

            // Use a Sunday (no weekday allowed) or hour outside
            var fecha = new DateTime(2024, 10, 13, 3, 0, 0); // Sunday early morning

            Assert.Throws<InvalidOperationException>(() => tarjeta.PagarBoleto(700m, fecha));
        }

        [Test]
        public void ToString_FormatsCorrectly()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("99", 1234.5m);
            string s = tarjeta.ToString();
            Assert.IsTrue(s.Contains("Tarjeta Nº: 99"));
            Assert.IsTrue(s.Contains("Saldo: $1234.50"));
        }

        [Test]
        public void Colectivo_ToString_And_PagarCon_NormalBehavior()
        {
            var cole = new Colectivo("L3", "EmpresaX", esInterurbana: true);
            Assert.IsTrue(cole.ToString().Contains("Interurbana"));

            var tarjeta = new Tarjeta.Clases.Tarjeta("c1", 10000);
            var boleto = cole.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000, boleto.TotalAbonado);
        }

        [Test]
        public void Program_Main_DoesNotThrow()
        {
            // Program is internal in the Tarjeta assembly; call Main via reflection to ensure it doesn't throw
            var asm = typeof(Tarjeta.Clases.Tarjeta).Assembly;
            var progType = asm.GetType("Tarjeta.Clases.Program", throwOnError: false);
            Assert.IsNotNull(progType, "Program type should exist");
            var main = progType.GetMethod("Main", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            Assert.IsNotNull(main, "Main method should exist");
            // Invoke with an empty string[] argument
            main.Invoke(null, new object[] { new string[0] });
        }

        [Test]
        public void Colectivo_PagarCon_WithMonto_TarjetaNormal_SaldoSuficiente()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("123", 2000);
            var colectivo = new Colectivo("102");

            var boleto = colectivo.PagarCon(tarjeta, 700);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(700, boleto.TotalAbonado);
            Assert.AreEqual(1300, boleto.SaldoRestante);
        }

        [Test]
        public void Colectivo_PagarCon_WithMonto_TarjetaNormal_SaldoInsuficiente()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("123", 100);
            var colectivo = new Colectivo("102");

            var boleto = colectivo.PagarCon(tarjeta, 700);

            Assert.IsNull(boleto);
        }

        [Test]
        public void Colectivo_PagarCon_WithMonto_BEG_PrimerViaje()
        {
            var tarjeta = new BEG("123", 2000);
            var colectivo = new Colectivo("102");

            var boleto = colectivo.PagarCon(tarjeta, 700);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.TotalAbonado); // Primer viaje gratis
            Assert.AreEqual(2000, boleto.SaldoRestante);
        }

        [Test]
        public void Colectivo_PagarCon_WithMonto_MedioBoleto()
        {
            var tarjeta = new MedioBoleto("123", 2000);
            var colectivo = new Colectivo("102");

            var boleto = colectivo.PagarCon(tarjeta, 700);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(350, boleto.TotalAbonado); // Medio boleto
            Assert.AreEqual(1650, boleto.SaldoRestante);
        }

        [Test]
        public void Trasbordo_SameLine_NoTrasbordo()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("t2", 10000);
            var usuario = new Usuario("Ana", tarjeta);
            var cole = new Colectivo("L1");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0); // Monday

            var primero = tarjeta.PagarBoleto(usuario, cole, fecha);
            Assert.AreEqual(700, primero.TotalAbonado);

            // Second on same line -> no trasbordo
            var segundo = tarjeta.PagarBoleto(usuario, cole, fecha.AddMinutes(30));
            Assert.AreEqual(700, segundo.TotalAbonado); // Full price
        }

        [Test]
        public void Trasbordo_OnSunday_NoTrasbordo()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("t3", 10000);
            var usuario = new Usuario("Ana", tarjeta);
            var cole1 = new Colectivo("L1");
            var cole2 = new Colectivo("L2");
            var fecha = new DateTime(2024, 10, 13, 8, 0, 0); // Sunday

            var primero = tarjeta.PagarBoleto(usuario, cole1, fecha);
            Assert.AreEqual(700, primero.TotalAbonado);

            // Second on different line but Sunday -> no trasbordo
            var segundo = tarjeta.PagarBoleto(usuario, cole2, fecha.AddMinutes(30));
            Assert.AreEqual(700, segundo.TotalAbonado);
        }

        [Test]
        public void Trasbordo_OutsideHours_NoTrasbordo()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("t4", 10000);
            var usuario = new Usuario("Ana", tarjeta);
            var cole1 = new Colectivo("L1");
            var cole2 = new Colectivo("L2");
            var fecha = new DateTime(2024, 10, 14, 23, 0, 0); // Monday late night

            var primero = tarjeta.PagarBoleto(usuario, cole1, fecha);
            Assert.AreEqual(700, primero.TotalAbonado);

            // Second on different line but outside hours -> no trasbordo
            var segundo = tarjeta.PagarBoleto(usuario, cole2, fecha.AddMinutes(30));
            Assert.AreEqual(700, segundo.TotalAbonado);
        }

        [Test]
        public void Trasbordo_AfterMoreThanOneHour_NoTrasbordo()
        {
            var tarjeta = new Tarjeta.Clases.Tarjeta("t5", 10000);
            var usuario = new Usuario("Ana", tarjeta);
            var cole1 = new Colectivo("L1");
            var cole2 = new Colectivo("L2");
            var fecha = new DateTime(2024, 10, 14, 8, 0, 0); // Monday

            var primero = tarjeta.PagarBoleto(usuario, cole1, fecha);
            Assert.AreEqual(700, primero.TotalAbonado);

            // Second after 61 minutes -> no trasbordo
            var segundo = tarjeta.PagarBoleto(usuario, cole2, fecha.AddMinutes(61));
            Assert.AreEqual(700, segundo.TotalAbonado);
        }
    }
}
