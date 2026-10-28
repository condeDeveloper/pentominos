using Conde.Pentominos;

var comando = args.Length > 0 ? args[0] : "tudo";

if (comando is "pecas" or "tudo") Pecas();
if (comando is "caixas" or "tudo") Caixas();
if (comando is "poda" or "tudo") Poda();
if (comando is "orientacoes" or "tudo") Orientacoes();
if (comando is "primeira" or "tudo") Primeira();
if (comando is "tresporvinte" or "tudo") TresPorVinte();
if (comando is "juiz" or "tudo") Juiz();

static void Pecas()
{
    Console.WriteLine();
    Console.WriteLine("AS PECAS NAO SAO DIGITADAS, sao derivadas: cresce uma celula e joga fora quem repete.");
    Console.WriteLine();
    Console.WriteLine($"  {"celulas",8} {"fixos",7} {"de um lado",11} {"livres",7}");

    foreach (var l in Medidas.Poliominos())
        Console.WriteLine($"  {l.Celulas,8} {l.Fixos,7:n0} {l.DeUmLado,11:n0} {l.Livres,7:n0}");

    Console.WriteLine();
}

static void Caixas()
{
    Console.WriteLine();
    Console.WriteLine("AS CAIXAS DE 60 CELULAS, varridas inteiras.");
    Console.WriteLine();
    Console.WriteLine($"  {"caixa",-22} {"solucoes",10} {"sem simetria",13} {"nos",14} {"cortes",12} {"segundos",9}");

    foreach (var l in Medidas.PorCaixa())
        Console.WriteLine($"  {l.Caixa,-22} {l.Solucoes,10:n0} {l.Distintas,13:n0} {l.Nos,14:n0} {l.Cortes,12:n0} {l.Segundos,9:0.00}");

    Console.WriteLine();
}

static void Poda()
{
    Console.WriteLine();
    Console.WriteLine("A PODA DO BURACO PEQUENO: desistir quando sobra um pedaco com menos de cinco celulas.");
    Console.WriteLine();
    Console.WriteLine($"  {"caixa",-12} {"nos com poda",14} {"nos sem poda",14} {"razao",8} {"solucoes",10}");

    foreach (var l in Medidas.Podas())
        Console.WriteLine($"  {l.Caixa,-12} {l.ComPoda,14:n0} {l.SemPoda,14:n0} {l.Vezes,7:0.00}x {l.Solucoes,10:n0}");

    Console.WriteLine();
}

static void Orientacoes()
{
    Console.WriteLine();
    Console.WriteLine("ORIENTACOES DE CADA PECA, e onde ela cabe na caixa de 6 por 10.");
    Console.WriteLine();
    Console.WriteLine($"  {"peca",5} {"orientacoes",12} {"colocacoes",11}");

    foreach (var l in Medidas.Orientacoes())
        Console.WriteLine($"  {l.Nome,5} {l.Orientacoes,12} {l.Colocacoes,11:n0}");

    Console.WriteLine();
}

static void Primeira()
{
    Console.WriteLine();
    Console.WriteLine("ACHAR UMA contra ACHAR TODAS, em nos.");
    Console.WriteLine();
    Console.WriteLine($"  {"caixa",-22} {"nos ate a primeira",19} {"nos ate o fim",14} {"fracao",8}");

    foreach (var l in Medidas.AchaUmaContraAchaTodas())
        Console.WriteLine($"  {l.Caixa,-22} {l.NosPrimeira,19:n0} {l.NosTodas,14:n0} {l.Fracao,7:0.0%}");

    Console.WriteLine();
}

static void TresPorVinte()
{
    Console.WriteLine();
    Console.WriteLine("AS SOLUCOES DE 3 POR 20, que cabem todas aqui:");
    Console.WriteLine();

    foreach (var solucao in Medidas.AsDuasDeTresPorVinte())
    {
        foreach (var linha in solucao.Split('/')) Console.WriteLine("  " + linha);
        Console.WriteLine();
    }
}

static void Juiz()
{
    Console.WriteLine();
    Console.WriteLine("TODA SOLUCAO PASSA PELO JUIZ, que le o desenho e nao sabe nada da busca.");
    Console.WriteLine();

    foreach (var (largura, altura) in new[] { (20, 3), (15, 4), (12, 5) })
    {
        var (solucoes, aprovadas) = Medidas.TodasPassamPeloJuiz(largura, altura);
        Console.WriteLine($"  {altura} x {largura,-3}  {solucoes,8:n0} solucoes  {aprovadas,8:n0} aprovadas  {(solucoes == aprovadas ? "sim" : "NAO")}");
    }

    Console.WriteLine();
}
