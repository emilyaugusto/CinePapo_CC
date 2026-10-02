# 🎬 CinePapo 2.0 — Evolução e Melhorias

> Continuidade do projeto acadêmico CinePapo, desenvolvido no semestre anterior, com foco na implementação de melhorias, correções, otimizações e evolução da experiência do usuário.

---

## 📋 Informações Básicas

### Nome do Projeto Original

**CinePapo**

### Nome da Iniciativa de Melhorias

**CinePapo 2.0**

### Equipe de Melhorias

| Nome | GitHub | Papel Principal |
|------|--------|-----------------|
| Emily Pereira Augusto | @emilyaugusto | Desenvolvedora Full-Stack / UI/UX / Co-Gerenciamento |
| Thiago de Oliveira Freitas | @thiagofreitasx | Líder / Desenvolvedor Full-Stack / Banco de Dados / Co-Gerenciamento |

### Repositórios

- **Projeto Original:** A definir
- **Versão 2.0:** A definir

---

# 📋 Contexto do Projeto Original

## 1. Descrição do Projeto Original

### Propósito

O **CinePapo** é uma aplicação web desenvolvida como uma rede social voltada para fãs de cinema.

O projeto busca criar um ambiente próprio para que usuários possam descobrir filmes, compartilhar opiniões, publicar resenhas e interagir com outras pessoas que possuem interesse em cinema.

A proposta surgiu da necessidade de centralizar discussões sobre filmes que normalmente ficam dispersas em redes sociais genéricas, criando uma plataforma específica para esse tipo de interação.

Além da interação entre usuários, o CinePapo também funciona como uma ferramenta para descoberta e organização de filmes.

### Origem do Projeto

- [ ] Projeto pessoal anterior
- [x] Projeto de outra disciplina/semestre
- [ ] Projeto de trabalho/empresa
- [ ] Projeto open source
- [ ] Outro

**Detalhes:**

- **Quando foi criado:** 1º semestre de 2026
- **Contexto:** Projeto acadêmico desenvolvido no curso de Ciência da Computação da Universidade Nove de Julho — UNINOVE.
- **Desenvolvedores originais:** Emily Pereira Augusto e Thiago de Oliveira Freitas.
- **Status atual:** Projeto funcional em continuidade.
- **Objetivo atual:** Dar continuidade ao projeto durante o novo semestre, implementando ajustes, correções e melhorias sobre a versão já desenvolvida.

---

# ⚙️ Funcionalidades Existentes

## 2. Funcionalidades Implementadas

A primeira versão do CinePapo já possui uma base funcional da rede social.

### 🔐 Autenticação

- ✅ Login com e-mail e senha.
- ✅ Criação de conta.
- ✅ Integração de autenticação com Google.
- ✅ Validação para impedir duplicidade de usuários.

### 👤 Criação e Personalização de Perfil

- ✅ Fluxo inicial para novos usuários.
- ✅ Definição de nome de usuário.
- ✅ Seleção de preferências.
- ✅ Seleção de gêneros cinematográficos.
- ✅ Seleção de serviços/plataformas onde o usuário assiste aos filmes.
- ✅ Edição de informações do perfil.

### 🏠 Feed / Página Inicial

- ✅ Visualização de resenhas publicadas por usuários.
- ✅ Feed de publicações.
- ✅ Filtro entre conteúdo **Global** e **Seguindo**.
- ✅ Infinite Scrolling para carregamento contínuo de publicações.

### 🔎 Explorar

- ✅ Descoberta de filmes.
- ✅ Visualização de lançamentos recentes.
- ✅ Pesquisa de filmes.
- ✅ Pesquisa de usuários.

### 🎬 Filmes

- ✅ Página individual do filme.
- ✅ Exibição de informações como título, banner, ano de lançamento e elenco.
- ✅ Integração com dados externos da API TMDB.
- ✅ Adição de filmes à lista de assistidos.
- ✅ Adição à lista "Quero Ver".
- ✅ Adição aos favoritos.
- ✅ Publicação de resenhas.

### ⭐ Resenhas e Interação

- ✅ Publicação de resenhas.
- ✅ Visualização de resenhas de outros usuários.
- ✅ Curtidas.
- ✅ Comentários.
- ✅ Interação entre usuários.

### 🔔 Notificações

- ✅ Área de notificações.
- ✅ Notificações relacionadas às interações realizadas no perfil e nas publicações.

