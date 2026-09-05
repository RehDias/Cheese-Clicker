# 🧀 Cheese Clicker

Um jogo **Point and Click / Clicker** desenvolvido em Unity com C#.

O jogador deve clicar em um queijo para diminuir seu tamanho. Cada clique gera uma recompensa. Quando o queijo chega ao tamanho mínimo, ele desaparece e um novo queijo cai do topo da tela.

As recompensas obtidas podem ser utilizadas para comprar **cartas de upgrade**, que fornecem melhorias ao jogador, como aumento do poder dos cliques, e também **skins para o personagem**.

> **Escopo:** este documento cobre exclusivamente a parte de **programação e configuração técnica do jogo**. Assets, sprites, animações visuais, UI final e identidade visual serão desenvolvidos separadamente.

---

# 🎮 1. Conceito do jogo

O loop principal do jogo será:

```text
Jogador
   ↓
Clica no queijo
   ↓
Queijo diminui
   ↓
Jogador recebe recompensa
   ↓
Acumula moedas
   ↓
Compra cartas
   ↓
Recebe buffs
   ↓
Clica novamente
   ↓
Queijo chega ao tamanho mínimo
   ↓
Novo queijo cai
   ↓
Loop continua
```

O jogo deve ser inicialmente desenvolvido sem sistemas desnecessariamente complexos.

A primeira versão deve funcionar com:

* 1 personagem;
* 1 queijo;
* 1 tipo de moeda;
* 1 tipo de clique;
* algumas cartas;
* algumas skins;
* geração de novos queijos;
* sistema básico de progressão.

---

# 🛠️ 2. Configuração inicial do projeto Unity

## 2.1 Criar o projeto

Criar um projeto Unity utilizando o template:

```text
2D
```

O jogo será essencialmente 2D, portanto não há necessidade de utilizar um projeto 3D.

---

## 2.2 Estrutura de pastas

Criar uma estrutura organizada desde o início:

```text
Assets/
│
├── Art/
│   ├── Characters/
│   ├── Cheese/
│   ├── Cards/
│   ├── Skins/
│   └── UI/
│
├── Audio/
│   ├── Music/
│   └── SFX/
│
├── Prefabs/
│   ├── Cheese/
│   ├── Character/
│   ├── Cards/
│   └── UI/
│
├── Scenes/
│   ├── MainMenu.unity
│   └── Game.unity
│
├── Scripts/
│   ├── Core/
│   ├── Player/
│   ├── Cheese/
│   ├── Cards/
│   ├── Shop/
│   ├── UI/
│   └── Save/
│
├── ScriptableObjects/
│   ├── Cards/
│   └── Skins/
│
└── Resources/
```

A separação não precisa ser perfeita inicialmente. O objetivo é evitar colocar todos os scripts em uma única pasta.

---

# 🧩 3. Sistemas que precisam ser programados

O jogo será dividido nos seguintes sistemas:

### Core

Responsável pelo funcionamento geral do jogo.

* GameManager
* GameState
* inicialização dos sistemas

### Player

Responsável pelas informações do jogador.

* moedas;
* poder do clique;
* cartas adquiridas;
* skin selecionada.

### Cheese

Responsável pelo queijo.

* receber cliques;
* diminuir de tamanho;
* verificar quando foi destruído;
* gerar o próximo queijo.

### Reward

Responsável pelas recompensas.

* calcular recompensa;
* entregar moedas ao jogador.

### Cards

Responsável pelas cartas.

* dados das cartas;
* compra;
* aplicação de buffs;
* controle das cartas adquiridas.

### Shop

Responsável pela loja.

* verificar preço;
* verificar saldo;
* comprar carta;
* comprar skin.

### Skin

Responsável pela seleção e aplicação da aparência do personagem.

### Save

Responsável por salvar e carregar o progresso.

### UI

Responsável por comunicar os dados do jogo para o jogador.

---

# 🧀 4. Sistema do queijo

## 4.1 Criar o objeto Cheese

O queijo deve ser um `GameObject` na cena.

O objeto deverá possuir, dependendo da implementação escolhida:

