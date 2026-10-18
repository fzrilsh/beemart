using BeeMart.Api.Data;
using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Repositories;

public class UserRepository
{
    private readonly BeeMartDbContext _db;

    public UserRepository(BeeMartDbContext db)
    {
        _db = db;
    }

    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _db.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        return _db.Users.AnyAsync(x => x.Username == username, cancellationToken);
    }

    public Task<bool> IdUserExistsAsync(string idUser, CancellationToken cancellationToken)
    {
        return _db.Users.AnyAsync(x => x.IdUser == idUser, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
