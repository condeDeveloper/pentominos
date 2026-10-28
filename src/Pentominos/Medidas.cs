using System.Diagnostics;

namespace Conde.Pentominos;

/// <summary>
/// As medidas do README. Os testes conferem o que estes métodos devolvem; o
/// medidor só imprime.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaPoliomino(int Celulas, int Fixos, int DeUmLado, int Livres);

    public readonly record struct LinhaCaixa(
        string Caixa, int Largura, int Altura, long Solucoes, long Distintas, long Nos, long Cortes, double Segundos);

    public readonly record struct LinhaPoda(string Caixa, long ComPoda, long SemPoda, double Vezes, long Solucoes);

    public readonly record struct LinhaOrientacao(char Nome, int Orientacoes, int Colocacoes);

    public readonly record struct LinhaPrimeira(string Caixa, long NosPrimeira, long NosTodas, double Fracao);

    /// <summary>
    /// Os poliominós contados por varredura, por número de células. As três
    /// colunas são a mesma coisa com simetrias diferentes.
    /// </summary>
    public static List<LinhaPoliomino> Poliominos(int ate = 6)
    {
        var linhas = new List<LinhaPoliomino>();
        for (var n = 1; n <= ate; n++)
            linhas.Add(new LinhaPoliomino(n, Catalogo.Fixos(n).Count, Catalogo.DeUmLado(n).Count, Catalogo.Livres(n).Count));
        return linhas;
    }

    /// <summary>As quatro caixas de 60 células, mais o quadrado com buraco no meio.</summary>
    public static readonly (string Nome, int Largura, int Altura, (int X, int Y)[] Buracos, int Simetria)[] Caixas =
    [
        ("6 x 10", 10, 6, [], 4),
        ("5 x 12", 12, 5, [], 4),
        ("4 x 15", 15, 4, [], 4),
        ("3 x 20", 20, 3, [], 4),
        ("8 x 8 com 4 buracos", 8, 8, [(3, 3), (3, 4), (4, 3), (4, 4)], 8),
    ];

    public static List<LinhaCaixa> PorCaixa()
    {
        var linhas = new List<LinhaCaixa>();
        foreach (var (nome, largura, altura, buracos, simetria) in Caixas)
        {
            var tabuleiro = new Tabuleiro(largura, altura, buracos);
            var relogio = Stopwatch.StartNew();
            var pacote = new Empacotador(tabuleiro).Conta();
            relogio.Stop();

            linhas.Add(new LinhaCaixa(
                nome, largura, altura, pacote.Solucoes, pacote.Solucoes / simetria,
                pacote.Nos, pacote.Cortes, relogio.Elapsed.TotalSeconds));
        }
        return linhas;
    }

    /// <summary>
    /// O que a poda do buraco pequeno vale. Ela não olha para peça nenhuma: só
    /// pergunta se sobrou um pedaço vazio com menos de cinco células.
    /// </summary>
    public static List<LinhaPoda> Podas()
    {
        var linhas = new List<LinhaPoda>();
        // Sem poda a varredura abre bilhões de nós, então a comparação fica nas
        // três caixas que ainda terminam num tempo que dá para esperar: a mais
        // cheia de soluções, a mais vazia, e o quadrado com buraco no meio.
        foreach (var (nome, largura, altura, buracos, _) in Caixas.Where(c => c.Nome is "6 x 10" or "3 x 20" or "8 x 8 com 4 buracos"))
        {
            var tabuleiro = new Tabuleiro(largura, altura, buracos);
            var empacotador = new Empacotador(tabuleiro);
            var com = empacotador.Conta();
            var sem = empacotador.Conta(podaBuraco: false);

            linhas.Add(new LinhaPoda(nome, com.Nos, sem.Nos, (double)sem.Nos / com.Nos, com.Solucoes));
        }
        return linhas;
    }

    /// <summary>
    /// Quantas orientações cada peça tem e quantas colocações ela aceita na caixa
    /// de 6 por 10. A cruz tem uma orientação só, e é por isso que ela é a peça
    /// que mais manda na busca.
    /// </summary>
    public static List<LinhaOrientacao> Orientacoes()
    {
        var tabuleiro = new Tabuleiro(10, 6);
        return Catalogo.PorNome
            .Select(p => new LinhaOrientacao(p.Key, p.Value.Orientacoes().Count, tabuleiro.Colocacoes(p.Value).Count))
            .OrderBy(l => l.Orientacoes)
            .ThenBy(l => l.Nome)
            .ToList();
    }

    /// <summary>
    /// O que custa achar UMA solução contra o que custa achar todas. Numa caixa
    /// cheia de soluções a primeira sai de graça; na caixa que quase não tem, ela
    /// custa quase a varredura inteira.
    /// </summary>
    public static List<LinhaPrimeira> AchaUmaContraAchaTodas()
    {
        // Os totais vêm da tabela das caixas em vez de serem contados de novo:
        // são as mesmas varreduras, e cada uma custa dezenas de segundos.
        var totais = PorCaixa().ToDictionary(l => l.Caixa, l => l.Nos);
        var linhas = new List<LinhaPrimeira>();

        foreach (var (nome, largura, altura, buracos, _) in Caixas)
        {
            var primeira = new Empacotador(new Tabuleiro(largura, altura, buracos)).Conta(paraNaPrimeira: true);
            linhas.Add(new LinhaPrimeira(nome, primeira.Nos, totais[nome], (double)primeira.Nos / totais[nome]));
        }

        return linhas;
    }

    /// <summary>
    /// Todas as soluções da caixa de 3 por 20, que são tão poucas que cabem no
    /// README.
    /// </summary>
    public static List<string> AsDuasDeTresPorVinte()
    {
        var achadas = new List<string>();
        new Empacotador(new Tabuleiro(20, 3)).Conta(aCada: achadas.Add);
        return achadas;
    }

    /// <summary>
    /// Cada solução achada passa pelo juiz, que a lê do desenho e não sabe nada da
    /// busca. O retorno é (soluções, aprovadas).
    /// </summary>
    public static (long Solucoes, long Aprovadas) TodasPassamPeloJuiz(int largura, int altura)
    {
        var tabuleiro = new Tabuleiro(largura, altura);
        long aprovadas = 0;
        var pacote = new Empacotador(tabuleiro).Conta(aCada: desenho =>
        {
            if (Juiz.ConfereCompleto(desenho, tabuleiro)) aprovadas++;
        });
        return (pacote.Solucoes, aprovadas);
    }
}
