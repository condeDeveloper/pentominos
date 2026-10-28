# Pentominós

Os doze pentominós do zero em C# e .NET 8, encaixados nas quatro caixas de 60
células por varredura completa. A de 6 por 10 tem **2.339** soluções, e a de 3 por
20 tem **duas**.

As peças não são digitadas no código. Elas são **derivadas**: um poliominó cresce
uma célula de cada vez, quem é igual a outro girado ou espelhado é jogado fora, e
no tamanho cinco sobram doze. A lista com os nomes de Golomb existe só para
chamá-las de F, I, L, N, P, T, U, V, W, X, Y e Z, e um teste confere que as duas
listas são o mesmo conjunto.

```bash
dotnet test testes/Pentominos.Testes/Pentominos.Testes.csproj -c Release    # 103 testes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release          # todas as tabelas
```

## As peças, derivadas

```
   celulas   fixos  de um lado  livres
         1       1           1       1
         2       2           1       1
         3       6           2       2
         4      19           7       5
         5      63          18      12
         6     216          60      35
```

Três colunas para a mesma pergunta, com simetrias diferentes. **Fixos**: só
deslizar conta. **De um lado**: deslizar e girar; é o conjunto do Tetris, e é por
isso que o S e o Z são peças diferentes lá, já que ninguém vira a peça do avesso.
**Livres**: deslizar, girar e espelhar, que é o que pentominó quer dizer.

## As caixas

```
  caixa                    solucoes  sem simetria            nos       cortes  segundos
  6 x 10                      9,356         2,339    152,022,678   95,948,889     11.13
  5 x 12                      4,040         1,010    254,520,049  165,829,291     17.90
  4 x 15                      1,472           368    268,470,758  183,084,715     17.41
  3 x 20                          8             2     82,698,774   57,306,920      5.14
  8 x 8 com 4 buracos           520            65      5,187,425    3,136,403      0.34
```

2.339, 1.010, 368, 2 e 65: os números que Haselgrove calculou em 1960 num
computador que ocupava uma sala, refeitos aqui.

A busca escolhe sempre a **primeira célula vazia** e tenta as peças que cobrem
*ela*. Não é detalhe: escolher a peça em vez da célula acharia a mesma solução
doze vezes, e um buraco impossível só apareceria no fim.

Olhe a coluna de nós. A caixa de 4 por 15 tem **seis vezes menos soluções** que a
de 6 por 10 e custa **mais nós** para ser varrida. O custo mede o tamanho da
árvore que a busca precisa abrir, e isso não tem relação com quantas folhas dessa
árvore são respostas.

## Achar uma, achar todas

```
  caixa                   nos ate a primeira  nos ate o fim   fracao
  6 x 10                              53,451    152,022,678    0.0%
  5 x 12                             203,765    254,520,049    0.1%
  4 x 15                             121,252    268,470,758    0.0%
  3 x 20                          55,080,167     82,698,774   66.6%
  8 x 8 com 4 buracos                 12,757      5,187,425    0.2%
```

Em quatro das cinco caixas, achar *uma* solução é de graça: 53 mil nós contra 152
milhões, 0,03% do caminho. Quem quer uma resposta para montar em casa para a busca
quase na largada.

Na caixa de 3 por 20 não. Ela tem duas soluções e a primeira custa **66,6% da
varredura inteira**: 55 dos 82 milhões de nós. **Quando a resposta é rara, achar
uma e provar que são todas viram o mesmo problema**, porque o trabalho está em
eliminar o que não serve, e isso é a mesma coisa nos dois casos.

## A poda que não olha para peça nenhuma

```
  caixa                    nos com poda   nos sem poda    razao   solucoes
  6 x 10                    152,022,678    349,226,636    2.30x      9,356
  3 x 20                     82,698,774  1,528,716,954   18.49x          8
  8 x 8 com 4 buracos          5,187,425      9,016,271    1.74x        520
```

A poda é uma pergunta só: sobrou algum pedaço vazio com menos de cinco células? Se
sobrou, nenhuma peça cabe nele, e tudo o que vier depois é trabalho jogado fora.
Ela não sabe quais peças ainda estão na mão nem o formato de nenhuma.

