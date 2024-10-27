using GoldRoger.Data;
using GoldRoger.Entity.Entities.Security;
using GoldRogerServer.Business.Core;
using GoldRogerServer.DTOs.Security;
using Microsoft.EntityFrameworkCore;


namespace GoldRogerServer.Business.Security
{
    public class SecurityBusiness : BaseBusiness
    {
        public SecurityBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        { }


        public async Task<IEnumerable<Permission>> GetPermissions()
        {
            return await uow.PermissionRepository.Get().ToListAsync();
        }

        public async Task<Permission> Add(AddPermissionRequest addPRequest, int userId)
        {
            var user = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException(message: "MSG_Usuario invalido");


            if (await uow.PermissionRepository.Get(p => p.Key.ToLower() == addPRequest.Key.ToLower()).AnyAsync())
                throw new ArgumentException(message: "MSG_Ya existe un permiso con la misma clave");

            Permission permission = new Permission();
            permission.Id = addPRequest.Id;
            permission.Key = addPRequest.Key;
            permission.Description = addPRequest.Description;
            permission.CreatedBy = user.FirstName;
            var now = DateTime.Now;
            permission.CreatedOn = now;
            permission.ModifiedBy = user.FirstName;
            permission.ModifiedOn = now;

            uow.PermissionRepository.Insert(permission);
            await uow.SaveAsync();
            return permission;
        }


        public async Task<List<Permission>> AddMultiple(List<AddPermissionRequest> addPRequests, int userId)
        {
            var user = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException(message: "MSG_Usuario invalido");


            List<Permission> permissions = new List<Permission>();
            foreach (var addPRequest in addPRequests)
            {
                if (await uow.PermissionRepository.Get(p => p.Key.ToLower() == addPRequest.Key.ToLower()).AnyAsync())
                    throw new ArgumentException(message: "MSG_Ya existe un permiso con la misma clave");

                Permission permission = new Permission();
                permission.Id = addPRequest.Id;
                permission.Key = addPRequest.Key;
                permission.Description = addPRequest.Description;
                permission.CreatedBy = user.FirstName;
                var now = DateTime.Now;
                permission.CreatedOn = now;
                permission.ModifiedBy = user.FirstName;
                permission.ModifiedOn = now;

                uow.PermissionRepository.Insert(permission);
                permissions.Add(permission);
            }
            await uow.SaveAsync();
            return permissions;
        }


        public async Task<Permission> Update(UpdatePermissionRequest updatePRequest, int userId)
        {
            var user = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException(message: "MSG_Usuario invalido");


            var permission = await uow.PermissionRepository.Get(p => p.Id == updatePRequest.Id).FirstOrDefaultAsync();
            if (permission == null)
                throw new ArgumentException(message: "MSG_Permiso no encontrado");


            permission.Description = updatePRequest.Description;
            permission.ModifiedBy = user.FirstName;
            var now = DateTime.Now;
            permission.ModifiedOn = now;

            uow.PermissionRepository.Update(permission);
            await uow.SaveAsync();
            return permission;
        }


        //delete permission


        //utiliza el updatepermissionrequest para deletear el permisso tomando de parametro el permissionid

        //public async Task<Permission> Delete(UpdatePermissionRequest updatePRequest, int userId)
        //{
        //    var user = await uow.UserRepository.Get(u => u.Id == userId).FirstOrDefaultAsync();
        //    if (user == null)
        //        throw new ArgumentException("MSG_Usuario invalido");
        //    else if (!user.Active)
        //        throw new ArgumentException("MSG_Usuario bloqueado");

        //    var permission = await uow.PermissionRepository.Get(p => p.Id == updatePRequest.Id).FirstOrDefaultAsync();
        //    if (permission == null)
        //        throw new ArgumentException("MSG_Permiso no encontrado");

        //    uow.PermissionRepository.Delete(permission);
        //    await uow.SaveAsync();
        //    return permission;
        //}




        //delete utlizando el deletepermissionrequest para deletear el permisso tomando de parametro el permissionid


        public async Task<Permission> Delete(DeletePermissionRequest deletePermissionRequest, int userId)
        {
            var user = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException("MSG_Usuario invalido");


            var permission = await uow.PermissionRepository.Get(p => p.Id == deletePermissionRequest.Id).FirstOrDefaultAsync();
            if (permission == null)
                throw new ArgumentException("MSG_Permiso no encontrado");

            uow.PermissionRepository.Delete(permission);
            await uow.SaveAsync();
            return permission;
        }

        //check if user has permission user doesnt contain the permission list
        public async Task<Dictionary<int, string>> GetUserPermissionKeys(CheckPermissionRequest checkPermissionRequest)
        {
            // Obtener el usuario por su ID
            var user = await uow.UserRepository.Get(u => u.UserId == checkPermissionRequest.UserId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException("MSG_Usuario invalido");

            // Obtener los IDs de permiso asignados al usuario
            var permissionIds = await uow.UserPermissionRepository
                .Get(up => up.UserId == checkPermissionRequest.UserId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            // Obtener las claves de permiso asociadas a los IDs de permiso
            var permissionKeys = await uow.PermissionRepository
                .Get(p => permissionIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Key })
                .ToDictionaryAsync(p => p.Id, p => p.Key);

            return permissionKeys;
        }



        public async Task<List<Permission>> GetUserPermissions(int userId)
        {
            // Obtener el usuario por su ID
            var user = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException("MSG_Usuario invalido");


            var permissions = await uow.UserPermissionRepository
                .Get(up => up.UserId == userId)
                .Include(up => up.Permission)
                .Select(up => up.Permission)
                .ToListAsync();

            return permissions;
        }
    }
}
