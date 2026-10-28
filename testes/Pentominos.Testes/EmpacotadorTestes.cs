using Conde.Pentominos;
using Xunit;

namespace Conde.Pentominos.Testes;

public class EmpacotadorTestes
{
    [Fact]
    public void OTabuleiroSabeQuantasCelulasPrecisamDePeca()
    {
        Assert.Equal(60, new Tabuleiro(10, 6).Celulas);
        Assert.Equal(60, new Tabuleiro(8, 8, new[] { (3, 3), (3, 4), (4, 3), (4, 4) }).Celulas);
    }

    [Fact]
    public void MaisDeSessentaEQuatroCelulasNaoCabemNumUlong()
    {
        Assert.Throws<ArgumentException>(() => new Tabuleiro(9, 9));
    }

    [Fact]
    public void BuracoForaDoTabuleiroNaoExiste()
    {
        Assert.Throws<ArgumentException>(() => new Tabuleiro(5, 5, new[] { (9, 9) }));
    }

    [Fact]
    public void ACruzCabeEmTrintaEDoisLugaresNaCaixaDeSeisPorDez()
    {
        Assert.Equal(32, new Tabuleiro(10, 6).Colocacoes(Catalogo.PorNome['X']).Count);
    }

    [Fact]
    public void OIDeitadoNaoCabeNaCaixaDeTresDeAltura()
    {
        var tabuleiro = new Tabuleiro(4, 3);
        // Na caixa 4 x 3 o I de cinco células não cabe de jeito nenhum.
        Assert.Empty(tabuleiro.Colocacoes(Catalogo.PorNome['I']));
    }

    [Fact]
    public void ACaixaDeTresPorVinteTemDuasSolucoesForaAsSimetrias()
    {
        var pacote = new Empacotador(new Tabuleiro(20, 3)).Conta();
        Assert.Equal(8, pacote.Solucoes);
        Assert.Equal(2, pacote.Solucoes / 4);
    }

    [Fact]
    public void TodaSolucaoDeTresPorVintePassaPeloJuiz()
    {
        var tabuleiro = new Tabuleiro(20, 3);
        var aprovadas = 0;
        var pacote = new Empacotador(tabuleiro).Conta(aCada: d =>
        {
            if (Juiz.ConfereCompleto(d, tabuleiro)) aprovadas++;
        });
        Assert.Equal(pacote.Solucoes, aprovadas);
    }

    [Fact]
    public void PararNaPrimeiraCustaMuitoMenos()
    {
        var empacotador = new Empacotador(new Tabuleiro(20, 3));
        var tudo = empacotador.Conta();
        var primeira = empacotador.Conta(paraNaPrimeira: true);

        Assert.Equal(1, primeira.Solucoes);
        Assert.Equal(tudo.Primeira, primeira.Primeira);

        // E custa quase o mesmo: numa caixa com duas soluções, chegar à primeira
        // é dois terços do caminho até o fim.
        Assert.Equal(55_080_167L, primeira.Nos);
        Assert.Equal(0.67, (double)primeira.Nos / tudo.Nos, 2);
    }

    [Fact]
    public void APodaNaoMudaOQueEhAchado()
    {
        var empacotador = new Empacotador(new Tabuleiro(8, 8, new[] { (3, 3), (3, 4), (4, 3), (4, 4) }));
        var com = empacotador.Conta();
        var sem = empacotador.Conta(podaBuraco: false);

        Assert.Equal(com.Solucoes, sem.Solucoes);
        Assert.True(com.Nos < sem.Nos);
        Assert.Equal(0, sem.Cortes);
    }

    [Fact]
    public void UmaCaixaQueNaoDaParaFecharDaZeroSolucoes()
    {
        // 59 células: uma peça não cabe inteira.
        var tabuleiro = new Tabuleiro(10, 6, new[] { (0, 0) });
        Assert.Equal(0, new Empacotador(tabuleiro).Conta().Solucoes);
    }

    [Fact]
    public void OBuracoDeQuatroCelulasEhCortadoPelaPoda()
    {
        var tabuleiro = new Tabuleiro(10, 6);
        var empacotador = new Empacotador(tabuleiro);
        // Uma quina de 2 x 2 isolada tem quatro células: nenhuma peça cabe.
        ulong quina = 0;
        foreach (var (x, y) in new[] { (0, 0), (1, 0), (0, 1), (1, 1) }) quina |= 1UL << tabuleiro.Indice(x, y);
        Assert.True(empacotador.TemBuracoPequeno(quina));
    }

    [Fact]
    public void AsDozePecasEstaoNaBusca()
    {
        Assert.Equal(12, new Empacotador(new Tabuleiro(10, 6)).Pecas.Count);
    }
}
