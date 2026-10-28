using Conde.Pentominos;
using Xunit;

namespace Conde.Pentominos.Testes;

public class CatalogoTestes
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    [InlineData(4, 19)]
    [InlineData(5, 63)]
    [InlineData(6, 216)]
    public void QuantosPoliominosFixosExistem(int celulas, int quantos)
    {
        Assert.Equal(quantos, Catalogo.Fixos(celulas).Count);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 5)]
    [InlineData(5, 12)]
    [InlineData(6, 35)]
    public void QuantosPoliominosLivresExistem(int celulas, int quantos)
    {
        Assert.Equal(quantos, Catalogo.Livres(celulas).Count);
    }

    [Theory]
    [InlineData(4, 7)]
    [InlineData(5, 18)]
    [InlineData(6, 60)]
    public void QuantosPoliominosDeUmLadoExistem(int celulas, int quantos)
    {
        // É o conjunto do Tetris: lá o S e o Z são peças diferentes porque não se
        // pode virar a peça do avesso.
        Assert.Equal(quantos, Catalogo.DeUmLado(celulas).Count);
    }

    [Fact]
    public void ADerivacaoDaExatamenteAsDozePecasEscritasAMao()
    {
        var derivadas = Catalogo.Livres(5).Select(f => f.ToString()).OrderBy(x => x, StringComparer.Ordinal);
        var aMao = Catalogo.PorNome.Values.Select(f => f.Livre().ToString()).OrderBy(x => x, StringComparer.Ordinal);
        Assert.Equal(aMao, derivadas);
    }

    [Fact]
    public void TodaPecaDerivadaTemNome()
    {
        var nomes = Catalogo.Livres(5).Select(Catalogo.Nome).OrderBy(c => c).ToArray();
        Assert.Equal("FILNPTUVWXYZ", new string(nomes));
    }

    [Fact]
    public void ONomeNaoMudaComOGiro()
    {
        foreach (var (nome, peca) in Catalogo.PorNome)
            foreach (var orientacao in peca.Orientacoes())
                Assert.Equal(nome, Catalogo.Nome(orientacao));
    }

    [Fact]
    public void OQueNaoEhPentominoNaoTemNome()
    {
        Assert.Throws<ArgumentException>(() => Catalogo.Nome(Forma.Desenho("##")));
    }

    [Fact]
    public void TodoPoliominoDerivadoEhLigado()
    {
        foreach (var forma in Catalogo.Fixos(5))
            Assert.True(forma.Ligada(), forma.ToString());
    }

    [Fact]
    public void OsFixosSaoMaisQueOsDeUmLadoQueSaoMaisQueOsLivres()
    {
        for (var n = 4; n <= 6; n++)
        {
            Assert.True(Catalogo.Fixos(n).Count > Catalogo.DeUmLado(n).Count);
            Assert.True(Catalogo.DeUmLado(n).Count > Catalogo.Livres(n).Count);
        }
    }

    [Fact]
    public void ZeroCelulasNaoEhPeca()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Catalogo.Fixos(0));
    }
}
