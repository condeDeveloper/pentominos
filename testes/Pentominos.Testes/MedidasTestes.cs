using Conde.Pentominos;
using Xunit;

namespace Conde.Pentominos.Testes;

/// <summary>
/// Todo número do README está preso aqui. As tabelas caras são calculadas uma vez
/// só para a classe inteira: cada uma delas é uma varredura de centenas de
/// milhões de nós.
/// </summary>
public class MedidasTestes
{
    private static readonly List<Medidas.LinhaCaixa> Caixas = Medidas.PorCaixa();
    private static readonly List<Medidas.LinhaPoda> AsPodas = Medidas.Podas();
    private static readonly List<Medidas.LinhaOrientacao> AsOrientacoes = Medidas.Orientacoes();

    [Theory]
    [InlineData(1, 1, 1, 1)]
    [InlineData(2, 2, 1, 1)]
    [InlineData(3, 6, 2, 2)]
    [InlineData(4, 19, 7, 5)]
    [InlineData(5, 63, 18, 12)]
    [InlineData(6, 216, 60, 35)]
    public void CadaLinhaDosPoliominos(int celulas, int fixos, int deUmLado, int livres)
    {
        var linha = Medidas.Poliominos().Single(l => l.Celulas == celulas);
        Assert.Equal(fixos, linha.Fixos);
        Assert.Equal(deUmLado, linha.DeUmLado);
        Assert.Equal(livres, linha.Livres);
    }

    [Theory]
    [InlineData("6 x 10", 9_356L, 2_339L, 152_022_678L, 95_948_889L)]
    [InlineData("5 x 12", 4_040L, 1_010L, 254_520_049L, 165_829_291L)]
    [InlineData("4 x 15", 1_472L, 368L, 268_470_758L, 183_084_715L)]
    [InlineData("3 x 20", 8L, 2L, 82_698_774L, 57_306_920L)]
    [InlineData("8 x 8 com 4 buracos", 520L, 65L, 5_187_425L, 3_136_403L)]
    public void CadaLinhaDasCaixas(string caixa, long solucoes, long distintas, long nos, long cortes)
    {
        var linha = Caixas.Single(l => l.Caixa == caixa);
        Assert.Equal(solucoes, linha.Solucoes);
        Assert.Equal(distintas, linha.Distintas);
        Assert.Equal(nos, linha.Nos);
        Assert.Equal(cortes, linha.Cortes);
    }

    [Fact]
    public void OCustoNaoAcompanhaONumeroDeSolucoes()
    {
        var seisPorDez = Caixas.Single(l => l.Caixa == "6 x 10");
        var quatroPorQuinze = Caixas.Single(l => l.Caixa == "4 x 15");

        // A caixa com SEIS VEZES menos soluções custa mais nós que a mais cheia.
        Assert.True(quatroPorQuinze.Solucoes < seisPorDez.Solucoes / 6);
        Assert.True(quatroPorQuinze.Nos > seisPorDez.Nos);
    }

    [Fact]
    public void AMetadeDosNosEhCorte()
    {
        foreach (var linha in Caixas)
            Assert.InRange((double)linha.Cortes / linha.Nos, 0.60, 0.70);
    }

    [Theory]
    [InlineData("6 x 10", 152_022_678L, 349_226_636L, 2.30)]
    [InlineData("3 x 20", 82_698_774L, 1_528_716_954L, 18.49)]
    [InlineData("8 x 8 com 4 buracos", 5_187_425L, 9_016_271L, 1.74)]
    public void CadaLinhaDaPoda(string caixa, long comPoda, long semPoda, double vezes)
    {
        var linha = AsPodas.Single(l => l.Caixa == caixa);
        Assert.Equal(comPoda, linha.ComPoda);
        Assert.Equal(semPoda, linha.SemPoda);
        Assert.Equal(vezes, linha.Vezes, 2);
    }