### 👤 Perfil

- ✅ Visualização das informações do usuário.
- ✅ Edição de perfil.
- ✅ Visualização das resenhas publicadas.
- ✅ Lista de filmes assistidos.
- ✅ Lista "Quero Ver".
- ✅ Lista de favoritos.

---

# 👥 Casos de Uso Existentes

## Caso de Uso 1 — Criar uma conta

**Ator:** Novo usuário

**Fluxo:**

1. Usuário acessa o CinePapo.
2. Seleciona a opção de criação de conta ou autenticação.
3. Realiza cadastro ou autenticação via Google.
4. O sistema identifica que se trata de um novo usuário.
5. O usuário passa pelo processo de criação do perfil.
6. Informa suas preferências.
7. Finaliza o cadastro.

**Resultado:**  
O usuário passa a possuir um perfil no CinePapo e pode utilizar as funcionalidades da plataforma.

---

## Caso de Uso 2 — Explorar filmes

**Ator:** Usuário autenticado

**Fluxo:**

1. Usuário acessa a página **Explorar**.
2. Visualiza filmes disponíveis e lançamentos.
3. Pode utilizar a pesquisa para localizar um filme específico.
4. Seleciona um filme.
5. Visualiza suas informações detalhadas.

**Resultado:**  
O usuário encontra novos filmes e pode interagir com o conteúdo relacionado à obra.

---

## Caso de Uso 3 — Avaliar um filme

**Ator:** Usuário autenticado

**Fluxo:**

1. Usuário pesquisa ou seleciona um filme.
2. Acessa a página do filme.
3. Seleciona a opção de escrever uma resenha.
4. Publica sua opinião.
5. A publicação passa a integrar a plataforma.

**Resultado:**  
A resenha fica disponível para interação de outros usuários.

---

## Caso de Uso 4 — Organizar filmes

**Ator:** Usuário autenticado

**Fluxo:**

1. Usuário acessa um filme.
2. Seleciona uma das opções disponíveis.
3. Adiciona o filme como:
   - Visto;
   - Quero Ver;
   - Favorito.

**Resultado:**  
O filme passa a fazer parte da respectiva lista no perfil do usuário.

---

# 🧰 Stack Tecnológica Original

| Componente | Tecnologia | Uso |
|------------|------------|-----|
| **Frontend** | ASP.NET Core MVC / HTML / CSS / JavaScript | Interface da aplicação |
| **Backend** | C# / ASP.NET Core MVC | Regras de negócio e processamento |
| **Banco de Dados** | MongoDB | Persistência dos dados |
| **Arquitetura** | MVC | Organização da aplicação |
| **Autenticação** | Google OAuth | Login integrado com conta Google |
| **API Externa** | TMDB API | Informações e catálogo de filmes |
| **UI/UX** | Figma | Prototipação das interfaces |
| **Fluxos** | Miro | Planejamento da jornada e fluxos |
| **Versionamento** | Git / GitHub | Controle de versão |
| **Gestão** | Kanban / Notion | Organização das tarefas |
| **IDE** | Visual Studio / Visual Studio Code | Desenvolvimento |

> As versões específicas das tecnologias deverão ser confirmadas diretamente no projeto antes da documentação final.

---

# 🏗️ Arquitetura Atual

O CinePapo utiliza o padrão arquitetural **MVC — Model, View, Controller**.