```text
Cheese
├── SpriteRenderer
├── Collider2D
└── CheeseController
```

O `Collider2D` será utilizado para detectar o clique.

---

## 4.2 CheeseController

Criar um script:

```text
CheeseController.cs
```

Responsabilidades:

* receber o clique;
* informar ao sistema de jogo que o jogador clicou;
* reduzir o tamanho do queijo;
* verificar se chegou ao tamanho mínimo;
* avisar quando deve ser substituído.

O script não deve ser responsável por controlar moedas ou loja.

Exemplo de responsabilidade:

```text
CheeseController
        ↓
"Fui clicado"
        ↓
aplica dano/redução
        ↓
verifica tamanho
        ↓
"cheguei ao fim"
```

---

# 🖱️ 5. Sistema de clique

Cada clique no queijo deve executar uma ação.

O valor básico deve ser controlado por uma variável:

```csharp
public int clickPower;
```

Exemplo:

```text
Click Power = 1
```

Um clique:

```text
Queijo = 100%
↓
Clique
↓
Queijo = 99%
```

O valor real pode ser ajustado posteriormente.

É importante **não colocar o valor diretamente no código em vários lugares**.

Evitar:

```csharp
cheeseSize -= 1;
coins += 1;
```

em vários scripts.

Preferir propriedades centralizadas:

```csharp
player.ClickPower
player.Reward
```

Isso facilitará a implementação das cartas posteriormente.

---

# 📉 6. Redução do tamanho do queijo

O queijo deve diminuir conforme o jogador clica.

Uma possibilidade simples:

```csharp
transform.localScale -= ...
```

Porém, o tamanho mínimo precisa ser controlado.

Exemplo conceitual:

```text
Tamanho inicial = 1.0
Tamanho mínimo = 0.1
```

O jogo deve verificar:

```text
Se tamanho <= tamanho mínimo
    queijo terminou
```

Não permitir que o tamanho continue diminuindo indefinidamente.

---

# 💰 7. Sistema de recompensa

Cada clique deve gerar uma recompensa.

Inicialmente:

```text
1 clique = 1 moeda
```

Depois as cartas poderão modificar esse valor.

Criar um sistema responsável pela recompensa.

Por exemplo:

```text
RewardSystem
```

Responsabilidade:

```text
receber informação do clique
        ↓
calcular recompensa
        ↓
adicionar moeda ao jogador
```

Isso permite posteriormente implementar:

```text
Recompensa base = 1

Carta +50% recompensa

Resultado:
1 → 1.5
```

Ou, para manter o jogo simples:

```text
Recompensa base = 1
Carta = +1 moeda por clique

Resultado:
1 → 2
```

Para o primeiro jogo, a segunda abordagem é mais fácil de controlar.

---

# 🪙 8. Sistema de moedas

Criar uma variável central para representar o dinheiro do jogador.

Exemplo:

```csharp
private int coins;
```

O jogador deve poder:

* receber moedas;
* gastar moedas;
* consultar quantidade atual.

Criar métodos:

```csharp
AddCoins()
```

e

```csharp
SpendCoins()
```

A loja não deve alterar diretamente a variável `coins`.

Evitar:

```csharp
player.coins -= price;
```

Preferir:

```csharp
player.SpendCoins(price);
```

Isso permite centralizar as regras.

---

# 🔄 9. Novo queijo

Quando o queijo chegar ao tamanho mínimo:

```text
Queijo terminou
      ↓
Remover/desativar queijo atual
      ↓
Criar novo queijo
      ↓
Posicionar no topo
      ↓
Fazer queijo cair
```

O novo queijo deverá aparecer no topo da área de jogo.

A criação pode utilizar um **Prefab**.

Estrutura:

```text
CheeseSpawner
       ↓
Instantiate(CheesePrefab)
```

---

# 🏭 10. CheeseSpawner

Criar:

```text
CheeseSpawner.cs
```

Responsabilidades:

* criar queijo;
* definir posição inicial;
* controlar qual prefab será utilizado;
* criar um novo queijo quando o anterior terminar.

O `CheeseSpawner` **não deve controlar moedas ou cartas**.

---

