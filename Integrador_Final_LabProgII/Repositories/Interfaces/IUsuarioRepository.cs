using ITManager.Models;
using ITManager.Models.Enums;

namespace ITManager.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol);
        Task<bool> UpdatePasswordAsync(int usuarioId, string newPasswordHash);
    }
}
