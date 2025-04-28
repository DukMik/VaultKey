using EntityFrameworkComm.EfModel.Context;
using EntityFrameworkComm.EfModel.Models;

namespace Api.Repositories;

public class VaultRepository
{
    private readonly Context _context;
    public VaultRepository(Context context) => _context = context;

    public async Task<Vault> CreateAsync(Vault vault)
    {
        _context.Vault.Add(vault);
        await _context.SaveChangesAsync();
        return vault;
    }

    public Task<Vault?> GetByIdAsync(int id) =>
        _context.Vault.FindAsync(id).AsTask();
}