# ⬇️ 11. Queda do queijo

Quando um novo queijo aparecer, ele deve cair de cima.

Para a primeira versão, não é necessário criar um sistema de física complexo.

Uma alternativa simples é utilizar:

```csharp
transform.position
```

e mover o objeto gradualmente.

Posteriormente, caso seja necessário, pode ser utilizado:

```text
Rigidbody2D
```

com gravidade.

Para um primeiro projeto, recomenda-se começar pela solução mais simples.

---

# 🎴 12. Sistema de cartas

As cartas são o principal sistema de progressão.

Cada carta deve possuir informações próprias.

Exemplo:

```text
Carta:
Nome: Super Click
Preço: 20
Efeito: +1 Click Power
```

Outra:

```text
Carta:
Nome: Cheese Master
Preço: 50
Efeito: +2 moedas por clique
```

---

# 📦 13. ScriptableObject para cartas

As cartas são um excelente caso de uso para `ScriptableObject`.

Criar:

```text
CardData.cs
```

Exemplo conceitual:

```csharp
[CreateAssetMenu(menuName = "Cards/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int price;
    public int clickPowerBonus;
    public int rewardBonus;
}
```

Assim, cada carta pode ser criada diretamente pelo editor do Unity.

Por exemplo:

```text
Cards/
├── SuperClick.asset
├── CheeseMaster.asset
└── MegaClick.asset
```

Isso evita criar uma classe diferente para cada carta.

---

# 🎯 14. Tipos de efeito das cartas

Inicialmente, limitar os efeitos das cartas.

### Efeito 1 — Click Power

Aumenta o poder do clique.

```text
Carta: Strong Fingers
+1 Click Power
```

### Efeito 2 — Reward

Aumenta a quantidade de moedas recebidas.

```text
Carta: Golden Cheese
+1 Reward
```

### Efeito 3 — Multiplicador

Pode ser implementado posteriormente.

```text
+10% recompensa
```

Não é necessário implementar todos os tipos de efeito na primeira versão.

---

# 🛒 15. Sistema da loja

Criar:

```text
ShopManager.cs
```

Responsabilidades:

* apresentar cartas disponíveis;
* apresentar preço;
* verificar se o jogador possui moedas;
* realizar compra;
* aplicar a carta;
* atualizar a UI.

Fluxo:

```text
Jogador seleciona carta
        ↓
ShopManager verifica preço
        ↓
Player possui moedas?
       ↙ ↘
     SIM  NÃO
      ↓     ↓
Compra   Não compra
      ↓
Aplica carta
      ↓
Atualiza UI
```

---

# 💳 16. Compra de carta

Antes da compra:

```text
Moedas = 100
Preço = 50
```

Depois:

```text
Moedas = 50
```

A compra deve falhar caso:

```text
Moedas < preço
```

Não permitir saldo negativo.

---

# 📚 17. Inventário de cartas

O jogador precisa possuir uma lista de cartas adquiridas.

Exemplo:

```csharp
List<CardData> ownedCards;
```

O sistema deve permitir:

* adicionar carta;
* verificar se possui carta;
* contar cartas;
* carregar cartas salvas posteriormente.

Caso as cartas possam ser compradas apenas uma vez:

```text
Carta comprada
↓
Não pode comprar novamente
```

Caso sejam upgrades acumuláveis:

```text
Carta nível 1
↓
Comprar novamente
↓
Carta nível 2
```

Para a primeira versão, recomenda-se **uma compra por carta**.

---

# ⚡ 18. Aplicação dos buffs

Quando uma carta é comprada:

```text
CardData
    ↓
CardManager
    ↓
PlayerStats
```

Exemplo:

```text
Click Power inicial = 1

Carta:
+2 Click Power

Resultado:
Click Power = 3
```

É importante separar:

```text
CardData
```

dos atributos atuais do jogador.

A carta representa os dados do upgrade.

O jogador possui o resultado desses upgrades.

---

# 👤 19. PlayerStats

Criar:

```text
PlayerStats.cs
```

Esse sistema deve armazenar atributos como:

```csharp
public int coins;
public int clickPower;
public int rewardPerClick;
```

