using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CinePapo.Models;
using CinePapo.Services;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CinePapo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Review> _reviewsCollection;
        private readonly IMongoCollection<Notification> _notificationsCollection;
        private readonly TmdbService _tmdbService;

        // =========================================================================================
        // CONSTRUTOR DO CONTROLADOR
        // Responsável por injetar os serviços necessários, como a API do TMDB e o banco de dados MongoDB.
        // =========================================================================================
        public HomeController(ILogger<HomeController> logger, IMongoDatabase database, TmdbService tmdbService)
        {
            _logger = logger;
            _tmdbService = tmdbService;
            _usersCollection = database.GetCollection<User>("Users");
            _reviewsCollection = database.GetCollection<Review>("Reviews");
            _notificationsCollection = database.GetCollection<Notification>("Notifications");
        }


        // =========================================================================================
        // 1. PÁGINAS PRINCIPAIS DO SISTEMA
        // =========================================================================================

        // PÁGINA INICIAL (INDEX)
        // Se o usuário estiver logado, exibe o Feed (com resenhas da comunidade e sugestões baseadas no gosto dele).
        // Se não estiver logado, exibe a Landing Page de apresentação do CinePapo.
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index(string filter = "global")
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();

                ViewBag.HasUnreadNotifications = await _notificationsCollection.Find(n => n.UserId == currentUser.Id && !n.IsRead).AnyAsync();

                List<Review> feedReviews;

                if (filter == "seguindo" && currentUser?.FollowingIds != null && currentUser.FollowingIds.Any())
                {
                    var targetIds = new List<string>(currentUser.FollowingIds) { currentUser.Id! };
                    feedReviews = await _reviewsCollection.Find(r => targetIds.Contains(r.UserId))
                                                          .SortByDescending(r => r.CreatedAt)
                                                          .Limit(5).ToListAsync();
                }
                else
                {
                    feedReviews = await _reviewsCollection.Find(_ => true)
                                                          .SortByDescending(r => r.CreatedAt)
                                                          .Limit(5).ToListAsync();
                }

                var authorIds = feedReviews.Select(r => r.UserId)
                                           .Where(id => MongoDB.Bson.ObjectId.TryParse(id, out _))
                                           .Distinct().ToList();

                var projection = Builders<User>.Projection
                    .Include(u => u.Id).Include(u => u.Name).Include(u => u.Username).Include(u => u.ProfilePicture);

                var authors = await _usersCollection.Find(u => authorIds.Contains(u.Id)).Project<User>(projection).ToListAsync();

                var suggestedMovies = new List<TmdbMovie>();
                if (currentUser?.FavoriteGenres != null && currentUser.FavoriteGenres.Any())
                {
                    var random = new Random();
                    var randomGenre = currentUser.FavoriteGenres.OrderBy(x => random.Next()).First();
                    suggestedMovies = (await _tmdbService.GetMoviesByGenreAsync(randomGenre)).Take(8).ToList();
                }
                else
                {
                    suggestedMovies = (await _tmdbService.GetTrendingMoviesAsync(1)).Take(8).ToList();
                }

                ViewBag.SuggestedMovies = suggestedMovies;
                ViewBag.CurrentUser = currentUser;
                ViewBag.FeedReviews = feedReviews;
                ViewBag.Authors = authors;
                ViewBag.CurrentFilter = filter;

                return View("Feed");
            }

            var trending = await _tmdbService.GetTrendingMoviesAsync(1);
            ViewBag.HeroMovies = trending.Take(9).ToList();

            return View();
        }

        // PÁGINA DE EXPLORAR
        // Permite ao usuário buscar por filmes específicos ou outros usuários.
        // Caso não haja uma busca ativa, exibe listas de filmes em alta e sugestões dinâmicas.
        public async Task<IActionResult> Explore(string q, string tab = "tudo")
        {
            string safeQuery = q ?? string.Empty;
            bool isSearchLocal = !string.IsNullOrWhiteSpace(safeQuery);

            ViewBag.Query = safeQuery;
            ViewBag.ActiveTab = tab?.ToLower() ?? "tudo";
            ViewBag.IsSearch = isSearchLocal;

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = string.IsNullOrEmpty(userEmail) ? null : _usersCollection.Find(u => u.Email == userEmail).FirstOrDefault();
            ViewBag.CurrentUser = currentUser;

            var sortDefinition = Builders<Review>.Sort.Descending(r => r.LikesCount).Descending(r => r.CreatedAt);
            var communityReviews = _reviewsCollection.Find(_ => true).Sort(sortDefinition).Limit(3).ToList();
            var authorIds = communityReviews.Select(r => r.UserId).Where(id => MongoDB.Bson.ObjectId.TryParse(id, out _)).Distinct().ToList();
            var projection = Builders<User>.Projection.Include(u => u.Id).Include(u => u.Name).Include(u => u.Username).Include(u => u.ProfilePicture);
            var authors = _usersCollection.Find(u => authorIds.Contains(u.Id)).Project<User>(projection).ToList();

            ViewBag.CommunityReviews = communityReviews;
            ViewBag.Authors = authors;

            var movies = new List<TmdbMovie>();

            try
            {
                if (isSearchLocal)
                {
                    if (ViewBag.ActiveTab == "tudo" || ViewBag.ActiveTab == "filmes")
                        movies = (await _tmdbService.SearchMoviesAsync(safeQuery)).Take(10).ToList();

                    if (ViewBag.ActiveTab == "tudo" || ViewBag.ActiveTab == "pessoas")
                    {
                        var filter = Builders<User>.Filter.Regex(u => u.Username, new MongoDB.Bson.BsonRegularExpression(safeQuery, "i"));
                        ViewBag.Users = _usersCollection.Find(filter).Project<User>(projection).Limit(10).ToList();
                    }
                    return View(movies);
                }
                else
                {
                    var trendingPage1 = await _tmdbService.GetTrendingMoviesAsync(1);
                    var trendingPage2 = await _tmdbService.GetTrendingMoviesAsync(2);
                    var suggestedMovies = new List<TmdbMovie>();

                    if (currentUser?.FavoriteGenres != null && currentUser.FavoriteGenres.Any())
                    {
                        var random = new Random();
                        var randomGenre = currentUser.FavoriteGenres.OrderBy(x => random.Next()).First();
                        suggestedMovies = await _tmdbService.GetMoviesByGenreAsync(randomGenre);
                    }
                    else
                    {
                        suggestedMovies = await _tmdbService.GetTrendingMoviesAsync(3);
                    }

                    ViewBag.TrendingBrazil = trendingPage1.Take(10).ToList();
                    ViewBag.TrendingWorld = trendingPage2.Take(10).ToList();
                    ViewBag.SuggestedMovies = suggestedMovies.OrderBy(x => Guid.NewGuid()).Take(8).ToList();
                    ViewBag.SuggestedUsers = _usersCollection.Find(u => currentUser == null || u.Id != currentUser.Id)
                        .Project<User>(projection).Limit(15).ToList();

                    return View(new List<TmdbMovie>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados no Explorar");
                return View(new List<TmdbMovie>());
            }
        }

        // PÁGINA SOBRE O APP
        // Página estática e institucional contando a história ou detalhes do CinePapo (Acessível sem login).
        [HttpGet]
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }


        // =========================================================================================
        // 2. INTERAÇÃO COM FILMES
        // =========================================================================================

        // DETALHES DO FILME
        // Puxa as informações completas de um filme na API do TMDB (sinopse, capas)
        // e exibe as resenhas que a comunidade já escreveu sobre essa obra específica.
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _tmdbService.GetMovieDetailsAsync(id);
            if (movie == null) return RedirectToAction("Explore");

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            string googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = null;
            bool isWatched = false, isWantToSee = false, isFavorite = false;

            if (!string.IsNullOrEmpty(userEmail))
            {
                currentUser = _usersCollection.Find(u => u.Email == userEmail).FirstOrDefault();
                if (currentUser != null)
                {
                    isWatched = currentUser.Watched != null && currentUser.Watched.Any(m => m.MovieId == id);
                    isWantToSee = currentUser.WantToSee != null && currentUser.WantToSee.Any(m => m.MovieId == id);
                    isFavorite = currentUser.Favorites != null && currentUser.Favorites.Any(m => m.MovieId == id);
                }
            }

            var movieReviews = _reviewsCollection.Find(r => r.MovieId == id)
                                                 .SortByDescending(r => r.CreatedAt)
                                                 .Limit(3)
                                                 .ToList();

            var authorIds = movieReviews.Select(r => r.UserId)
                                        .Where(uid => MongoDB.Bson.ObjectId.TryParse(uid, out _))
                                        .Distinct().ToList();

            var projection = Builders<User>.Projection.Include(u => u.Id).Include(u => u.Name).Include(u => u.Username).Include(u => u.ProfilePicture);
            var authors = _usersCollection.Find(u => authorIds.Contains(u.Id)).Project<User>(projection).ToList();

            ViewBag.IsWatched = isWatched;
            ViewBag.IsWantToSee = isWantToSee;
            ViewBag.IsFavorite = isFavorite;

            ViewBag.MovieReviews = movieReviews;
            ViewBag.ReviewAuthors = authors;
            ViewBag.TotalReviews = _reviewsCollection.CountDocuments(r => r.MovieId == id);

            ViewBag.CurrentUser = currentUser;
            ViewBag.GoogleId = googleId;

            return View(movie);
        }

        // SALVAR FILME EM UMA LISTA (AJAX)
        // Ação invisível acionada por botões que adiciona/remove filmes nas listas: Visto, Quero Ver e Favoritos do usuário.
        [HttpPost]
        public async Task<IActionResult> ToggleMovieAction(int movieId, string title, string poster, string actionType)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Json(new { success = false, message = "Faça login para salvar!" });

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return Json(new { success = false, message = "Usuário não encontrado." });

            var movieItem = new SavedMovie { MovieId = movieId, Title = title, PosterPath = poster, AddedAt = DateTime.UtcNow };
            bool isAdded = false;
            string message = "";

            UpdateDefinition<User> update = null;

            switch (actionType.ToLower())
            {
                case "visto":
                    if (currentUser.Watched != null && currentUser.Watched.Any(m => m.MovieId == movieId))
                    {
                        update = Builders<User>.Update.PullFilter(u => u.Watched, m => m.MovieId == movieId);
                        message = "Removido de Vistos";
                    }
                    else
                    {
                        update = Builders<User>.Update.AddToSet(u => u.Watched, movieItem);
                        isAdded = true;
                        message = "Adicionado aos Vistos";
                    }
                    break;
                case "querover":
                    if (currentUser.WantToSee != null && currentUser.WantToSee.Any(m => m.MovieId == movieId))
                    {
                        update = Builders<User>.Update.PullFilter(u => u.WantToSee, m => m.MovieId == movieId);
                        message = "Removido de Quero Ver";
                    }
                    else
                    {
                        update = Builders<User>.Update.AddToSet(u => u.WantToSee, movieItem);
                        isAdded = true;
                        message = "Adicionado ao Quero Ver";
                    }
                    break;
                case "favorito":
                    if (currentUser.Favorites != null && currentUser.Favorites.Any(m => m.MovieId == movieId))
                    {
                        update = Builders<User>.Update.PullFilter(u => u.Favorites, m => m.MovieId == movieId);
                        message = "Removido dos Favoritos";
                    }
                    else
                    {
                        update = Builders<User>.Update.AddToSet(u => u.Favorites, movieItem);
                        isAdded = true;
                        message = "Adicionado aos Favoritos";
                    }
                    break;
            }

            if (update != null) await _usersCollection.UpdateOneAsync(u => u.Id == currentUser.Id, update);

            return Json(new { success = true, isAdded, message });
        }


        // =========================================================================================
        // 3. CARREGAMENTO CONTÍNUO DE RESENHAS (AJAX / PAGINAÇÃO)
        // =========================================================================================

        // CARREGAR MAIS RESENHAS NO FEED
        // Busca o próximo lote de resenhas para injetar na página principal conforme o usuário rola a tela.
        [HttpGet]
        public IActionResult LoadFeedReviewsAjax(int skip, string filter = "global")
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            string googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = string.IsNullOrEmpty(userEmail) ? null : _usersCollection.Find(u => u.Email == userEmail).FirstOrDefault();

            List<Review> feedReviews;

            if (filter == "seguindo" && currentUser?.FollowingIds != null && currentUser.FollowingIds.Any())
            {
                var targetIds = new List<string>(currentUser.FollowingIds) { currentUser.Id! };
                feedReviews = _reviewsCollection.Find(r => targetIds.Contains(r.UserId))
                                                  .SortByDescending(r => r.CreatedAt)
                                                  .Skip(skip)
                                                  .Limit(5).ToList();
            }
            else
            {
                feedReviews = _reviewsCollection.Find(_ => true)
                                                  .SortByDescending(r => r.CreatedAt)
                                                  .Skip(skip)
                                                  .Limit(5).ToList();
            }

            var authorIds = feedReviews.Select(r => r.UserId).Where(id => MongoDB.Bson.ObjectId.TryParse(id, out _)).Distinct().ToList();

            var projection = Builders<User>.Projection.Include(u => u.Id).Include(u => u.Name).Include(u => u.Username).Include(u => u.ProfilePicture);
            var authors = _usersCollection.Find(u => authorIds.Contains(u.Id)).Project<User>(projection).ToList();

            var result = feedReviews.Select(r => new {
                id = r.Id,
                userId = r.UserId,
                content = r.Content,
                rating = r.Rating,
                likesCount = r.LikesCount,
                commentsCount = r.CommentsCount,
                movieTitle = r.MovieTitle,
                posterPath = r.PosterPath,
                createdAt = r.CreatedAt.ToString("dd MMM yyyy"),
                authorName = authors.FirstOrDefault(a => a.Id == r.UserId)?.Name ?? "Usuário",
                authorUsername = authors.FirstOrDefault(a => a.Id == r.UserId)?.Username?.ToLower() ?? "usuario",
                authorPhoto = authors.FirstOrDefault(a => a.Id == r.UserId)?.ProfilePicture,
                isLiked = currentUser != null && r.LikedByUsers != null && (r.LikedByUsers.Contains(currentUser.Id) || (googleId != null && r.LikedByUsers.Contains(googleId))),
                isFollowing = currentUser != null && currentUser.FollowingIds != null && currentUser.FollowingIds.Contains(r.UserId)
            });

            return Json(result);
        }

        // CARREGAR MAIS RESENHAS NA PÁGINA DO FILME
        // Busca o próximo lote de resenhas escritas exclusivamente sobre o filme sendo visualizado.
        [HttpGet]
        public IActionResult LoadMovieReviews(int movieId, int skip)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            string googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = string.IsNullOrEmpty(userEmail) ? null : _usersCollection.Find(u => u.Email == userEmail).FirstOrDefault();

            var reviews = _reviewsCollection.Find(r => r.MovieId == movieId)
                                            .SortByDescending(r => r.CreatedAt)
                                            .Skip(skip)
                                            .Limit(3)
                                            .ToList();

            var authorIds = reviews.Select(r => r.UserId)
                                   .Where(id => MongoDB.Bson.ObjectId.TryParse(id, out _))
                                   .Distinct().ToList();

            var projection = Builders<User>.Projection.Include(u => u.Id).Include(u => u.Name).Include(u => u.Username).Include(u => u.ProfilePicture);
            var authors = _usersCollection.Find(u => authorIds.Contains(u.Id)).Project<User>(projection).ToList();

            var result = reviews.Select(r => new {
                id = r.Id,
                userId = r.UserId,
                content = r.Content,
                rating = r.Rating,
                likesCount = r.LikesCount,
                commentsCount = r.CommentsCount,
                createdAt = r.CreatedAt.ToString("dd MMM yyyy"),
                authorName = authors.FirstOrDefault(a => a.Id == r.UserId)?.Name ?? "Usuário",
                authorUsername = authors.FirstOrDefault(a => a.Id == r.UserId)?.Username?.ToLower() ?? "usuario",
                authorPhoto = authors.FirstOrDefault(a => a.Id == r.UserId)?.ProfilePicture,

                isLiked = currentUser != null && r.LikedByUsers != null && (r.LikedByUsers.Contains(currentUser.Id) || (googleId != null && r.LikedByUsers.Contains(googleId))),
                isFollowing = currentUser != null && currentUser.FollowingIds != null && currentUser.FollowingIds.Contains(r.UserId)
            });

            return Json(result);
        }


        // =========================================================================================
        // 4. NOTIFICAÇÕES (MENSAGENS DO SISTEMA)
        // =========================================================================================

        // PÁGINA DE NOTIFICAÇÕES
        // Carrega o histórico completo de alertas do usuário logado (curtidas, comentários, seguidores).
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();

            if (currentUser == null) return RedirectToAction("Login", "Account");

            var notifications = await _notificationsCollection.Find(n => n.UserId == currentUser.Id)
                                                              .SortByDescending(n => n.CreatedAt)
                                                              .ToListAsync();

            var triggerUserIds = notifications.Select(n => n.TriggeredByUserId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
            var triggerUsers = await _usersCollection.Find(u => triggerUserIds.Contains(u.Id))
                                                     .Project(u => new { u.Id, u.ProfilePicture })
                                                     .ToListAsync();

            ViewBag.TriggerUsers = triggerUsers.ToDictionary(u => u.Id, u => u.ProfilePicture);

            return View(notifications);
        }

        // MARCAR UMA NOTIFICAÇÃO COMO LIDA (AJAX)
        // Atualiza a flag IsRead para true em uma notificação específica.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkNotificationAsRead(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return Json(new { success = false });

            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            await _notificationsCollection.UpdateOneAsync(n => n.Id == id && n.UserId == currentUser.Id, update);

            return Json(new { success = true });
        }

        // APAGAR UMA NOTIFICAÇÃO ESPECÍFICA (AJAX)
        // Deleta a notificação permanentemente do banco de dados a pedido do usuário.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return Json(new { success = false });

            // Busca e apaga apenas se a notificação pertencer ao usuário logado
            await _notificationsCollection.DeleteOneAsync(n => n.Id == id && n.UserId == currentUser.Id);

            return Json(new { success = true });
        }

        // MARCAR TODAS AS NOTIFICAÇÕES COMO LIDAS (AJAX)
        // Limpa a caixa de entrada do usuário marcando todas de uma só vez.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return Json(new { success = false });

            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            await _notificationsCollection.UpdateManyAsync(n => n.UserId == currentUser.Id, update);

            return Json(new { success = true });
        }


        // =========================================================================================
        // 5. TRATAMENTO DE ERROS
        // =========================================================================================

        // PÁGINA DE ERRO PADRÃO
        // Exibida caso aconteça alguma falha técnica imprevista no servidor.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}