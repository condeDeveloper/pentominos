using System.Numerics;

namespace Conde.Pentominos;

/// <summary>O que uma varredura achou e o que custou.</summary>
public readonly record struct Pacote(long Solucoes, string Primeira, long Nos, long Cortes);

/// <summary>
/// A busca: a cada passo escolhe a primeira célula vazia e tenta todas as peças
/// que cobrem ELA. Escolher a célula em vez da peça é o que impede a mesma
/// solução de ser achada doze vezes, e é o que faz um buraco impossível aparecer
/// cedo.
/// </summary>
public sealed class Empacotador
{
    private readonly Tabuleiro _tabuleiro;
    private readonly char[] _nomes;
    private readonly List<ulong>[][] _porCelula;

    public Empacotador(Tabuleiro tabuleiro, IReadOnlyDictionary<char, Forma>? pecas = null)
    {
        ArgumentNullException.ThrowIfNull(tabuleiro);
        _tabuleiro = tabuleiro;
        var catalogo = pecas ?? Catalogo.PorNome;
        _nomes = catalogo.Keys.ToArray();

        var celulas = tabuleiro.Largura * tabuleiro.Altura;
        for (var y = 0; y < tabuleiro.Altura; y++)
        {
            _colunaEsquerda |= 1UL << tabuleiro.Indice(0, y);
            _colunaDireita |= 1UL << tabuleiro.Indice(tabuleiro.Largura - 1, y);
        }

        _porCelula = new List<ulong>[_nomes.Length][];
        for (var p = 0; p < _nomes.Length; p++)
        {
            _porCelula[p] = new List<ulong>[celulas];
            for (var c = 0; c < celulas; c++) _porCelula[p][c] = [];

            foreach (var mascara in tabuleiro.Colocacoes(catalogo[_nomes[p]]))
            {
                var primeira = BitOperations.TrailingZeroCount(mascara);
                for (var c = 0; c < celulas; c++)
                    if ((mascara >> c & 1) != 0)
                        _porCelula[p][c].Add(mascara);
                _ = primeira;
            }
        }
    }

    /// <summary>As peças disponíveis, na ordem em que são tentadas.</summary>
    public IReadOnlyList<char> Pecas => _nomes;

    /// <summary>
    /// Conta todas as maneiras de cobrir o tabuleiro usando cada peça uma vez.
    /// </summary>
    /// <param name="podaBuraco">
    /// Quando ligada, desiste assim que sobra um pedaço vazio com menos de cinco
    /// células, que nenhuma peça poderia preencher.
    /// </param>
    public Pacote Conta(bool podaBuraco = true, bool paraNaPrimeira = false, Action<string>? aCada = null)
    {
        long solucoes = 0, nos = 0, cortes = 0;
        var primeira = string.Empty;
        var usada = new bool[_nomes.Length];
        var escolhidas = new ulong[_nomes.Length];

        bool Passo(ulong ocupado)
        {
            nos++;

            var falta = _tabuleiro.Livres & ~ocupado;
            if (falta == 0)
            {
                solucoes++;
                var desenho = Desenho(escolhidas, usada);
                if (primeira.Length == 0) primeira = desenho;
                aCada?.Invoke(desenho);
                return paraNaPrimeira;
            }

            if (podaBuraco && TemBuracoPequeno(falta))
            {
                cortes++;
                return false;
            }

            var celula = BitOperations.TrailingZeroCount(falta);
            for (var p = 0; p < _nomes.Length; p++)
            {
                if (usada[p]) continue;
                foreach (var mascara in _porCelula[p][celula])
                {
                    if ((mascara & ocupado) != 0) continue;
                    usada[p] = true;
                    escolhidas[p] = mascara;
                    if (Passo(ocupado | mascara)) return true;
                    usada[p] = false;
                }
            }

            return false;
        }

        Passo(0);
        return new Pacote(solucoes, primeira, nos, cortes);
    }

    private string Desenho(ulong[] escolhidas, bool[] usada)
    {
        var mapa = new Dictionary<char, ulong>();
        for (var p = 0; p < _nomes.Length; p++)
            if (usada[p])
                mapa[_nomes[p]] = escolhidas[p];
        return _tabuleiro.Desenho(mapa);
    }

    /// <summary>
    /// Existe pedaço vazio com menos de cinco células? Se existe, nenhuma peça
    /// cabe nele e o resto da busca é trabalho jogado fora.
    /// </summary>
    public bool TemBuracoPequeno(ulong falta)
    {
        var porVisitar = falta;
        while (porVisitar != 0)
        {
            var inicio = BitOperations.TrailingZeroCount(porVisitar);
            var pedaco = Pedaco(falta, inicio);
            if (BitOperations.PopCount(pedaco) < 5) return true;
            porVisitar &= ~pedaco;
        }
        return false;
    }

    /// <summary>
    /// O pedaço ligado que contém a célula de partida, crescido com operações de
    /// 64 bits em vez de uma fila. Numa busca que chama isto 150 milhões de vezes,
    /// a diferença entre empilhar vizinhos e deslocar um ulong é o repositório
    /// rodar em 20 segundos ou em dois minutos.
    /// </summary>
    private ulong Pedaco(ulong falta, int inicio)
    {
        var pedaco = 1UL << inicio;
        while (true)
        {
            var crescido = pedaco | Vizinhos(pedaco) & falta;
            if (crescido == pedaco) return pedaco;
            pedaco = crescido;
        }
    }

    private ulong Vizinhos(ulong m) =>
        (m & ~_colunaDireita) << 1 |
        (m & ~_colunaEsquerda) >> 1 |
        m << _tabuleiro.Largura |
        m >> _tabuleiro.Largura;

    private readonly ulong _colunaEsquerda;
    private readonly ulong _colunaDireita;
}