Exemplo:

```text
coins = 0
clickPower = 1
rewardPerClick = 1
```

As cartas modificam esses valores.

---

# 🧍 20. Sistema de personagem

O personagem deve possuir um script simples responsável por controlar qual skin está equipada.

Criar:

```text
CharacterController.cs
```

O sistema deverá:

* conhecer a skin atual;
* trocar a skin;
* informar à UI qual skin está equipada.

A parte visual será feita pelos assets.

O código apenas precisa fornecer a lógica.

---

# 🎨 21. Sistema de skins

Assim como as cartas, as skins podem utilizar `ScriptableObject`.

Criar:

```text
SkinData.cs
```

Exemplo:

```csharp
[CreateAssetMenu(menuName = "Skins/Skin")]
public class SkinData : ScriptableObject
{
    public string skinName;
    public Sprite sprite;
    public int price;
}
```

Cada skin poderá ser criada pelo Unity:

```text
Skins/
├── Default.asset
├── Chef.asset
├── Knight.asset
└── Wizard.asset
```

O artista poderá substituir os sprites sem precisar alterar os scripts.

---

# 🔓 22. Compra e desbloqueio de skins

A skin deve possuir um preço.

Fluxo:

```text
Jogador seleciona skin
        ↓
Skin está desbloqueada?
      ↙   ↘
    SIM    NÃO
     ↓      ↓
 Equipar   Verificar moedas
              ↓
           Comprar
              ↓
           Desbloquear
              ↓
            Equipar
```

A skin inicial deve estar desbloqueada.

---

# 🖥️ 23. Interface do jogo

A programação da UI deve mostrar os dados atuais do jogo.

Elementos necessários:

```text
┌───────────────────────────────┐
│ 🪙 Moedas: 120                │
│                               │
│           🧀                  │
│                               │
│                               │
│      [ LOJA ]                 │
└───────────────────────────────┘
```

A UI deve mostrar pelo menos:

* moedas;
* poder do clique;
* botão da loja;
* cartas disponíveis;
* preço das cartas;
* skins;
* botão de compra;
* informação de carta adquirida.

---

# 🔢 24. Atualização da UI

A UI não deve ficar verificando constantemente os valores.

Evitar depender excessivamente de:

```csharp
Update()
```

para atualizar tudo.

Preferir eventos.

Exemplo conceitual:

```text
PlayerStats
    ↓
CoinsChanged
    ↓
UI
    ↓
Atualiza texto de moedas
```

Isso deixa o projeto mais organizado.

Para o primeiro jogo, porém, uma implementação simples também é aceitável. O mais importante é entender a separação entre **dados** e **interface**.

---

# 🎮 25. Input

O jogador precisa conseguir clicar no queijo.

Pode ser utilizado o sistema de input padrão do Unity ou o **Input System**.

Para este projeto, o input necessário inicialmente é:

```text
Mouse Left Click
```

Não é necessário implementar controles complexos.

---

# 🧱 26. Prefabs

Criar Prefabs para objetos que serão reutilizados.

Principalmente:

```text
Cheese.prefab
Character.prefab
CardUI.prefab
SkinUI.prefab
```

O queijo deve ser um Prefab porque será criado várias vezes durante o jogo.

---

# 🧠 27. GameManager

Criar:

```text
GameManager.cs
```

O `GameManager` será responsável pelo estado geral do jogo.

Exemplo:

```text
GameManager
├── inicia jogo
├── inicializa Player
├── inicializa CheeseSpawner
└── controla estado da partida
```

Não colocar toda a lógica do jogo dentro dele.

Evitar transformar o `GameManager` em um "script que faz tudo".

---

# 🔗 28. Comunicação entre sistemas

Uma arquitetura simples pode ser:

```text
                    GameManager
                         │
          ┌──────────────┼──────────────┐
          ↓              ↓              ↓
     PlayerStats    CheeseSpawner    ShopManager
          │              │              │
          ↓              ↓              ↓
      Rewards         Cheese          Cards
          │
          ↓
         UI
```

Exemplo de clique:

