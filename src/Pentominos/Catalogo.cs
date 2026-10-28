namespace Conde.Pentominos;

/// <summary>
/// De onde vêm as peças. Elas não são digitadas: são DERIVADAS, fazendo crescer
/// uma célula de cada vez e jogando fora quem é a mesma peça girada. A lista
/// escrita à mão existe só para dar nome às doze, e o teste confere que as duas
/// listas são o mesmo conjunto.
/// </summary>
public static class Catalogo
{
    /// <summary>
    /// Todos os poliominós FIXOS de n células: os que não são iguais nem
    /// deslizando. 1, 2, 6, 19, 63, 216 para n de 1 a 6.
    /// </summary>
    public static List<Forma> Fixos(int celulas)
    {
        if (celulas < 1) throw new ArgumentOutOfRangeException(nameof(celulas));

        var atual = new HashSet<Forma> { new(new[] { (0, 0) }) };
        for (var n = 2; n <= celulas; n++)
        {
            var proxima = new HashSet<Forma>();
            foreach (var forma in atual)
                foreach (var (x, y) in forma.Celulas)
                    foreach (var vizinho in new[] { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1) })
                    {
                        if (forma.Celulas.Contains(vizinho)) continue;
                        proxima.Add(new Forma(forma.Celulas.Append(vizinho)));
                    }
            atual = proxima;
        }

        return atual.OrderBy(f => f.ToString(), StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// Os poliominós LIVRES: girar e espelhar não contam. 1, 1, 2, 5, 12, 35.
    /// Os doze de cinco células são os pentominós.
    /// </summary>
    public static List<Forma> Livres(int celulas) =>
        Fixos(celulas)
            .Select(f => f.Livre())
            .Distinct()
            .OrderBy(f => f.ToString(), StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Os poliominós de UM LADO: girar conta, espelhar não. 1, 1, 2, 7, 18, 60.
    /// É o conjunto que o Tetris usa, e é por isso que o S e o Z são peças
    /// diferentes lá.
    /// </summary>
    public static List<Forma> DeUmLado(int celulas)
    {
        var vistos = new List<Forma>();
        foreach (var forma in Fixos(celulas))
        {
            var atual = forma;
            var achou = false;
            for (var giro = 0; giro < 4 && !achou; giro++)
            {
                if (vistos.Contains(atual)) achou = true;
                atual = atual.Girada();
            }
            if (!achou) vistos.Add(forma);
        }
        return vistos.OrderBy(f => f.ToString(), StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// As doze peças com os nomes de Golomb, escritas à mão. Esta lista NÃO é
    /// usada pela busca: ela serve para dar nome e para ser conferida contra o que
    /// a derivação produz.
    /// </summary>
    public static readonly IReadOnlyDictionary<char, Forma> PorNome = new Dictionary<char, Forma>
    {
        ['F'] = Forma.Desenho(".##/##./.#."),
        ['I'] = Forma.Desenho("#####"),
        ['L'] = Forma.Desenho("#./#./#./##"),
        ['N'] = Forma.Desenho(".#/.#/##/#."),
        ['P'] = Forma.Desenho("##/##/#."),
        ['T'] = Forma.Desenho("###/.#./.#."),
        ['U'] = Forma.Desenho("#.#/###"),
        ['V'] = Forma.Desenho("#../#../###"),
        ['W'] = Forma.Desenho("#../##./.##"),
        ['X'] = Forma.Desenho(".#./###/.#."),
        ['Y'] = Forma.Desenho(".#/##/.#/.#"),
        ['Z'] = Forma.Desenho("##./.#./.##"),
    };

    /// <summary>O nome de uma peça de cinco células, qualquer que seja o giro dela.</summary>
    public static char Nome(Forma forma)
    {
        ArgumentNullException.ThrowIfNull(forma);
        var livre = forma.Livre();
        foreach (var (nome, peca) in PorNome)
            if (peca.Livre().Equals(livre))
                return nome;
        throw new ArgumentException("isto não é um pentominó: " + forma);
    }
}