```text
┌───────────────────────────────┐
│             VIEW              │
│                               │
│ Interface / páginas CinePapo  │
│ HTML • CSS • JavaScript       │
└───────────────┬───────────────┘
                │
                ↓
┌───────────────────────────────┐
│          CONTROLLER           │
│                               │
│      ASP.NET Core / C#        │
│ Requisições e regras do fluxo │
└───────────────┬───────────────┘
                │
                ↓
┌───────────────────────────────┐
│             MODEL             │
│                               │
│      Modelos e serviços       │
└──────────┬───────────┬────────┘
           │           │
           ↓           ↓
┌─────────────────┐ ┌─────────────────┐
│     MongoDB     │ │    TMDB API     │
│ Dados internos  │ │ Dados de filmes │
└─────────────────┘ └─────────────────┘

````

### Descrição dos componentes

**View**  
Responsável pela interface utilizada pelos usuários, incluindo páginas como feed, explorar, perfil, login e detalhes dos filmes.

**Controller**  
Recebe as ações realizadas pelos usuários, processa as requisições e realiza a comunicação entre interface, serviços e dados.

**Model**  
Representa as estruturas e entidades utilizadas pela aplicação.

**MongoDB**  
Responsável pelo armazenamento dos dados internos da rede social.

**TMDB API**  
Fornece dados externos relacionados aos filmes, como títulos, imagens, sinopses, elenco e demais informações cinematográficas.

---

# 🔍 Análise da Versão Original

O desenvolvimento realizado no semestre anterior permitiu construir a base funcional do CinePapo.

Neste novo semestre, o projeto entra em uma etapa de **evolução**, utilizando a aplicação existente como ponto de partida para identificar oportunidades de melhoria.

## Pontos que serão analisados

- [ ] Experiência do usuário.
- [ ] Interface e responsividade.
- [ ] Fluxos existentes.
- [ ] Performance da aplicação.
- [ ] Organização e qualidade do código.
- [ ] Integração com a API TMDB.
- [ ] Estrutura do banco MongoDB.
- [ ] Segurança e autenticação.
- [ ] Tratamento de erros.
- [ ] Funcionalidades sociais.
- [ ] Compatibilidade entre desktop e dispositivos móveis.

---

# 📊 Análise SWOT do Projeto Original

## Forças — Strengths

- Aplicação já possui uma base funcional.
- Projeto desenvolvido com arquitetura MVC.
- Utilização de C# e ASP.NET Core.
- Banco de dados NoSQL adequado às estruturas dinâmicas da aplicação.
- Integração com uma base externa de filmes.
- Identidade visual própria.
- Fluxos de UI/UX previamente planejados.
- Recursos de interação social já presentes.
- Organização de filmes por preferências do usuário.

## Fraquezas — Weaknesses

- Projeto ainda necessita de validações mais aprofundadas com usuários.
- Existem oportunidades de melhoria na experiência e nos fluxos.
- Necessidade de revisão e refatoração do código desenvolvido na primeira versão.
- Performance e responsividade precisam ser novamente analisadas.

## Oportunidades — Opportunities

- Criar novas funcionalidades sociais.
- Melhorar o sistema de recomendações.
- Evoluir a experiência mobile.
- Melhorar acessibilidade.
- Aprimorar mecanismos de busca e descoberta.
- Criar testes automatizados.
- Otimizar performance.
- Evoluir a arquitetura existente sem reconstruir todo o projeto.

## Ameaças — Threats

- Dependência da disponibilidade da API externa TMDB.
- Alterações futuras nos serviços externos utilizados.
- Crescimento do volume de dados e necessidade de otimização.
- Possíveis vulnerabilidades decorrentes da evolução das funcionalidades sociais.
- Introdução de regressões durante as alterações do sistema existente.

---

# 🚀 Melhorias Propostas

## 1. Objetivos das Melhorias

### Objetivo 1 — Evoluir a experiência do usuário

Revisar os fluxos já desenvolvidos e identificar pontos que possam ser simplificados ou aprimorados.

**Justificativa:**  
A primeira versão priorizou a construção das funcionalidades principais. A continuidade permitirá analisar a experiência completa e aperfeiçoar a interação com a plataforma.

**Métricas de sucesso:**

- Redução de dificuldades nos principais fluxos.
- Melhoria da navegação.
- Feedback positivo durante testes com usuários.
- Melhor experiência em diferentes tamanhos de tela.

### Objetivo 2 — Melhorar a qualidade técnica da aplicação

Revisar código, estrutura e desempenho do sistema sem comprometer as funcionalidades existentes.

**Justificativa:**  
Como o CinePapo continuará evoluindo, manter uma base de código organizada facilitará novas implementações e manutenções futuras.

**Métricas de sucesso:**

- Redução de código duplicado.
- Melhor organização das responsabilidades.
- Redução de erros.
- Manutenção ou melhoria da performance.
- Funcionalidades antigas funcionando após as alterações.

### Objetivo 3 — Expandir o CinePapo

Implementar novas funcionalidades que complementem a experiência social e cinematográfica da plataforma.

**Justificativa:**  
A base desenvolvida no semestre anterior permite que o projeto avance além de um MVP inicial e receba recursos mais completos.

**Métricas de sucesso:**

- Novas funcionalidades integradas ao sistema.
- Integração correta com recursos existentes.
- Funcionalidades testadas e documentadas.

---

# 📝 Melhorias Planejadas

> As melhorias específicas serão definidas após a análise técnica e de usabilidade da versão desenvolvida no semestre anterior.

## Melhoria 1 — A definir

**Categoria:**

- [ ] Nova Funcionalidade
- [ ] Correção
- [ ] Otimização
- [ ] Refatoração
- [ ] Segurança

**Problema que resolve:**

> A definir após análise da versão atual.

**Solução proposta:**

> A definir.

**Impacto esperado:**

> A definir.

**Complexidade:** A definir  
**Responsável:** A definir

---

## Melhoria 2 — A definir

**Categoria:**

- [ ] Nova Funcionalidade
- [ ] Correção
- [ ] Otimização
- [ ] Refatoração
- [ ] Segurança

**Problema que resolve:**

> A definir.

**Solução proposta:**

> A definir.

**Impacto esperado:**

> A definir.

**Complexidade:** A definir  
**Responsável:** A definir

---

## Melhoria 3 — A definir

**Categoria:**

- [ ] Nova Funcionalidade
- [ ] Correção
- [ ] Otimização
- [ ] Refatoração
- [ ] Segurança

**Problema que resolve:**

> A definir.

**Solução proposta:**

> A definir.

**Impacto esperado:**

> A definir.

**Complexidade:** A definir  
**Responsável:** A definir

---

# 🛠️ Especificações Técnicas das Melhorias

## Arquitetura Proposta

Inicialmente, o projeto continuará utilizando a arquitetura **MVC**, preservando a estrutura construída anteriormente.

```text
┌───────────────────────────────┐
│          INTERFACE            │
│                               │
│     CinePapo 2.0 / Views      │
└───────────────┬───────────────┘
                │
                ↓
