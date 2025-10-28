using Xunit;
using TarjetaClass = Tarjeta.Clases.Tarjeta;

namespace TarjetaTests
{
    public class TarjetaTests
    {
        [Fact]
        public void Constructor_DeberiaInicializarTarjetaConNumeroYSaldoCero()
        {
            string numero = "12345";
            
            var tarjeta = new TarjetaClass(numero);
            
            Assert.Equal(numero, tarjeta.Numero);
            Assert.Equal(0, tarjeta.Saldo);
        }

        [Fact]
        public void Constructor_DeberiaInicializarTarjetaConSaldoInicial()
        {
            string numero = "12345";
            decimal saldoInicial = 5000;
            
            var tarjeta = new TarjetaClass(numero, saldoInicial);
            
            Assert.Equal(numero, tarjeta.Numero);
            Assert.Equal(saldoInicial, tarjeta.Saldo);
        }

        [Theory]
        [InlineData(2000)]
        [InlineData(3000)]
        [InlineData(4000)]
        [InlineData(5000)]
        [InlineData(8000)]
        [InlineData(10000)]
        [InlineData(15000)]
        [InlineData(20000)]
        [InlineData(25000)]
        [InlineData(30000)]
        public void Recargar_ConCargasAceptadas_DeberiaRetornarTrue(decimal monto)
        {
            var tarjeta = new TarjetaClass("12345");
            
            bool resultado = tarjeta.Recargar(monto);
            
            Assert.True(resultado);
            Assert.Equal(monto, tarjeta.Saldo);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(1580)]
        [InlineData(2500)]
        [InlineData(7000)]
        [InlineData(12000)]
        [InlineData(40000)]
        public void Recargar_ConCargasNoAceptadas_DeberiaRetornarFalse(decimal monto)
        {
            var tarjeta = new TarjetaClass("12345");
            decimal saldoAnterior = tarjeta.Saldo;
            
            bool resultado = tarjeta.Recargar(monto);
            
            Assert.False(resultado);
            Assert.Equal(saldoAnterior, tarjeta.Saldo);
        }

        [Fact]
        public void Recargar_ExcediendoLimiteDeSaldo_DeberiaRetornarFalse()
        {
            var tarjeta = new TarjetaClass("12345", 35000);
            
            bool resultado = tarjeta.Recargar(10000);
            
            Assert.False(resultado);
            Assert.Equal(35000, tarjeta.Saldo);
        }

        [Fact]
        public void Recargar_NoExcediendoLimiteDeSaldo_DeberiaRetornarTrue()
        {
            var tarjeta = new TarjetaClass("12345", 35000);
            
            bool resultado = tarjeta.Recargar(5000);
            
            Assert.True(resultado);
            Assert.Equal(40000, tarjeta.Saldo);
        }

        [Fact]
        public void DescontarSaldo_ConSaldoSuficiente_DeberiaRetornarTrue()
        {
            var tarjeta = new TarjetaClass("12345", 5000);
            decimal montoADescontar = 1580;
            
            bool resultado = tarjeta.DescontarSaldo(montoADescontar);
            
            Assert.True(resultado);
            Assert.Equal(3420, tarjeta.Saldo);
        }

        [Fact]
        public void DescontarSaldo_ConSaldoInsuficiente_DeberiaRetornarFalse()
        {
            var tarjeta = new TarjetaClass("12345", 1000);
            decimal montoADescontar = 1580;
            
            bool resultado = tarjeta.DescontarSaldo(montoADescontar);
            
            Assert.False(resultado);
            Assert.Equal(1000, tarjeta.Saldo);
        }

        [Fact]
        public void DescontarSaldo_ConSaldoExacto_DeberiaRetornarTrue()
        {
            var tarjeta = new TarjetaClass("12345", 1580);
            decimal montoADescontar = 1580;
            
            bool resultado = tarjeta.DescontarSaldo(montoADescontar);
            
            Assert.True(resultado);
            Assert.Equal(0, tarjeta.Saldo);
        }
        [Fact]
        public void DescontarSaldo_PuedeLlegarHastaMenos1200()
        {
            var t = new Tarjeta("003", 0);
            bool resultado = t.DescontarSaldo(1000);

            Assert.True(resultado);
            Assert.Equal(-1000, t.Saldo);
        }

        [Fact]
        public void DescontarSaldo_NoDebeSuperarMenos1200()
        {
            var t = new Tarjeta("004", -1000);
            bool resultado = t.DescontarSaldo(300);

            Assert.False(resultado);
            Assert.Equal(-1000, t.Saldo);
        public void PagarBoleto_DeberiaDescontarSaldo()
        {
            var t = new Tarjeta("001", 1000);
            bool resultado = t.PagarBoleto(400);

            Assert.True(resultado);
            Assert.Equal(600, t.Saldo);
        }

        [Fact]
        public void PagarBoleto_NoDeberiaPermitirSaldoNegativo()
        {
            var t = new Tarjeta("002", 300);
            bool resultado = t.PagarBoleto(500);

            Assert.False(resultado);
            Assert.Equal(300, t.Saldo);
        }
    }
}

        [Fact]
        public void ToString_DeberiaRetornarFormatoCorrect()
        {
            var tarjeta = new TarjetaClass("12345", 2500.50m);
            
            string resultado = tarjeta.ToString();
            
            Assert.Equal("Tarjeta Nº: 12345, Saldo: $2500.50", resultado);
        }
    }
    public class FranquiciaTests
    {
        [Fact]
        public void MedioBoleto_DeberiaCobrarMitad()
        {
            var t = new MedioBoleto("005", 1000);
            t.PagarBoleto(800);
            Assert.Equal(600, t.Saldo);
        }

        [Fact]
        public void FranquiciaCompleta_DeberiaCobrarEntero()
        {
            var t = new FranquiciaCompleta("006", 1000);
            t.PagarBoleto(800);
            Assert.Equal(200, t.Saldo);
        }

        [Fact]
        public void BoletoGratuito_NoDeberiaDescontarNada()
        {
            var t = new BoletoGratuitoEstudiantil("007", 1000);
            t.PagarBoleto(800);
            Assert.Equal(1000, t.Saldo);
        }
    }
}
}