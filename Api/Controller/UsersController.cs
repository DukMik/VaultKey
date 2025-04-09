using Microsoft.AspNetCore.Mvc;
using TheApiDto;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkComm.EfModel.Context;
using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;



namespace Api.Controller
{
    /// <summary>
    /// Contrôleur permettant la gestion des utilisateurs et l'extraction de leurs données associées.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserService _service;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="UsersController"/>.
        /// </summary>
        /// <param name="context">Contexte de base de données utilisé pour les opérations CRUD sur les utilisateurs.</param>
        public UsersController(Context context, UserService userService)
        {
            _context = context;
            _service = userService;
        }

        /// <summary>
        /// Récupère la liste de tous les utilisateurs enregistrés.
        /// </summary>
        /// <returns>Une liste d’objets <see cref="UserDTO"/> contenant les informations des utilisateurs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            // Sélectionne et mappe les utilisateurs sur le DTO sans inclure les vaults et logs pour simplifier la réponse
            var users = await _context.User
                .Select(u => new UserDto
                {
                    IdUser = u.IdUser,
                    EntraIdUser = u.EntraIdUser,
                    Vaults = null, // Les coffres (vaults) ne sont pas inclus dans cette opération
                    Logs = null // Les logs d’audit ne sont pas inclus dans cette opération
                })
                .ToListAsync();

            return Ok(users);
        }


        [HttpGet("user")]
        [Authorize] // S'assure que seul un utilisateur connecté peut accéder
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {            

           
            // 2. Recherchez l'utilisateur dans la base de données
            var user = await _context.User
                .Where(u => u.IdUser == _service.CurrentUserId)
                .Select(u => new UserDto
                {
                    IdUser = u.IdUser,
                    EntraIdUser = u.EntraIdUser,
                    Vaults = null,
                    Logs = null
                })
                .FirstOrDefaultAsync();

            // 3. Vérifie si l'utilisateur existe
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            return Ok(user);
        }


        ///// <summary>
        ///// Récupère les informations de l'utilisateur actuellement connecté.
        ///// </summary>
        //[HttpGet("/user")]
        //[Authorize]
        //public async Task<ActionResult<UserDto>> GetCurrentUser()
        //{
        //    // Récupère le claim "oid" (object ID) de l'utilisateur connecté
        //    var objectId = User.GetObjectId();

        //    if (!Guid.TryParse(objectId, out Guid externalUserId))
        //        return Unauthorized("Identifiant utilisateur invalide.");

        //    // Utilise ton service pour récupérer ou créer l'utilisateur
        //    var internalUserId = await UserService.GetOrCreateAppUserIdAsync(externalUserId);

        //    // Crée un DTO de réponse
        //    var result = new UserDto
        //    {
        //        IdUser = internalUserId,
        //        EntraIdUser = externalUserId,
        //        Vaults = null,
        //        Logs = null
        //    };

        //    return Ok(result);
        //}

        /// <summary>
        /// Récupère la liste des coffres (vaults) d’un utilisateur spécifié par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l’utilisateur dont on veut récupérer les coffres.</param>
        /// <returns>Une liste d’objets <see cref="VaultDTO"/> représentant les coffres actifs de l’utilisateur.</returns>
        [HttpGet("{id}/vaults")]
        public async Task<ActionResult<IEnumerable<VaultDto>>> GetVaultsByUser(int id)
        {
            // Vérifie que l'identifiant est valide (strictement supérieur à zéro)
            if (id <= 0)
            {
                return BadRequest("L'identifiant de l'utilisateur doit être spécifié et valide.");
            }

            // Recherche des coffres qui appartiennent à l'utilisateur et qui ne sont pas désactivés
            var vaults = await _context.Vault
                .Where(v => v.UserId == id && !v.IsDesactivated)
                .Select(v => new VaultDto
                {
                    IdVault = v.IdVault,
                    UserId = v.UserId,
                    VaultName = v.VaultName,
                    DateCreated = v.DateCreated,
                    KeyHash = v.KeyHash,
                    Salt = v.Salt,
                    PrivateKey = v.PrivateKey,
                    IsDesactivated = v.IsDesactivated
                })
                .ToListAsync();

            return Ok(vaults);
        }
    }
}
