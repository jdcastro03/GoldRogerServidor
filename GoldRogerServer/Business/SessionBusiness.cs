using GoldRoger.Data;
using GoldRoger.Entity.Entities;
using GoldRogerServer.Business.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldRogerServer.Business
{
    public class SessionBusiness : BaseBusiness
    {
        public SessionBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Dictionary<int, string>> GetUserPermissionKeys(int userId)
        {
            // Obtener los IDs de permiso asignados al usuario
            var permissionIds = await uow.UserPermissionRepository
                .Get(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            // Obtener las claves de permiso asociadas a los IDs de permiso
            var permissionKeys = await uow.PermissionRepository
                .Get(p => permissionIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Key })
                .ToDictionaryAsync(p => p.Id, p => p.Key);

            return permissionKeys;
        }
    }
}