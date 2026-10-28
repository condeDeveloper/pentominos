using Conde.Pentominos;
using Xunit;

namespace Conde.Pentominos.Testes;

public class JuizTestes
{
    private static readonly string UmaDeTresPorVinte =
        new Empacotador(new Tabuleiro(20, 3)).Conta(paraNaPrimeira: true).Primeira;

    [Fact]
    public void UmaSolucaoDeVerdadePassa()
    {
        Assert.True(Juiz.Confere(UmaDeTresPorVinte, new Tabuleiro(20, 3)));
        Assert.True(Juiz.ConfereCompleto(UmaDeTresPorVinte, new Tabuleiro(20, 3)));
    }

    [Fact]
    public void DuasLetrasTrocadasEntreSiReprovam()
    {
        // Trocar o nome de duas peças quebra a forma das duas.
        var estragada = UmaDeTresPorVinte.Replace('I', '@').Replace('X', 'I').Replace('@', 'X');
        Assert.False(Juiz.Confere(estragada, new Tabuleiro(20, 3)));
    }

    [Fact]
    public void CelulaVaziaOndeNaoHaBuracoReprova()
    {
        var comFuro = UmaDeTresPorVinte.Replace('I', '.');
        Assert.False(Juiz.Confere(comFuro, new Tabuleiro(20, 3)));
    }

    [Fact]
    public void LetraQueNaoEhPecaReprova()
    {
        var estranha = UmaDeTresPorVinte.Replace('I', 'Q');
        Assert.False(Juiz.Confere(estranha, new Tabuleiro(20, 3)));
    }

    [Fact]
    public void DesenhoDoTamanhoErradoReprova()
    {
        Assert.False(Juiz.Confere("XX/XX", new Tabuleiro(20, 3)));
        Assert.False(Juiz.Confere(UmaDeTresPorVinte, new Tabuleiro(10, 6)));
    }

    [Fact]
    public void PecaEmCimaDeBuracoReprova()
    {
        var tabuleiro = new Tabuleiro(8, 8, new[] { (3, 3), (3, 4), (4, 3), (4, 4) });
        var solucao = new Empacotador(tabuleiro).Conta(paraNaPrimeira: true).Primeira;
        Assert.True(Juiz.ConfereCompleto(solucao, tabuleiro));

        var tapando = solucao.Replace('.', 'X');
        Assert.False(Juiz.Confere(tapando, tabuleiro));
    }

    [Fact]
    public void PecaFaltandoReprovaNoCompleto()
    {
        // Em um tabuleiro de 55 células só onze peças entram: a solução está certa
        // pela definição de cada peça, mas não é um empacotamento dos doze.
        var tabuleiro = new Tabuleiro(11, 5);
        var pacote = new Empacotador(tabuleiro).Conta(paraNaPrimeira: true);
        Assert.Equal(1, pacote.Solucoes);
        Assert.True(Juiz.Confere(pacote.Primeira, tabuleiro));
        Assert.False(Juiz.ConfereCompleto(pacote.Primeira, tabuleiro));
    }

    [Fact]
    public void DesenhoVazioNaoEhSolucao()
    {
        Assert.Throws<ArgumentException>(() => Juiz.Confere(string.Empty, new Tabuleiro(20, 3)));
    }
}
