using EntityFrameworkComm.EfModel.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkComm.EfModel.Context;

namespace Api.Repositories
{
    /// <summary>
    /// Repository pour la gestion des utilisateurs en base de données.
    /// </summary>
    public class UserRepositorie
    {
        private readonly IDbContextFactory<Context> _dbContextFactory;

        /// <summary>
        /// Initialise une nouvelle instance du repository.
        /// </summary>
        /// <param name="dbContextFactory">Factory permettant de créer des instances du contexte de base de données VaultContext.</param>
        public UserRepositorie(IDbContextFactory<Context> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Récupère l'identifiant interne de l'utilisateur existant ou crée un nouvel enregistrement si aucun utilisateur correspondant n'est trouvé.
        /// </summary>
        /// <param name="externalUserId">Identifiant externe de l'utilisateur (issu par exemple d'Azure AD).</param>
        /// <returns>L'identifiant interne de l'utilisateur.</returns>
        public async Task<int> GetOrCreateAppUserIdAsync(Guid externalUserId)
        {
            // Création d'une nouvelle instance du contexte de base de données
            using Context dbContext = await _dbContextFactory.CreateDbContextAsync();

            // Démarrage d'une transaction pour assurer la cohérence de l'opération
            using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Recherche de l'utilisateur dans la base à partir de son identifiant externe
                User? appUser = await dbContext.User.FirstOrDefaultAsync(u => u.EntraIdUser == externalUserId);
                bool isNewUser = false;

                // Si l'utilisateur n'existe pas, on le crée et on l'ajoute à la base
                if (appUser == null)
                {
                    // On initialise aussi la collection Logs pour respecter le membre required
                    appUser = new User
                    {
                        EntraIdUser = externalUserId,
                        Logs = new List<Log>()
                    };

                    await dbContext.User.AddAsync(appUser);
                    await dbContext.SaveChangesAsync();
                    isNewUser = true;
                }

                // Si l'utilisateur vient d'être créé, on enregistre un log indiquant la création
                if (isNewUser)
                {
                    // Crée un log pour signaler la création de l'utilisateur
                    var createLog = new Log
                    {
                        ActionDate = DateTime.UtcNow,
                        ActionType = "Création utilisateur",
                        Details = $"L'utilisateur avec EntraIdUser {externalUserId} a été créé dans la base de données.",
                        UserId = appUser.IdUser,
                        User = appUser,
                        // Les clés étrangères sont laissées à null pour ce log particulier.
                        VaultId = null,
                        EntryId = null,
                        DataId = null
                        // Pour les propriétés de navigation requises (Vault, Entrie et EncryptedData),
                        // pensez à modifier votre modèle pour qu'elles soient optionnelles ou à fournir des instances par défaut.
                    };

                    await dbContext.Log.AddAsync(createLog);
                    await dbContext.SaveChangesAsync();
                }

                // Valide la transaction
                await transaction.CommitAsync();

                // Retourne l'identifiant interne de l'utilisateur
                return appUser.IdUser;
            }
            catch (Exception ex)
            {
                // En cas d'erreur, la transaction est annulée
                await transaction.RollbackAsync();
                // Lève une exception pour une gestion ultérieure
                throw new Exception("Erreur interne lors de la création ou de la récupération de l'utilisateur.", ex);
            }
        }
    }
}