┌───────────────────────────────┐
│          CONTROLLERS          │
│                               │
│      ASP.NET Core / C#        │
└───────────────┬───────────────┘
                │
                ↓
┌───────────────────────────────┐
│      MODELS / SERVICES        │
└──────────┬───────────┬────────┘
           │           │
           ↓           ↓
┌─────────────────┐ ┌─────────────────┐
│     MongoDB     │ │    TMDB API     │
└─────────────────┘ └─────────────────┘
```

### Mudanças previstas

- [x] Manutenção da arquitetura MVC.
- [x] Evolução dos componentes existentes.
- [x] Refatoração quando necessária.
- [ ] Adição de novos componentes.
- [ ] Novas bibliotecas.
- [ ] Alterações na estrutura do banco.
- [ ] Novas integrações externas.

As alterações serão documentadas conforme forem definidas e implementadas durante o semestre.

---

# 📅 Plano de Implementação

## Cronograma — 3 meses

### Fase 1 — Análise e Preparação | Semanas 1–2

- [ ] Revisar o código desenvolvido no semestre anterior.
- [ ] Configurar o ambiente de desenvolvimento.
- [ ] Validar funcionamento da aplicação.
- [ ] Revisar arquitetura atual.
- [ ] Identificar bugs e limitações.
- [ ] Analisar experiência do usuário.
- [ ] Definir melhorias prioritárias.
- [ ] Organizar backlog no Kanban.
- [ ] Definir responsabilidades da equipe.

### Fase 2 — Primeiras Melhorias | Semanas 3–6

- [ ] Implementar correções prioritárias.
- [ ] Iniciar melhorias de UI/UX.
- [ ] Refatorar pontos necessários.
- [ ] Desenvolver primeira nova melhoria/funcionalidade.
- [ ] Realizar testes durante o desenvolvimento.
- [ ] Validar funcionalidades existentes após alterações.

### Fase 3 — Evolução | Semanas 7–9

- [ ] Implementar melhorias restantes.
- [ ] Desenvolver novas funcionalidades definidas.
- [ ] Revisar integração com TMDB.
- [ ] Realizar otimizações.
- [ ] Integrar todas as alterações.
- [ ] Realizar testes de regressão.

### Fase 4 — Finalização | Semanas 10–12

- [ ] Corrigir bugs identificados.
- [ ] Realizar testes completos.
- [ ] Testar com usuários.
- [ ] Aplicar ajustes baseados nos feedbacks.
- [ ] Documentar versão final.
- [ ] Registrar comparação antes/depois.
- [ ] Atualizar README.
- [ ] Preparar apresentação final.

---

# 🧪 Estratégia de Testes

Para garantir que a evolução do CinePapo não comprometa funcionalidades desenvolvidas anteriormente, serão realizados testes durante todo o processo.

### Funcionalidades que deverão ser validadas

- [ ] Login.
- [ ] Cadastro.
- [ ] Login com Google.
- [ ] Criação de perfil.
- [ ] Preferências do usuário.
- [ ] Feed.
- [ ] Infinite Scrolling.
- [ ] Explorar.
- [ ] Pesquisa de filmes.
- [ ] Pesquisa de usuários.
- [ ] Página do filme.
- [ ] Publicação de resenhas.
- [ ] Comentários.
- [ ] Curtidas.
- [ ] Favoritos.
- [ ] Quero Ver.
- [ ] Filmes assistidos.
- [ ] Perfil.
- [ ] Notificações.
- [ ] Integração com TMDB.

### Estratégias

- [x] Testes manuais.
- [ ] Testes automatizados.
- [ ] Testes de regressão.
- [ ] Testes de responsividade.
- [ ] Testes de usabilidade.
- [ ] Testes com usuários.

---

# 📊 Comparação Antes / Depois

Esta seção será atualizada conforme as melhorias forem implementadas.

| Funcionalidade | CinePapo Original | CinePapo 2.0 | Status |
|----------------|-------------------|--------------|--------|
| Login | ✅ Implementado | A avaliar | ⏳ |
| Google Login | ✅ Implementado | A avaliar | ⏳ |
| Onboarding | ✅ Implementado | A melhorar | ⏳ |
| Feed | ✅ Implementado | A melhorar | ⏳ |
| Explorar | ✅ Implementado | A melhorar | ⏳ |
| Perfil | ✅ Implementado | A melhorar | ⏳ |
| Resenhas | ✅ Implementado | A melhorar | ⏳ |
| Listas de filmes | ✅ Implementado | A melhorar | ⏳ |
| Notificações | ✅ Implementado | A melhorar | ⏳ |
| Novas funcionalidades | ❌ | A definir | ⏳ |

---

# 📸 Evidências Visuais

## Versão Original

As imagens da primeira versão podem ser armazenadas em:

```text
docs/
└── antes/
    ├── landing-page.png
    ├── login.png
    ├── onboarding.png
    ├── pagina-inicial.png
    ├── explorar.png
    ├── perfil.png
    └── filme.png
