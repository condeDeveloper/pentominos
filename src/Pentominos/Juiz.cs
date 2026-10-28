namespace Conde.Pentominos;

/// <summary>
/// O juiz olha para o desenho pronto, letra por letra, e não sabe nada de
/// máscara, de busca e de poda. Ele reconstrói cada peça a partir das células que
/// têm a mesma letra e confere que ela é mesmo a peça daquele nome.
/// </summary>
public static class Juiz
{
    public static bool Confere(string desenho, Tabuleiro tabuleiro)
    {
        ArgumentException.ThrowIfNullOrEmpty(desenho);
        ArgumentNullException.ThrowIfNull(tabuleiro);

        var linhas = desenho.Split('/');
        if (linhas.Length != tabuleiro.Altura) return false;

        var celulasPorLetra = new Dictionary<char, List<(int, int)>>();
        for (var y = 0; y < linhas.Length; y++)
        {
            if (linhas[y].Length != tabuleiro.Largura) return false;
            for (var x = 0; x < linhas[y].Length; x++)
            {
                var letra = linhas[y][x];
                var ehBuraco = (tabuleiro.Buracos >> tabuleiro.Indice(x, y) & 1) != 0;

                if (letra == '.')
                {
                    if (!ehBuraco) return false;   // célula viva sem peça
                    continue;
                }

                if (ehBuraco) return false;        // peça em cima de buraco
                if (!celulasPorLetra.TryGetValue(letra, out var lista))
                    celulasPorLetra[letra] = lista = [];
                lista.Add((x, y));
            }
        }

        foreach (var (letra, celulas) in celulasPorLetra)
        {
            if (!Catalogo.PorNome.TryGetValue(letra, out var peca)) return false;
            var forma = new Forma(celulas);
            if (!forma.Ligada()) return false;
            if (!forma.Livre().Equals(peca.Livre())) return false;
        }

        return true;
    }

    /// <summary>
    /// A conferência completa de um empacotamento de pentominós: além de estar
    /// certo, usa as doze peças, cada uma uma vez.
    /// </summary>
    public static bool ConfereCompleto(string desenho, Tabuleiro tabuleiro)
    {
        if (!Confere(desenho, tabuleiro)) return false;
        var letras = desenho.Replace("/", string.Empty).Where(c => c != '.').Distinct().ToList();
        if (letras.Count != Catalogo.PorNome.Count) return false;

        foreach (var letra in letras)
            if (desenho.Count(c => c == letra) != 5)
                return false;

        return true;
    }
}