E ela paga muito mais onde há menos resposta: 2,30x na caixa cheia de soluções,
**18,49x na caixa que quase não tem**. Faz sentido depois de medido: numa caixa com
2.339 saídas, a busca chega ao fim por bem; numa com duas, quase todo caminho
morre, e a poda é justamente quem mata cedo.

## As peças não são iguais

```
   peca  orientacoes  colocacoes
      X            1          32
      I            2          56
      T            4         128
      U            4         152
      V            4         128
      W            4         128
      Z            4         128
      F            8         256
      L            8         248
      N            8         248
      P            8         304
      Y            8         248
```

A cruz é simétrica por todos os lados: girar não muda nada, e ela tem **uma
orientação só**. Cabe em 32 lugares da caixa de 6 por 10, contra 304 do P. São
2.056 colocações no total, e é essa lista que a busca percorre.

## As duas de 3 por 20

```
  VZYYYYWTFNNNIIIIIXUU
  VZZZYWWTFFFNNLPPXXXU
  VVVZWWTTTFLLLLPPPXUU

  VVVNNNFTWYYYYZPPPXUU
  VLNNFFFTWWYZZZPPXXXU
  VLLLLFTTTWWZIIIIIXUU
```

As outras seis que a varredura conta são estas duas viradas e espelhadas.

## O que as medidas me corrigiram

**A poda custava cinco vezes o repositório inteiro.** Escrevi o "sobrou buraco
pequeno?" com uma pilha e um conjunto de visitados, do jeito que se ensina flood
fill, e a varredura das cinco caixas levou 4 minutos e 48 segundos. Trocando a
pilha por crescimento de máscara — `pedaço | vizinhos(pedaço) & vazio`, até parar
de crescer — foram 54 segundos. **O algoritmo não mudou**: são os mesmos
152.022.678 nós, dígito por dígito, nas duas versões. Mudou só o preço de cada nó,
e quando o nó acontece 150 milhões de vezes, esse preço é o programa inteiro.

**Dividi por quatro o que tinha de ser dividido por oito.** Cada solução aparece
quatro vezes nas caixas retangulares, porque o retângulo tem quatro simetrias. O
quadrado de 8 por 8 tem oito, e eu tinha escrito 130 onde a literatura diz 65. O
número conhecido existia justamente para esse tipo de erro ser pego.

**Esperava que a caixa com menos soluções fosse a mais barata de varrer.** A de 3
por 20 tem duas soluções e custa 82 milhões de nós; a de 4 por 15 tem 368 e custa
268 milhões. Número de respostas e tamanho da árvore são coisas diferentes, e eu
estava tratando como se fossem a mesma.

**Escrevi um teste afirmando que parar na primeira é barato, e ele quebrou.** Eu
tinha posto `nós da primeira < nós totais / 100`, confiante. Na caixa de 3 por 20 a
primeira custa 66,6% do total. O teste quebrado não era um teste frouxo: era uma
medida que eu não tinha feito, e virou uma tabela do README.

## Os arquivos

| arquivo | o que faz |
|---|---|
| `src/Pentominos/Forma.cs` | um poliominó, suas oito orientações e a representante do grupo |
| `src/Pentominos/Catalogo.cs` | as peças derivadas por crescimento, e a lista com nomes para conferir |
| `src/Pentominos/Tabuleiro.cs` | a caixa em 64 bits, com buracos, e as colocações de cada peça |
| `src/Pentominos/Empacotador.cs` | a busca pela primeira célula vazia, com a poda do buraco pequeno |
| `src/Pentominos/Juiz.cs` | lê o desenho pronto, remonta cada peça e confere pela forma |
| `src/Pentominos/Medidas.cs` | as seis tabelas deste README |
| `ferramentas/Medidor` | imprime as tabelas, uma por comando |

## Rodando

```bash
dotnet test testes/Pentominos.Testes/Pentominos.Testes.csproj -c Release

dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- pecas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- caixas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- primeira
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- poda
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- orientacoes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- tresporvinte
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
```

C# 12 e .NET 8. Zero dependências fora do xUnit, e só nos testes.

103 testes. Os números deste README estão presos em `MedidasTestes`.
