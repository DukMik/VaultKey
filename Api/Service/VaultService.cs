using Api.Repositories;
using EntityFrameworkComm.EfModel.Models;

namespace Api.Service;

public class VaultService
{
    private readonly VaultRepository _vaultRepo;
    public VaultService(VaultRepository vaultRepo) => _vaultRepo = vaultRepo;

    public Task<Vault> CreateVaultAsync(Vault vault) =>
        _vaultRepo.CreateAsync(vault);

    public Task<Vault?> GetVaultAsync(int id) =>
        _vaultRepo.GetByIdAsync(id);
}