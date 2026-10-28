namespace Conde.Pentominos;

/// <summary>
/// A caixa a ser preenchida. Até 64 células, porque cada estado é um
/// <c>ulong</c>: cobrir é um ou, conflitar é um e, e a primeira célula livre é um
/// contador de zeros à direita.
/// </summary>
public sealed class Tabuleiro
{
    public Tabuleiro(int largura, int altura, IEnumerable<(int X, int Y)>? buracos = null)
    {
        if (largura < 1 || altura < 1) throw new ArgumentOutOfRangeException(nameof(largura));
        if (largura * altura > 64) throw new ArgumentException("mais de 64 células não cabem num ulong");

        Largura = largura;
        Altura = altura;

        ulong fora = 0;
        foreach (var (x, y) in buracos ?? [])
        {
            if (x < 0 || x >= largura || y < 0 || y >= altura)
                throw new ArgumentException($"buraco fora do tabuleiro: {x},{y}");
            fora |= 1UL << Indice(x, y);
        }

        Buracos = fora;
        var todas = largura * altura == 64 ? ulong.MaxValue : (1UL << (largura * altura)) - 1;
        Livres = todas & ~fora;
        Celulas = System.Numerics.BitOperations.PopCount(Livres);
    }

    public int Largura { get; }

    public int Altura { get; }

    /// <summary>As células que precisam ser cobertas.</summary>
    public ulong Livres { get; }

    /// <summary>As células que não existem.</summary>
    public ulong Buracos { get; }

    /// <summary>Quantas células precisam ser cobertas.</summary>
    public int Celulas { get; }

    public int Indice(int x, int y) => y * Largura + x;

    /// <summary>
    /// Todas as maneiras de pôr uma forma dentro do tabuleiro, como máscaras. Uma
    /// colocação que cai em buraco ou passa da borda não entra na lista.
    /// </summary>
    public List<ulong> Colocacoes(Forma forma)
    {
        ArgumentNullException.ThrowIfNull(forma);
        var saida = new List<ulong>();

        foreach (var orientacao in forma.Orientacoes())
            for (var y = 0; y + orientacao.Altura <= Altura; y++)
                for (var x = 0; x + orientacao.Largura <= Largura; x++)
                {
                    ulong mascara = 0;
                    foreach (var (cx, cy) in orientacao.Celulas)
                        mascara |= 1UL << Indice(x + cx, y + cy);

                    if ((mascara & Buracos) != 0) continue;
                    saida.Add(mascara);
                }

        return saida;
    }

    /// <summary>O desenho de uma solução, uma letra por célula.</summary>
    public string Desenho(IReadOnlyDictionary<char, ulong> pecas)
    {
        ArgumentNullException.ThrowIfNull(pecas);
        var grade = new char[Largura * Altura];
        Array.Fill(grade, '.');

        foreach (var (nome, mascara) in pecas)
            for (var i = 0; i < Largura * Altura; i++)
                if ((mascara >> i & 1) != 0)
                    grade[i] = nome;

        var linhas = new string[Altura];
        for (var y = 0; y < Altura; y++) linhas[y] = new string(grade, y * Largura, Largura);
        return string.Join("/", linhas);
    }
}