```

Exemplo de uso no README:

```markdown
![Landing Page - CinePapo Original](docs/antes/landing-page.png)
![Página Inicial - CinePapo Original](docs/antes/pagina-inicial.png)
![Explorar - CinePapo Original](docs/antes/explorar.png)
```

## Versão Melhorada

```text
docs/
└── depois/
    ├── landing-page.png
    ├── login.png
    ├── onboarding.png
    ├── pagina-inicial.png
    ├── explorar.png
    ├── perfil.png
    └── filme.png
```

---

# 🎓 Aprendizados do Projeto Original

O desenvolvimento da primeira versão permitiu aplicar conceitos estudados durante o curso em uma aplicação completa.

Entre os principais conhecimentos utilizados estão:

- Arquitetura MVC.
- Desenvolvimento com C#.
- ASP.NET Core.
- Banco de dados NoSQL.
- MongoDB.
- Consumo de APIs externas.
- Autenticação.
- Desenvolvimento de interfaces.
- UI/UX.
- Prototipação com Figma.
- Controle de versão com Git e GitHub.
- Organização de projeto utilizando Kanban.
- Trabalho em equipe.
- Integração entre frontend, backend e banco de dados.

A continuidade do projeto permitirá trabalhar também com manutenção e evolução de software existente, aproximando o desenvolvimento de situações encontradas em projetos reais.

---

# 🔒 Considerações de Segurança

Durante o desenvolvimento do CinePapo 2.0 serão revisados aspectos relacionados à segurança da aplicação.

### Pontos de análise

- [ ] Autenticação dos usuários.
- [ ] Integração com Google OAuth.
- [ ] Proteção de dados.
- [ ] Validação de entradas.
- [ ] Controle de acesso.
- [ ] Tratamento de sessões.
- [ ] Proteção das credenciais da API TMDB.
- [ ] Tratamento de erros.
- [ ] Dependências utilizadas pelo projeto.

> Vulnerabilidades específicas somente serão registradas após análise técnica da versão atual.

---

# 🌍 Impacto das Melhorias

## Benefícios para os usuários

A evolução do CinePapo pretende proporcionar:

- Navegação mais intuitiva.
- Melhor experiência em dispositivos diferentes.
- Maior estabilidade.
- Melhor desempenho.
- Novas possibilidades de interação.
- Melhor experiência na descoberta de filmes.
- Evolução das funcionalidades sociais existentes.

## Impacto do Projeto

O CinePapo busca continuar evoluindo como um ambiente digital dedicado à comunidade cinéfila, permitindo que usuários descubram filmes, compartilhem opiniões e interajam com outras pessoas a partir de interesses cinematográficos em comum.

---

# 📚 Referências

## Projeto Original

- Documentação acadêmica do CinePapo — 2026.
- Código-fonte da primeira versão.
- Protótipos desenvolvidos no Figma.
- Fluxos desenvolvidos no Miro.
- Histórico do repositório Git/GitHub.

## Referências Técnicas

- Microsoft — Documentação C# e ASP.NET Core.
- MongoDB — Documentação oficial.
- GitHub — Documentação Git/GitHub.
- TMDB — API Documentation.
- Figma — Documentação.
- Google — OAuth.

## Literatura

- FOWLER, Martin. **Refactoring: Improving the Design of Existing Code**. 2ª ed. Addison-Wesley, 2018.
- MARTIN, Robert C. **Código Limpo**. Alta Books, 2009.

---

# ✅ Validação das Melhorias

## Funcionalidades Originais

- [ ] Login continua funcionando.
- [ ] Cadastro continua funcionando.
- [ ] Autenticação Google continua funcionando.
- [ ] Perfis continuam funcionando.
- [ ] Feed continua funcionando.
- [ ] Pesquisa continua funcionando.
- [ ] Resenhas continuam funcionando.
- [ ] Interações continuam funcionando.
- [ ] Listas de filmes continuam funcionando.
- [ ] Integração TMDB continua funcionando.
- [ ] Dados existentes continuam compatíveis.
- [ ] Nenhuma regressão crítica foi introduzida.

## Melhorias

- [ ] Melhorias planejadas implementadas.
- [ ] Melhorias testadas.
- [ ] Código revisado.
- [ ] Responsividade validada.
- [ ] Performance validada.
- [ ] Feedback dos usuários analisado.

## Documentação

- [ ] README atualizado.
- [ ] Melhorias documentadas.
- [ ] Screenshots antes/depois adicionados.
- [ ] Changelog atualizado.
- [ ] Documentação final atualizada.

---

# 📝 Changelog

## CinePapo 2.0 — Em desenvolvimento

### ⭐ Adicionado

- A definir durante o desenvolvimento.

### 🔧 Corrigido

- A definir durante o desenvolvimento.

### ⚡ Melhorado

- A definir durante o desenvolvimento.

### ♻️ Refatorado

- A definir durante o desenvolvimento.

### 🔒 Segurança

- A definir após análise técnica.

---

# 🎯 Próximos Passos

O projeto desenvolvido no semestre anterior será utilizado como base para a nova etapa de desenvolvimento.

### Etapas imediatas

- [ ] Executar e revisar a versão atual.
- [ ] Analisar o código existente.
- [ ] Mapear bugs.
- [ ] Identificar limitações de usabilidade.
- [ ] Definir melhorias prioritárias.
- [ ] Criar backlog.
- [ ] Distribuir atividades entre os integrantes.
- [ ] Iniciar desenvolvimento do CinePapo 2.0.

---

# 🎬 Conclusão

O **CinePapo 2.0** representa a continuidade do projeto desenvolvido no semestre anterior.

Diferentemente da primeira etapa, cujo principal objetivo foi desenvolver a estrutura e as funcionalidades essenciais da plataforma, esta nova fase será direcionada à **evolução de um software já existente**.

O foco será analisar a solução desenvolvida, identificar oportunidades de melhoria e implementar ajustes técnicos e funcionais sem comprometer os recursos que já estão funcionando.

Ao final do semestre, espera-se obter uma versão mais completa, estável e aprimorada do CinePapo, documentando também a evolução entre a versão original e a versão melhorada.

---

<div align="center">

## 🍿 CINEPAPO

**De fãs para fãs de cinema.**

🎬 **CinePapo 2.0 — Em desenvolvimento**

</div>
