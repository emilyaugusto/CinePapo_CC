using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CinePapo.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CinePapo.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Review> _reviewsCollection;
        private readonly IMongoCollection<Notification> _notificationsCollection;

        // ==========================================
        // CONSTRUTOR
        // Inicializa o controlador e conecta com as coleções de Usuários, Resenhas e Notificações do banco de dados.
        // ==========================================
        public AccountController(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("Users");
            _reviewsCollection = database.GetCollection<Review>("Reviews");
            _notificationsCollection = database.GetCollection<Notification>("Notifications");
        }

        // =========================================================================================
        // 1. ÁREA DE AUTENTICAÇÃO (REGISTRO, LOGIN E LOGOUT)
        // =========================================================================================

        // Exibe a tela de criação de conta para novos usuários tradicionais.
        [HttpGet]
        public IActionResult Register() => View();

        // Processa os dados do formulário de cadastro, criptografa a senha e salva o novo usuário.
        [HttpPost]
        public IActionResult Register(User newUser, string ConfirmPassword)
        {
            if (newUser.PasswordHash != ConfirmPassword)
            {
                ViewBag.ErrorMessage = "As senhas não coincidem.";
                return View();
            }

            var exists = _usersCollection.Find(u => u.Email == newUser.Email).Any();
            if (exists)
            {
                ViewBag.ErrorMessage = "Este e-mail já está cadastrado.";
                return View();
            }

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            newUser.CreatedAt = DateTime.UtcNow;
            _usersCollection.InsertOne(newUser);

            return RedirectToAction("Onboarding", new { userId = newUser.Id });
        }

        // Exibe a tela de acesso para usuários que possuem e-mail e senha cadastrados.
        [HttpGet]
        public IActionResult Login() => View();

        // Autentica o usuário validando o e-mail e a senha, e cria a sessão (Cookie) de acesso.
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _usersCollection.Find(u => u.Email == email).FirstOrDefault();

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username ?? user.Name ?? "Cinéfilo"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id!)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                TempData["ToastMessage"] = $"Bem-vindo de volta, @{user.Username ?? "Cinéfilo"}!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "E-mail ou senha incorretos.";
            return View();
        }

        // Redireciona o usuário para a página segura de login do Google.
        [HttpGet]
        public IActionResult LoginGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Recebe a resposta do Google e cria uma conta nova ou faz o login se o usuário já existir.
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("Login");

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            var user = _usersCollection.Find(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {
                user = new User
                {
                    Email = email!,
                    Name = name,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = Guid.NewGuid().ToString() // Senha aleatória, já que usa Google
                };
                _usersCollection.InsertOne(user);
                return RedirectToAction("Onboarding", new { userId = user.Id, googleName = name });
            }

            if (string.IsNullOrEmpty(user.Username))
                return RedirectToAction("Onboarding", new { userId = user.Id, googleName = user.Name });

            TempData["ToastMessage"] = $"Bom te ver de novo, @{user.Username}!";
            return RedirectToAction("Index", "Home");
        }

        // Encerra a sessão atual do usuário, apagando o cookie, e redireciona para a página inicial.
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        // =========================================================================================
        // 2. ONBOARDING (PRIMEIROS PASSOS)
        // =========================================================================================

        // Exibe a tela de configuração inicial para preencher as preferências logo após o cadastro.
        [HttpGet]
        public IActionResult Onboarding(string userId, string? googleName)
        {
            ViewBag.UserId = userId;
            ViewBag.GoogleName = googleName;
            return View();
        }

        // Salva as escolhas do Onboarding (nome, metas, gêneros, foto) e realiza o login definitivo.
        [HttpPost]
        public async Task<IActionResult> CompleteOnboarding(
             string UserId,
             string FullName,
             string Username,
             List<string> PlatformGoals,
             List<string> FavoriteGenres,
             List<string> StreamingPlatforms,
             string? ProfilePictureBase64)
        {
            string cleanUsername = Username.Trim().ToLower().Replace(" ", "");

            var usernameExists = _usersCollection.Find(u => u.Username == cleanUsername && u.Id != UserId).Any();
            if (usernameExists)
            {
                return RedirectToAction("Onboarding", new { userId = UserId, googleName = FullName });
            }

            var filter = Builders<User>.Filter.Eq(u => u.Id, UserId);

            var update = Builders<User>.Update
                .Set(u => u.Name, FullName)
                .Set(u => u.Username, cleanUsername)
                .Set(u => u.PlatformGoals, PlatformGoals ?? new List<string>())
                .Set(u => u.FavoriteGenres, FavoriteGenres ?? new List<string>())
                .Set(u => u.StreamingPlatforms, StreamingPlatforms ?? new List<string>());

            if (!string.IsNullOrEmpty(ProfilePictureBase64))
            {
                update = update.Set(u => u.ProfilePicture, ProfilePictureBase64);
            }

            await _usersCollection.UpdateOneAsync(filter, update);

            var user = await _usersCollection.Find(u => u.Id == UserId).FirstOrDefaultAsync();

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username ?? user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id!)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            }

            TempData["ToastMessage"] = $"Perfil finalizado! Bem-vindo, @{cleanUsername}!";
            return RedirectToAction("Index", "Home");
        }


        // =========================================================================================
        // 3. GESTÃO DE PERFIL (VER, EDITAR E APAGAR)
        // =========================================================================================

        // Busca os dados e as resenhas do usuário para montar a página pública ou privada do perfil.
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Profile(string? id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = _usersCollection.Find(u => u.Email == userEmail).FirstOrDefault();
            if (currentUser == null) return RedirectToAction("Login", "Account");

            User profileUser = currentUser;
            bool isMyProfile = true;
            bool isFollowing = false;

            if (!string.IsNullOrEmpty(id) && id != currentUser.Id)
            {
                profileUser = _usersCollection.Find(u => u.Id == id).FirstOrDefault();
                if (profileUser == null) return RedirectToAction("Explore", "Home");

                isMyProfile = false;
                isFollowing = currentUser.FollowingIds != null && currentUser.FollowingIds.Contains(profileUser.Id);
            }

            var userReviews = _reviewsCollection.Find(r => r.UserId == profileUser.Id)
                                                .SortByDescending(r => r.CreatedAt)
                                                .ToList();
            profileUser.ReviewCount = userReviews.Count;

            ViewBag.Reviews = userReviews;
            ViewBag.IsMyProfile = isMyProfile;
            ViewBag.IsFollowing = isFollowing;
            ViewBag.CurrentUser = currentUser;

            return View(profileUser);
        }

        // Exibe o formulário com os dados atuais do usuário para permitir edição.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login");

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return RedirectToAction("Login");

            return View(currentUser);
        }

        // Recebe e salva as alterações feitas pelo usuário (como nova foto, capa, nome ou gêneros).
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(string Name, string Username, List<string> FavoriteGenres, string? ProfilePictureBase64, string? CoverPhotoBase64)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();
            if (currentUser == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(Username))
            {
                TempData["ErrorMessage"] = "O nome de usuário não pode ficar vazio.";
                return RedirectToAction("EditProfile");
            }

            string cleanUsername = Username.Replace(" ", "").ToLower();

            // Pega as imagens atuais. Se a flag for "REMOVE", apaga a imagem gravando null.
            string profilePictureData = currentUser.ProfilePicture;
            string coverPhotoData = currentUser.CoverPhoto;

            if (ProfilePictureBase64 == "REMOVE") profilePictureData = null;
            else if (!string.IsNullOrEmpty(ProfilePictureBase64)) profilePictureData = ProfilePictureBase64;

            if (CoverPhotoBase64 == "REMOVE") coverPhotoData = null;
            else if (!string.IsNullOrEmpty(CoverPhotoBase64)) coverPhotoData = CoverPhotoBase64;

            var update = Builders<User>.Update
                .Set(u => u.Name, Name)
                .Set(u => u.Username, cleanUsername)
                .Set(u => u.ProfilePicture, profilePictureData)
                .Set(u => u.CoverPhoto, coverPhotoData)
                .Set(u => u.FavoriteGenres, FavoriteGenres ?? new List<string>());

            await _usersCollection.UpdateOneAsync(u => u.Id == currentUser.Id, update);

            TempData["ToastMessage"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Profile");
        }

        // Apaga permanentemente a conta do usuário logado, junto com suas resenhas e notificações.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login");

            var currentUser = await _usersCollection.Find(u => u.Email == userEmail).FirstOrDefaultAsync();

            if (currentUser != null)
            {
                await _reviewsCollection.DeleteManyAsync(r => r.UserId == currentUser.Id);
                await _notificationsCollection.DeleteManyAsync(n => n.UserId == currentUser.Id);
                await _usersCollection.DeleteOneAsync(u => u.Id == currentUser.Id);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            TempData["ToastMessage"] = "Sua conta foi excluída com sucesso. Sentiremos sua falta! 🍿";
            return RedirectToAction("Index", "Home");
        }


        // =========================================================================================
        // 4. MÉTODOS AUXILIARES 
        // =========================================================================================

        // Processa a ação de seguir ou deixar de seguir outro usuário, criando uma notificação se for novo seguidor.
        [HttpPost]
        public IActionResult ToggleFollow(string targetUserId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Json(new { success = false });

            var currentUser = _usersCollection.Find(u => u.Email == userEmail).FirstOrDefault();
            if (currentUser == null || currentUser.Id == targetUserId) return Json(new { success = false });

            bool isFollowing = currentUser.FollowingIds != null && currentUser.FollowingIds.Contains(targetUserId);

            if (isFollowing)
            {
                _usersCollection.UpdateOne(u => u.Id == currentUser.Id, Builders<User>.Update.Pull(u => u.FollowingIds, targetUserId).Inc(u => u.FollowingCount, -1));
                _usersCollection.UpdateOne(u => u.Id == targetUserId, Builders<User>.Update.Inc(u => u.FollowersCount, -1));
            }
            else
            {
                _usersCollection.UpdateOne(u => u.Id == currentUser.Id, Builders<User>.Update.AddToSet(u => u.FollowingIds, targetUserId).Inc(u => u.FollowingCount, 1));
                _usersCollection.UpdateOne(u => u.Id == targetUserId, Builders<User>.Update.Inc(u => u.FollowersCount, 1));

                var targetUser = _usersCollection.Find(u => u.Id == targetUserId).FirstOrDefault();
                if (targetUser != null)
                {
                    var notification = new Notification
                    {
                        UserId = targetUser.Id!,
                        TriggeredByUserId = currentUser.Id!,
                        TriggeredByUsername = currentUser.Username ?? currentUser.Name,
                        Type = "Follow",
                        Message = $"{currentUser.Username ?? currentUser.Name} começou a te seguir.",
                        Link = $"/Account/Profile?id={currentUser.Id}",
                        CreatedAt = DateTime.UtcNow
                    };
                    _notificationsCollection.InsertOne(notification);
                }
            }

            return Json(new { success = true, isFollowing = !isFollowing });
        }

        // Valida em tempo real via JavaScript se o nome de usuário digitado já está em uso no banco.
        [HttpGet]
        public IActionResult CheckUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return Json(false);

            var cleanUsername = username.ToLower().Trim();
            var exists = _usersCollection.Find(u => u.Username == cleanUsername).Any();

            return Json(exists);
        }
    }
}