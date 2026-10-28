namespace Conde.Pentominos;

/// <summary>
/// Um poliominó: um punhado de células grudadas, sem posição fixa. Duas formas
/// são a mesma se uma vira a outra deslizando; girar e espelhar são outra
/// história, e quem cuida disso é <see cref="Livre"/>.
/// </summary>
public sealed class Forma : IEquatable<Forma>
{
    private readonly (int X, int Y)[] _celulas;

    public Forma(IEnumerable<(int X, int Y)> celulas)
    {
        ArgumentNullException.ThrowIfNull(celulas);
        var lista = celulas.Distinct().ToArray();
        if (lista.Length == 0) throw new ArgumentException("forma sem célula nenhuma", nameof(celulas));

        var menorX = lista.Min(c => c.X);
        var menorY = lista.Min(c => c.Y);
        _celulas = lista
            .Select(c => (X: c.X - menorX, Y: c.Y - menorY))
            .OrderBy(c => c.X)
            .ThenBy(c => c.Y)
            .ToArray();

        Largura = _celulas.Max(c => c.X) + 1;
        Altura = _celulas.Max(c => c.Y) + 1;
    }

    public IReadOnlyList<(int X, int Y)> Celulas => _celulas;

    public int Tamanho => _celulas.Length;

    public int Largura { get; }

    public int Altura { get; }

    /// <summary>A forma girada um quarto de volta.</summary>
    public Forma Girada() => new(_celulas.Select(c => (c.Y, -c.X)));

    /// <summary>A forma espelhada no eixo vertical.</summary>
    public Forma Espelhada() => new(_celulas.Select(c => (-c.X, c.Y)));

    /// <summary>
    /// As oito formas que saem de girar e espelhar, sem repetição. Peça simétrica
    /// dá menos de oito, e é isso que faz a cruz ter uma só.
    /// </summary>
    public List<Forma> Orientacoes()
    {
        var vistas = new List<Forma>();
        var atual = this;
        for (var espelho = 0; espelho < 2; espelho++)
        {
            for (var giro = 0; giro < 4; giro++)
            {
                if (!vistas.Contains(atual)) vistas.Add(atual);
                atual = atual.Girada();
            }
            atual = atual.Espelhada();
        }
        return vistas;
    }

    /// <summary>
    /// A representante do grupo: a menor das orientações na ordem do texto. Duas
    /// formas têm a mesma livre exatamente quando são a mesma peça.
    /// </summary>
    public Forma Livre() => Orientacoes().OrderBy(f => f.ToString(), StringComparer.Ordinal).First();

    /// <summary>Todas as células estão ligadas por lado?</summary>
    public bool Ligada()
    {
        var conhecidas = new HashSet<(int, int)> { _celulas[0] };
        var fila = new Queue<(int X, int Y)>();
        fila.Enqueue(_celulas[0]);
        var dentro = _celulas.ToHashSet();

        while (fila.Count > 0)
        {
            var (x, y) = fila.Dequeue();
            foreach (var vizinho in new[] { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1) })
            {
                if (!dentro.Contains(vizinho) || !conhecidas.Add(vizinho)) continue;
                fila.Enqueue(vizinho);
            }
        }

        return conhecidas.Count == _celulas.Length;
    }

    public bool Equals(Forma? outra) =>
        outra is not null && outra._celulas.Length == _celulas.Length && _celulas.SequenceEqual(outra._celulas);

    public override bool Equals(object? obj) => Equals(obj as Forma);

    public override int GetHashCode()
    {
        var h = new HashCode();
        foreach (var (x, y) in _celulas) { h.Add(x); h.Add(y); }
        return h.ToHashCode();
    }

    /// <summary>O desenho em linhas, com # e ponto. É assim que a forma é escrita nos testes.</summary>
    public override string ToString()
    {
        var linhas = new string[Altura];
        for (var y = 0; y < Altura; y++)
        {
            var linha = new char[Largura];
            for (var x = 0; x < Largura; x++)
                linha[x] = _celulas.Contains((x, y)) ? '#' : '.';
            linhas[y] = new string(linha);
        }
        return string.Join("/", linhas);
    }

    /// <summary>A forma lida de um desenho como "###/.#./.#.".</summary>
    public static Forma Desenho(string texto)
    {
        ArgumentException.ThrowIfNullOrEmpty(texto);
        var linhas = texto.Split('/');
        var celulas = new List<(int, int)>();
        for (var y = 0; y < linhas.Length; y++)
            for (var x = 0; x < linhas[y].Length; x++)
                if (linhas[y][x] == '#')
                    celulas.Add((x, y));
        return new Forma(celulas);
    }
}