```text
CheeseController
       ↓
PlayerStats.ClickPower
       ↓
Cheese diminui
       ↓
RewardSystem
       ↓
PlayerStats.AddCoins()
       ↓
UI atualiza moedas
```

---

# 💾 29. Sistema de Save

O progresso deverá ser salvo.

Informações importantes:

```text
Moedas
Cartas compradas
Skins desbloqueadas
Skin equipada
```

Para a primeira versão, pode ser utilizado:

```text
PlayerPrefs
```

É suficiente para um projeto simples.

Porém, os dados devem ser organizados de maneira que posteriormente possam migrar para um sistema de save mais robusto.

---

# 💽 30. SaveManager

Criar:

```text
SaveManager.cs
```

Responsabilidades:

```text
Save()
Load()
```

Exemplo de fluxo:

```text
Jogo inicia
    ↓
Load()
    ↓
Recupera progresso
    ↓
Jogador joga
    ↓
Compra carta
    ↓
Save()
```

Salvar também quando necessário ao sair da aplicação.

---

# 🔄 31. Estado do jogo

Criar estados simples:

```text
MainMenu
Playing
Shop
Paused
```

Não é obrigatório implementar uma máquina de estados complexa.

Inicialmente, pode ser suficiente utilizar um enum:

```csharp
public enum GameState
{
    MainMenu,
    Playing,
    Shop,
    Paused
}
```

---

# 🧪 32. Testes

Mesmo sendo um jogo, os sistemas principais devem ser testados.

Criar testes para:

### Player

Verificar:

```text
Adicionar moedas
Gastar moedas
Impedir saldo negativo
```

### Clique

Verificar:

```text
Click Power = 1
Clique
Queijo reduz corretamente
```

### Recompensa

Verificar:

```text
1 clique
↓
1 recompensa
```

E:

```text
Click Power aumentado
↓
recompensa calculada corretamente
```

### Loja

Verificar:

```text
Possui dinheiro → compra funciona
Não possui dinheiro → compra falha
```

### Cartas

Verificar:

```text
Carta adicionada
Buff aplicado
Carta não pode ser comprada novamente
```

### Save

Verificar:

```text
Salvar
↓
Fechar
↓
Carregar
↓
Dados permanecem
```

---

# 🧪 33. Testes manuais dentro do Unity

Além dos testes automatizados, realizar testes jogando.

Checklist:

```text
[ ] Consigo clicar no queijo
[ ] Queijo diminui
[ ] Recebo moedas
[ ] Contador de moedas atualiza
[ ] Queijo desaparece ao chegar ao limite
[ ] Novo queijo aparece
[ ] Novo queijo cai
[ ] Consigo abrir a loja
[ ] Consigo comprar uma carta
[ ] Moedas são descontadas
[ ] Buff é aplicado
[ ] Não consigo comprar sem dinheiro
[ ] Consigo comprar uma skin
[ ] Consigo equipar uma skin
[ ] Progresso é salvo
[ ] Progresso é carregado
```

---

# 📋 34. Ordem recomendada de desenvolvimento

Como este é o primeiro jogo, **não desenvolver todos os sistemas simultaneamente**.

Seguir esta ordem:

## Fase 1 — Projeto

```text
[ ] Criar projeto Unity 2D
[ ] Criar estrutura de pastas
[ ] Criar cena Game
[ ] Configurar câmera
[ ] Criar GameObject do queijo
```

---

## Fase 2 — Primeiro protótipo

Objetivo: conseguir clicar no queijo.

```text
[ ] Criar CheeseController
[ ] Configurar Collider2D
[ ] Detectar clique
[ ] Reduzir tamanho
[ ] Detectar tamanho mínimo
```

Neste momento, o jogo já deve ser jogável.

---

## Fase 3 — Recompensa

```text
[ ] Criar PlayerStats
[ ] Criar sistema de moedas
[ ] Criar recompensa por clique
[ ] Atualizar contador de moedas
```

Resultado:

```text
Clique
↓
Queijo diminui
↓
Moeda aumenta
```

---

## Fase 4 — Respawn do queijo

