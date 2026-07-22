using System.Text.Json;
using CinePapo.Models;

namespace CinePapo.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // =========================================================================================
        // 1. CONFIGURAÇÃO E CONSTRUTOR
        // Prepara a comunicação com a API do TMDB definindo a URL base e a chave de segurança.
        // =========================================================================================
        public TmdbService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            // Pega a URL ou usa um padrão se falhar
            var baseUrl = config["TMDB:BaseUrl"] ?? "https://api.themoviedb.org/3/";
            _httpClient.BaseAddress = new Uri(baseUrl);

            // Pega a chave ou deixa vazio se falhar
            _apiKey = config["TMDB:ApiKey"] ?? string.Empty;
        }


        // =========================================================================================
        // 2. BUSCAS GERAIS E PESQUISA
        // Métodos para listar os filmes mais populares do momento ou pesquisar um filme pelo nome.
        // =========================================================================================

        // Busca os filmes que estão "Em Alta" (Trending) no momento, com suporte a paginação.
        public async Task<List<TmdbMovie>> GetTrendingMoviesAsync(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync($"trending/movie/week?api_key={_apiKey}&language=pt-BR&page={page}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbResponse>(jsonString);

                return result?.Results ?? new List<TmdbMovie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar filmes: {ex.Message}");
                return new List<TmdbMovie>();
            }
        }

        // Pesquisa filmes específicos pelo título digitado pelo usuário na barra de busca.
        public async Task<List<TmdbMovie>> SearchMoviesAsync(string query, int page = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return new List<TmdbMovie>();

                var urlQuery = Uri.EscapeDataString(query);
                var response = await _httpClient.GetAsync($"search/movie?api_key={_apiKey}&language=pt-BR&query={urlQuery}&page={page}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbResponse>(jsonString);

                return result?.Results ?? new List<TmdbMovie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pesquisar filmes: {ex.Message}");
                return new List<TmdbMovie>();
            }
        }


        // =========================================================================================
        // 3. DETALHES ESPECÍFICOS
        // Traz todas as informações de um único filme para a página de detalhes.
        // =========================================================================================

        // Busca a ficha técnica completa do filme, adicionando também o elenco e a equipe (credits).
        public async Task<TmdbMovie?> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                // O &append_to_response=credits traz os atores na mesma requisição
                var response = await _httpClient.GetAsync($"movie/{movieId}?api_key={_apiKey}&language=pt-BR&append_to_response=credits");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                // Lemos o JSON ignorando se a letra está maiúscula ou minúscula (mais seguro)
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var movie = JsonSerializer.Deserialize<TmdbMovie>(jsonString, options);

                return movie;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar detalhes do filme: {ex.Message}");
                return null;
            }
        }


        // =========================================================================================
        // 4. RECOMENDAÇÕES E DESCOBERTAS
        // Métodos inteligentes de filtragem usados no feed e na página de explorar.
        // =========================================================================================

        // Traz uma lista de filmes filtrada por um ÚNICO gênero específico.
        public async Task<List<TmdbMovie>> GetMoviesByGenreAsync(string genreName, int page = 1)
        {
            // Dicionário que traduz o nome do gênero (em português) para o ID numérico do TMDB
            var genreMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
                {"Ação", 28}, {"Aventura", 12}, {"Animação", 16}, {"Comédia", 35},
                {"Crime", 80}, {"Documentário", 99}, {"Drama", 18}, {"Família", 10751},
                {"Fantasia", 14}, {"História", 36}, {"Terror", 27}, {"Música", 10402},
                {"Mistério", 9648}, {"Romance", 10749}, {"Ficção científica", 878},
                {"Cinema TV", 10770}, {"Thriller", 53}, {"Guerra", 10752}, {"Faroeste", 37}
            };

            // Se não encontrar o gênero exato na lista, usa Ação (ID 28) como plano B de segurança
            int genreId = genreMap.ContainsKey(genreName) ? genreMap[genreName] : 28;

            try
            {
                // Ordena por popularidade e garante que o filme tem pelo menos 300 avaliações
                string urlParams = $"discover/movie?api_key={_apiKey}&language=pt-BR&with_genres={genreId}&page={page}&sort_by=popularity.desc&vote_count.gte=300";

                var response = await _httpClient.GetAsync(urlParams);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<TmdbResponse>(jsonString, options);

                return result?.Results ?? new List<TmdbMovie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar filmes por gênero: {ex.Message}");
                return new List<TmdbMovie>();
            }
        }

        // Traz uma lista de filmes misturando VÁRIOS gêneros de uma vez (Gênero A OU Gênero B).
        public async Task<List<TmdbMovie>> GetMixedRecommendationsAsync(List<string> genreNames, int page = 1)
        {
            var genreMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
                {"Ação", 28}, {"Aventura", 12}, {"Animação", 16}, {"Comédia", 35},
                {"Crime", 80}, {"Documentário", 99}, {"Drama", 18}, {"Família", 10751},
                {"Fantasia", 14}, {"História", 36}, {"Terror", 27}, {"Música", 10402},
                {"Mistério", 9648}, {"Romance", 10749}, {"Ficção científica", 878},
                {"Cinema TV", 10770}, {"Thriller", 53}, {"Guerra", 10752}, {"Faroeste", 37}
            };

            var genreIds = genreNames.Where(g => genreMap.ContainsKey(g))
                                     .Select(g => genreMap[g].ToString());

            string joinedIds = string.Join(",", genreIds);
            if (string.IsNullOrEmpty(joinedIds)) joinedIds = "28"; // Fallback para ação

            try
            {
                var response = await _httpClient.GetAsync($"discover/movie?api_key={_apiKey}&language=pt-BR&with_genres={joinedIds}&page={page}&sort_by=popularity.desc&vote_count.gte=100");
                response.EnsureSuccessStatusCode();

                var result = JsonSerializer.Deserialize<TmdbResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Results ?? new List<TmdbMovie>();
            }
            catch
            {
                return new List<TmdbMovie>();
            }
        }

        // Encontra filmes baseados no perfil de um filme específico (Filmes parecidos com X).
        public async Task<List<TmdbMovie>> GetSimilarMoviesAsync(int movieId, int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync($"movie/{movieId}/similar?api_key={_apiKey}&language=pt-BR&page={page}");
                response.EnsureSuccessStatusCode();

                var result = JsonSerializer.Deserialize<TmdbResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Results ?? new List<TmdbMovie>();
            }
            catch
            {
                return new List<TmdbMovie>();
            }
        }
    }
}