    [Fact]
    public void APodaPagaMaisOndeHaMenosResposta()
    {
        var cheia = AsPodas.Single(l => l.Caixa == "6 x 10");
        var vazia = AsPodas.Single(l => l.Caixa == "3 x 20");

        Assert.True(vazia.Solucoes < cheia.Solucoes / 1000);
        Assert.True(vazia.Vezes > 8 * cheia.Vezes);
    }

    [Theory]
    [InlineData('X', 1, 32)]
    [InlineData('I', 2, 56)]
    [InlineData('T', 4, 128)]
    [InlineData('U', 4, 152)]
    [InlineData('V', 4, 128)]
    [InlineData('W', 4, 128)]
    [InlineData('Z', 4, 128)]
    [InlineData('F', 8, 256)]
    [InlineData('L', 8, 248)]
    [InlineData('N', 8, 248)]
    [InlineData('P', 8, 304)]
    [InlineData('Y', 8, 248)]
    public void CadaLinhaDasOrientacoes(char nome, int orientacoes, int colocacoes)
    {
        var linha = AsOrientacoes.Single(l => l.Nome == nome);
        Assert.Equal(orientacoes, linha.Orientacoes);
        Assert.Equal(colocacoes, linha.Colocacoes);
    }

    [Fact]
    public void ACruzEhAPecaComMenosLugares()
    {
        Assert.Equal('X', AsOrientacoes[0].Nome);
        Assert.Equal(32, AsOrientacoes[0].Colocacoes);
        Assert.Equal(2_056, AsOrientacoes.Sum(l => l.Colocacoes));
    }

    [Fact]
    public void AsDuasSolucoesDeTresPorVinteSaoEspelhoUmaDaOutra()
    {
        var todas = Medidas.AsDuasDeTresPorVinte();
        Assert.Equal(8, todas.Count);
        Assert.Equal(8, todas.Distinct().Count());

        var tabuleiro = new Tabuleiro(20, 3);
        foreach (var solucao in todas)
            Assert.True(Juiz.ConfereCompleto(solucao, tabuleiro));
    }

    [Theory]
    [InlineData("6 x 10", 53_451L)]
    [InlineData("5 x 12", 203_765L)]
    [InlineData("4 x 15", 121_252L)]
    [InlineData("3 x 20", 55_080_167L)]
    [InlineData("8 x 8 com 4 buracos", 12_757L)]
    public void QuantoCustaAcharAPrimeira(string caixa, long nosPrimeira)
    {
        var (nome, largura, altura, buracos, _) = Medidas.Caixas.Single(c => c.Nome == caixa);
        var primeira = new Empacotador(new Tabuleiro(largura, altura, buracos)).Conta(paraNaPrimeira: true);
        Assert.Equal(nosPrimeira, primeira.Nos);
        Assert.Equal(nome, caixa);
    }

    [Fact]
    public void NaCaixaComDuasSolucoesAPrimeiraCustaDoisTercosDoCaminho()
    {
        var total = Caixas.Single(l => l.Caixa == "3 x 20").Nos;
        var primeira = new Empacotador(new Tabuleiro(20, 3)).Conta(paraNaPrimeira: true).Nos;
        Assert.Equal(0.666, (double)primeira / total, 3);

        var cheia = Caixas.Single(l => l.Caixa == "6 x 10").Nos;
        var primeiraCheia = new Empacotador(new Tabuleiro(10, 6)).Conta(paraNaPrimeira: true).Nos;
        Assert.True((double)primeiraCheia / cheia < 0.001);
    }

    [Theory]
    [InlineData(20, 3, 8L)]
    [InlineData(15, 4, 1_472L)]
    public void TodaSolucaoPassaPeloJuiz(int largura, int altura, long solucoes)
    {
        var (achadas, aprovadas) = Medidas.TodasPassamPeloJuiz(largura, altura);
        Assert.Equal(solucoes, achadas);
        Assert.Equal(achadas, aprovadas);
    }
}
