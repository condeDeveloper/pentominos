using Conde.Pentominos;
using Xunit;

namespace Conde.Pentominos.Testes;

public class FormaTestes
{
    [Fact]
    public void AFormaNasceEncostadaNoCanto()
    {
        var forma = new Forma(new[] { (5, 7), (6, 7), (5, 8) });
        Assert.Equal("##/#.", forma.ToString());
        Assert.Equal(2, forma.Largura);
        Assert.Equal(2, forma.Altura);
    }

    [Fact]
    public void ODesenhoVoltaIgual()
    {
        foreach (var peca in Catalogo.PorNome.Values)
            Assert.Equal(peca, Forma.Desenho(peca.ToString()));
    }

    [Fact]
    public void CelulaRepetidaNaoContaDuasVezes()
    {
        Assert.Equal(2, new Forma(new[] { (0, 0), (1, 0), (0, 0) }).Tamanho);
    }

    [Fact]
    public void FormaVaziaNaoExiste()
    {
        Assert.Throws<ArgumentException>(() => new Forma(Array.Empty<(int, int)>()));
    }

    [Fact]
    public void GirarQuatroVezesVoltaAoComeco()
    {
        foreach (var peca in Catalogo.PorNome.Values)
            Assert.Equal(peca, peca.Girada().Girada().Girada().Girada());
    }

    [Fact]
    public void EspelharDuasVezesVoltaAoComeco()
    {
        foreach (var peca in Catalogo.PorNome.Values)
            Assert.Equal(peca, peca.Espelhada().Espelhada());
    }

    [Theory]
    [InlineData('X', 1)]
    [InlineData('I', 2)]
    [InlineData('Z', 4)]
    [InlineData('T', 4)]
    [InlineData('U', 4)]
    [InlineData('V', 4)]
    [InlineData('W', 4)]
    [InlineData('F', 8)]
    [InlineData('L', 8)]
    [InlineData('N', 8)]
    [InlineData('P', 8)]
    [InlineData('Y', 8)]
    public void QuantasOrientacoesCadaPecaTem(char nome, int orientacoes)
    {
        Assert.Equal(orientacoes, Catalogo.PorNome[nome].Orientacoes().Count);
    }

    [Fact]
    public void TodaOrientacaoTemOMesmoTamanhoEAMesmaLivre()
    {
        foreach (var peca in Catalogo.PorNome.Values)
            foreach (var orientacao in peca.Orientacoes())
            {
                Assert.Equal(5, orientacao.Tamanho);
                Assert.Equal(peca.Livre(), orientacao.Livre());
            }
    }

    [Fact]
    public void AsDozePecasSaoLigadas()
    {
        foreach (var peca in Catalogo.PorNome.Values)
            Assert.True(peca.Ligada(), peca.ToString());
    }

    [Fact]
    public void DuasCelulasSoltasNaoSaoLigadas()
    {
        Assert.False(new Forma(new[] { (0, 0), (5, 5) }).Ligada());
    }

    [Fact]
    public void DuasPecasDiferentesTemLivresDiferentes()
    {
        var livres = Catalogo.PorNome.Values.Select(p => p.Livre()).ToList();
        Assert.Equal(12, livres.Distinct().Count());
    }
}