```text
[ ] Criar CheeseSpawner
[ ] Criar Cheese Prefab
[ ] Remover queijo terminado
[ ] Criar novo queijo
[ ] Posicionar no topo
[ ] Implementar queda
```

Resultado:

```text
Queijo termina
↓
Novo queijo cai
```

---

## Fase 5 — Sistema de cartas

```text
[ ] Criar CardData
[ ] Criar ScriptableObject
[ ] Criar algumas cartas
[ ] Criar CardManager
[ ] Criar inventário de cartas
[ ] Aplicar buffs
```

---

## Fase 6 — Loja

```text
[ ] Criar ShopManager
[ ] Mostrar cartas
[ ] Mostrar preço
[ ] Implementar compra
[ ] Verificar moedas
[ ] Aplicar carta
[ ] Atualizar UI
```

---

## Fase 7 — Skins

```text
[ ] Criar SkinData
[ ] Criar ScriptableObjects
[ ] Criar SkinManager
[ ] Criar compra
[ ] Criar desbloqueio
[ ] Criar seleção
[ ] Aplicar sprite
```

---

## Fase 8 — Save

```text
[ ] Criar SaveManager
[ ] Salvar moedas
[ ] Salvar cartas
[ ] Salvar skins
[ ] Salvar skin equipada
[ ] Implementar Load()
```

---

## Fase 9 — Testes

```text
[ ] Testar clique
[ ] Testar recompensa
[ ] Testar queijo
[ ] Testar respawn
[ ] Testar cartas
[ ] Testar loja
[ ] Testar skins
[ ] Testar save
```

---

# 📁 35. Scripts previstos

Uma estrutura inicial possível:

```text
Scripts/
│
├── Core/
│   ├── GameManager.cs
│   └── GameState.cs
│
├── Player/
│   ├── PlayerStats.cs
│   └── PlayerInventory.cs
│
├── Cheese/
│   ├── CheeseController.cs
│   └── CheeseSpawner.cs
│
├── Cards/
│   ├── CardData.cs
│   └── CardManager.cs
│
├── Shop/
│   └── ShopManager.cs
│
├── Skins/
│   ├── SkinData.cs
│   └── SkinManager.cs
│
├── Save/
│   └── SaveManager.cs
│
└── UI/
    ├── GameUI.cs
    ├── ShopUI.cs
    ├── CardUI.cs
    └── SkinUI.cs
```

Não é necessário criar todos esses arquivos no primeiro dia.

Eles devem ser criados conforme cada sistema for desenvolvido.

---

# 🚫 36. O que NÃO implementar inicialmente

Para evitar aumentar demais a complexidade do primeiro projeto, não implementar na primeira versão:

* multiplayer;
* banco de dados;
* servidor;
* login;
* sistema online;
* ranking online;
* economia complexa;
* dezenas de tipos de carta;
* árvore de habilidades;
* sistema de achievements complexo;
* anúncios;
* microtransações;
* sistema de inventário extremamente complexo;
* física avançada;
* procedural generation.

O objetivo da primeira versão é criar um **jogo pequeno, completo e funcional**.

---

# 🏗️ 37. Princípios de programação

Durante o desenvolvimento, seguir algumas regras simples.

## Uma classe deve ter uma responsabilidade principal

Evitar:

```text
GameManager.cs
```

contendo:

```text
clique
moedas
cartas
loja
skins
save
UI
```

Separar os sistemas.

---

## Evitar números mágicos

Evitar:

```csharp
cheeseSize -= 0.05f;
```

espalhado pelo projeto.

Preferir:

```csharp
[SerializeField]
private float clickSizeReduction = 0.05f;
```

Assim o valor pode ser alterado diretamente no Inspector.

---

## Utilizar SerializeField

Em vez de tornar tudo público:

```csharp
public float cheeseSize;
```

preferir:

```csharp
[SerializeField]
private float cheeseSize;
```

quando outro script não precisa acessar diretamente o campo.

---

# 🔌 38. Dependências entre sistemas

Manter as dependências simples.

Exemplo:

```text
CheeseController
      ↓
PlayerStats
```

é aceitável.

Porém:

```text
CheeseController
 ↓
ShopManager
 ↓
SkinManager
 ↓
GameManager
 ↓
CheeseController
```

