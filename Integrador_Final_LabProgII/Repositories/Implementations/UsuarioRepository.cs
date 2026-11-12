using ITManager.Data;
using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Repositories.Implementations
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            return await _dbSet
                .Include(u => u.Ubicacion)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());
        }

        public async Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol)
        {
            return await _dbSet
                .Include(u => u.Ubicacion)
                .Where(u => u.Rol == rol && u.Activo)
                .OrderBy(u => u.Apellido)
                .ThenBy(u => u.Nombre)
                .ToListAsync();
        }

        public async Task<bool> UpdatePasswordAsync(int usuarioId, string newPasswordHash)
        {
            var usuario = await _dbSet.FindAsync(usuarioId);
            if (usuario == null) return false;

            usuario.PasswordHash = newPasswordHash;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

