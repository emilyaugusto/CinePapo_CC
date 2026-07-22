<p align="center">
  <img src="https://raw.githubusercontent.com/emilyaugusto/CinePapo-Portfolio/master/SaaSDocumentacao/wwwroot/img/capa_topo.png" alt="Capa" width="100%">
</p>

# 🍿 CinePapo

O CinePapo é uma rede social feita de fãs para fãs de cinema! Este web app foi desenvolvido para ser o espaço definitivo para dar notas, criar listas de favoritos, receber recomendações e debater filmes. Criado como um desafio de desenvolvimento concluído em 3 meses, o projeto foca totalmente na experiência do usuário.

## Funcionalidades

* **Sistema de Avaliações:** Dê notas e escreva resenhas completas sobre os filmes que assistiu.
* **Listas Personalizadas:** Organize seu diário cinematográfico separando filmes em "Visto", "Quero Ver" e "Favoritos".
* **Recomendações Inteligentes:** Sistema de sugestões baseadas no seu gosto pessoal e histórico.
* **Login Seguro e Rápido:** Autenticação integrada diretamente com a sua conta do Google.
* **Catálogo Rico:** Dados, capas e sinopses em tempo real puxados da API do TMDB.

## Tecnologias Utilizadas

* **Back-end:** C# com ASP.NET Core MVC
* **Banco de Dados:** MongoDB (NoSQL)
* **Autenticação:** Google OAuth 2.0
* **Integrações:** API do TMDB
* **Deploy/Hospedagem:** Railway

## Como rodar o projeto localmente

1. Faça o clone deste repositório para a sua máquina:
   git clone https://github.com/emilyaugusto/CinePapo-Portfolio.git

2. Abra o terminal na pasta do projeto e restaure as dependências do .NET:
   dotnet restore

3. Configure as variáveis de ambiente com as suas próprias chaves utilizando o User Secrets:
   dotnet user-secrets init
   dotnet user-secrets set "CinepapoDatabase:ConnectionString" "SUA_CHAVE_MONGO"
   dotnet user-secrets set "Authentication:Google:ClientId" "SEU_CLIENT_ID"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "SEU_CLIENT_SECRET"
   dotnet user-secrets set "TMDB:ApiKey" "SUA_API_KEY_TMDB"

4. Execute o projeto:
   dotnet run

## 👥 Autores

* Desenvolvido por Emily Augusto e Thiago Freitas.
* Orientação do professor Wanderlei.

<p align="center">
  <img src="https://raw.githubusercontent.com/emilyaugusto/CinePapo-Portfolio/master/SaaSDocumentacao/wwwroot/img/Footer%201.png" alt="Footer" width="100%">
</p>