deve ser evitado.

Isso cria dependências circulares e dificulta a manutenção.

---

# 🧱 39. Critério de conclusão do MVP

A primeira versão do jogo será considerada funcional quando for possível:

```text
1. Iniciar o jogo
        ↓
2. Ver o queijo
        ↓
3. Clicar no queijo
        ↓
4. Queijo diminuir
        ↓
5. Receber moedas
        ↓
6. Queijo acabar
        ↓
7. Novo queijo cair
        ↓
8. Abrir loja
        ↓
9. Comprar carta
        ↓
10. Buff ser aplicado
        ↓
11. Comprar/desbloquear skin
        ↓
12. Equipar skin
        ↓
13. Fechar o jogo
        ↓
14. Abrir novamente
        ↓
15. Progresso continuar salvo
```

Se tudo isso funcionar, o **MVP está pronto**.

---

# 🚀 40. Melhorias futuras

Depois que o MVP estiver funcionando, podem ser adicionados:

* mais tipos de queijo;
* diferentes valores de recompensa;
* cartas raras;
* cartas com efeitos diferentes;
* sistema de níveis;
* combos;
* critical clicks;
* efeitos especiais;
* sons;
* partículas;
* conquistas;
* estatísticas;
* novos personagens;
* novas skins;
* progressão mais elaborada.

Esses sistemas devem ser adicionados **somente depois que o loop principal estiver funcionando**.

---

# 🗺️ 41. Visão geral da arquitetura

A arquitetura final esperada é aproximadamente:

```text
                         GAME
                          │
                    GameManager
                          │
             ┌────────────┼────────────┐
             │            │            │
             ↓            ↓            ↓
        PlayerStats   CheeseSpawner   ShopManager
             │            │            │
             │            ↓            ↓
             │         Cheese       CardManager
             │                           │
             ↓                           ↓
        RewardSystem                 CardData
             │
             ↓
            UI
             
             └──────────────┐
                            ↓
                       SaveManager
```

O princípio mais importante é:

> **Cada sistema deve cuidar da sua própria responsabilidade e conversar com os outros sistemas apenas quando necessário.**

---

# ✅ Checklist final

## Unity

* [ ] Projeto 2D criado
* [ ] Câmera configurada
* [ ] Cena principal criada
* [ ] Prefabs configurados
* [ ] Collider2D configurado
* [ ] UI criada
* [ ] ScriptableObjects configurados

## Gameplay

* [ ] Clique funcionando
* [ ] Queijo diminuindo
* [ ] Recompensa funcionando
* [ ] Moedas funcionando
* [ ] Queijo sendo substituído
* [ ] Novo queijo caindo

## Progressão

* [ ] Cartas funcionando
* [ ] Buffs funcionando
* [ ] Loja funcionando
* [ ] Skins funcionando
* [ ] Compra funcionando

## Persistência

* [ ] Save funcionando
* [ ] Load funcionando

## Qualidade

* [ ] Código separado por responsabilidade
* [ ] Sem números mágicos espalhados
* [ ] Prefabs reutilizáveis
* [ ] ScriptableObjects utilizados para dados
* [ ] Sistemas principais testados
* [ ] MVP concluído antes de adicionar funcionalidades extras

---

# 🎯 Objetivo técnico do projeto

O objetivo deste projeto não é apenas criar um jogo de clicar em queijo.

Ele também deve servir como um **primeiro projeto de aprendizado de Unity + C#**, permitindo praticar:

* classes;
* objetos;
* encapsulamento;
* listas;
* enums;
* métodos;
* eventos;
* composição;
* `MonoBehaviour`;
* `ScriptableObject`;
* Prefabs;
* componentes do Unity;
* colisores;
* Input;
* UI;
* gerenciamento de estado;
* persistência de dados;
* testes;
* organização de código.

A prioridade deve ser:

```text
FUNCIONAR
    ↓
ORGANIZAR
    ↓
TESTAR
    ↓
MELHORAR
```

e não tentar criar a arquitetura perfeita antes de ter o primeiro protótipo funcionando.
