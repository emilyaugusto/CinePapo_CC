using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CinePapo.Models;
using CinePapo.Services;
using System.Security.Claims;

namespace CinePapo.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IMongoCollection<Review> _reviewsCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Comment> _commentsCollection;
        private readonly IMongoCollection<Notification> _notificationsCollection;
        private readonly TmdbService _tmdbService;

        // =========================================================================================
        // CONSTRUTOR DO CONTROLADOR
        // Prepara o terreno conectando as tabelas do banco de dados e a API de filmes (TMDB).
        // =========================================================================================
        public ReviewController(IMongoDatabase database, TmdbService tmdbService)
        {
            _reviewsCollection = database.GetCollection<Review>("Reviews");
            _usersCollection = database.GetCollection<User>("Users");
            _commentsCollection = database.GetCollection<Comment>("Comments");
            _notificationsCollection = database.GetCollection<Notification>("Notifications");
            _tmdbService = tmdbService;
        }


        // =========================================================================================
        // 1. EXIBIR RESENHA (PÁGINA DE DETALHES)
        // Monta a tela completa de uma resenha específica, buscando o autor, o texto e os comentários.
        // =========================================================================================
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Home");

            // 🔥 1. Busca a review
            var review = await _reviewsCollection
                .Find(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (review == null)
                return RedirectToAction("Index", "Home");

            // 🔥 2. Busca comentários (UMA VEZ SÓ)
            var comments = await _commentsCollection
                .Find(c => c.ReviewId == id)
                .SortByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.Comments = comments;

            // 🔥 3. Autor da review
            var author = await _usersCollection
                .Find(u => u.Id == review.UserId)
                .FirstOrDefaultAsync();

            ViewBag.Author = author;

            // 🔥 4. Usuário logado
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var currentUser = string.IsNullOrEmpty(userEmail)
                ? null
                : await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();

            ViewBag.CurrentUser = currentUser;
            ViewBag.GoogleId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 🔥 5. Buscar usuários que comentaram (AGORA CERTO)
            var commenterIds = comments
                .Select(c => c.UserId)
                .Distinct()
                .ToList();

            var commenters = await _usersCollection
                .Find(u => commenterIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.Commenters = commenters;

            return View(review);
        }


        // =========================================================================================
        // 2. CRIAR NOVA RESENHA
        // Mostra a tela em branco para escrever e salva o que foi digitado no banco de dados.
        // =========================================================================================
        [HttpGet]
        public IActionResult Create(int? movieId, string? movieTitle, string? posterPath)
        {
            // Pega os dados que vieram do botão "Avaliar" e manda para a tela
            ViewBag.PreMovieId = movieId;
            ViewBag.PreMovieTitle = movieTitle;
            ViewBag.PrePosterPath = posterPath;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review newReview)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return RedirectToAction("Login", "Account");

            if (newReview.MovieId == 0 || string.IsNullOrEmpty(newReview.MovieTitle))
            {
                TempData["ToastMessage"] = "Por favor, selecione um filme da lista!";
                return View(newReview);
            }

            newReview.UserId = currentUser.Id!;
            newReview.CreatedAt = DateTime.UtcNow;
            newReview.LikedByUsers = new List<string>();
            newReview.Comments = new List<Comment>();
            newReview.LikesCount = 0;
            newReview.CommentsCount = 0;

            await _reviewsCollection.InsertOneAsync(newReview);

            await _usersCollection.UpdateOneAsync(u => u.Id == currentUser.Id, Builders<User>.Update.Inc(u => u.ReviewCount, 1));

            TempData["ToastMessage"] = "Resenha publicada com sucesso!";
            return RedirectToAction("Profile", "Account");
        }


        // =========================================================================================
        // 3. EDITAR RESENHA
        // Permite ao dono da resenha abrir a tela de edição e salvar o texto/nota atualizados.
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var review = await _reviewsCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (review == null) return NotFound();

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();

            if (currentUser == null || review.UserId != currentUser.Id)
            {
                return Unauthorized(); // Ou RedirectToAction("Details", new { id = id });
            }

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Review model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            var review = await _reviewsCollection.Find(r => r.Id == model.Id).FirstOrDefaultAsync();

            if (review != null && currentUser != null && review.UserId == currentUser.Id)
            {
                var update = Builders<Review>.Update
                    .Set(r => r.Content, model.Content)
                    .Set(r => r.Rating, model.Rating);

                await _reviewsCollection.UpdateOneAsync(r => r.Id == model.Id, update);
            }
            return RedirectToAction("Profile", "Account");
        }


        // =========================================================================================
        // 4. INTERAÇÕES (CURTIR E COMENTAR)
        // Controla as ações sociais: dar "coraçãozinho" numa resenha ou enviar um comentário.
        // =========================================================================================
        [HttpPost]
        public async Task<IActionResult> ToggleLike(string reviewId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Json(new { success = false, message = "Faça login para curtir." });

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return Json(new { success = false, message = "Usuário não encontrado." });

            var review = await _reviewsCollection.Find(r => r.Id == reviewId).FirstOrDefaultAsync();
            if (review == null) return Json(new { success = false, message = "Resenha não encontrada." });

            string mongoId = currentUser.Id ?? "";
            string googleId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            // Traz a lista para a memória e garante que ela não é nula
            var likedUsers = review.LikedByUsers ?? new List<string>();
            bool isLiked = likedUsers.Contains(mongoId) || (!string.IsNullOrEmpty(googleId) && likedUsers.Contains(googleId));

            if (isLiked)
            {
                likedUsers.Remove(mongoId);
                if (!string.IsNullOrEmpty(googleId)) likedUsers.Remove(googleId);
            }
            else
            {
                likedUsers.Add(mongoId);
            }

            // Substitui a lista inteira de forma segura no banco
            var update = Builders<Review>.Update
                .Set(r => r.LikedByUsers, likedUsers)
                .Set(r => r.LikesCount, likedUsers.Count);

            await _reviewsCollection.UpdateOneAsync(r => r.Id == reviewId, update);

            // Gerar notificação se o usuário curtiu a resenha de outra pessoa
            if (!isLiked && currentUser.Id != review.UserId)
            {
                var notification = new Notification
                {
                    UserId = review.UserId!,
                    TriggeredByUserId = currentUser.Id!,
                    TriggeredByUsername = currentUser.Username ?? currentUser.Name,
                    Type = "LikeReview",
                    Message = $"{currentUser.Username ?? currentUser.Name} curtiu sua resenha de {review.MovieTitle}.",
                    Link = $"/Review/Details/{review.Id}",
                    CreatedAt = DateTime.UtcNow,
                    RelatedEntityId = review.Id
                };
                await _notificationsCollection.InsertOneAsync(notification);
            }

            return Json(new { success = true, isLiked = !isLiked, likesCount = likedUsers.Count });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string reviewId, string content)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();

            if (currentUser == null || string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", new { id = reviewId });

            var comment = new Comment
            {
                ReviewId = reviewId, // RELACIONAL: Diz de quem é o comentário!
                UserId = currentUser.Id!,
                UserName = currentUser.Name ?? currentUser.Username ?? "Usuário",
                UserPhoto = "",
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            // SALVA DIRETO NA PASTA COMMENTS DO MONGODB
            await _commentsCollection.InsertOneAsync(comment);

            // APENAS AUMENTA O NÚMERO DE COMENTÁRIOS NA RESENHA (+1)
            var update = Builders<Review>.Update.Inc(r => r.CommentsCount, 1);
            await _reviewsCollection.UpdateOneAsync(r => r.Id == reviewId, update);

            // Gerar notificação se o usuário comentou na resenha de outra pessoa
            var review = await _reviewsCollection.Find(r => r.Id == reviewId).FirstOrDefaultAsync();
            if (review != null && currentUser.Id != review.UserId)
            {
                var notification = new Notification
                {
                    UserId = review.UserId!,
                    TriggeredByUserId = currentUser.Id!,
                    TriggeredByUsername = currentUser.Username ?? currentUser.Name,
                    Type = "CommentReview",
                    Message = $"{currentUser.Username ?? currentUser.Name} comentou na sua resenha de {review.MovieTitle}.",
                    Link = $"/Review/Details/{review.Id}#comments",
                    CreatedAt = DateTime.UtcNow,
                    RelatedEntityId = comment.Id
                };
                await _notificationsCollection.InsertOneAsync(notification);
            }

            TempData["ToastMessage"] = "Comentário adicionado!";
            return RedirectToAction("Details", new { id = reviewId });
        }


        // =========================================================================================
        // 5. APAGAR DADOS (EXCLUSÃO)
        // Deleta resenhas ou comentários do banco de dados sem recarregar a tela (AJAX).
        // =========================================================================================
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Json(new { success = false, message = "Sessão expirada. Faça login novamente." });

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            var review = await _reviewsCollection.Find(r => r.Id == id).FirstOrDefaultAsync();

            // Bloqueio de segurança
            if (review == null || currentUser == null || review.UserId != currentUser.Id)
            {
                return Json(new { success = false, message = "Não tem permissão para apagar esta resenha." });
            }

            // Apaga a resenha do banco
            await _reviewsCollection.DeleteOneAsync(r => r.Id == id);

            // 🔥 RETORNA JSON: Agora o JavaScript vai entender perfeitamente!
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(string reviewId, string commentId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            var review = await _reviewsCollection.Find(r => r.Id == reviewId).FirstOrDefaultAsync();

            if (review == null || currentUser == null)
                return Json(new { success = false, message = "Resenha ou utilizador não encontrado." });

            var comment = await _commentsCollection.Find(c => c.Id == commentId).FirstOrDefaultAsync();

            if (comment != null && (comment.UserId == currentUser.Id || review.UserId == currentUser.Id))
            {
                // Apaga o comentário
                await _commentsCollection.DeleteOneAsync(c => c.Id == commentId);

                // Diminui o contador de comentários na resenha (-1)
                await _reviewsCollection.UpdateOneAsync(
                    r => r.Id == reviewId,
                    Builders<Review>.Update.Inc(r => r.CommentsCount, -1)
                );

                // 🔥 RETORNA JSON: Alinhado com o modal VIP
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Não tem permissão para apagar este comentário." });
        }


        // =========================================================================================
        // 6. FUNÇÕES AUXILIARES (AJAX INVISÍVEL)
        // Requisições rápidas de bastidores, como procurar a capa do filme na tela de criação.
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> SearchMovieAjax(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Json(new List<TmdbMovie>());
            var movies = await _tmdbService.SearchMoviesAsync(query);
            return Json(movies.Take(5));
        }
    }
}