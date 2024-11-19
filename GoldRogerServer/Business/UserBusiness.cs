using GoldRoger.Data;
using GoldRoger.Entity.Entities;
using GoldRogerServer.DTOs.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRogerServer.Business.Core;
using Microsoft.SqlServer.Server;
using GoldRoger.Entity.Entities.Security;

namespace GoldRogerServer.Business
{
    public class UserBusiness : BaseBusiness
    {
        public UserBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        // Método para obtener todos los usuarios
        public async Task<List<User>> GetAllUsers()
        {
            return await uow.UserRepository.Get().ToListAsync();
        }

        //get user by id


        // Método para crear un nuevo usuario
        public async Task<User> CreateUser(CreateUserRequestDTO requestDTO)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(requestDTO.Username) || requestDTO.Username.Trim().Length <= 3)
                throw new ArgumentException("Nombre de usuario inválido");

            if (string.IsNullOrWhiteSpace(requestDTO.PasswordHash))
                throw new ArgumentException("Contraseña no puede estar vacía");

            if (string.IsNullOrWhiteSpace(requestDTO.Email) || !IsValidEmail(requestDTO.Email))
                throw new ArgumentException("Correo electrónico inválido");

            if (requestDTO.FirstName.Trim().Length <= 3)
                throw new ArgumentException("Nombre inválido");

            if (requestDTO.LastName.Trim().Length <= 3)
                throw new ArgumentException("Apellido inválido");

            // Crear el usuario básico
            var newUser = new User
            {
                Username = requestDTO.Username,
                PasswordHash = HashPassword(requestDTO.PasswordHash),
                Email = requestDTO.Email,
                FirstName = requestDTO.FirstName,
                LastName = requestDTO.LastName,
                UserType = requestDTO.UserType
            };

            // Insertar el usuario en la base de datos
            uow.UserRepository.Insert(newUser);
            await uow.SaveAsync();

            // Crear entidad específica basada en UserType
            switch (requestDTO.UserType)
            {
                case 1: // Jugador
                    if (string.IsNullOrWhiteSpace(requestDTO.Position))
                        throw new ArgumentException("La posición es requerida para jugadores.");
                    await CreatePlayer(newUser.UserId, requestDTO.Position);
                    break;

                case 2: // Árbitro
                    if (string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
                        throw new ArgumentException("El campo 'LicenseNumber' es obligatorio para árbitros.");
                    await CreateReferee(newUser.UserId, requestDTO.LicenseNumber);
                    break;

                case 3: // Organizador
                    if (string.IsNullOrWhiteSpace(requestDTO.OrganizationName))
                        throw new ArgumentException("El campo 'OrganizationName' es obligatorio para organizadores.");
                    await CreateOrganizer(newUser.UserId, requestDTO.OrganizationName);
                    break;

                case 4: // Entrenador
                    if (string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
                        throw new ArgumentException("El campo 'LicenseNumber' es obligatorio para entrenadores.");
                    await CreateCoach(newUser.UserId, requestDTO.LicenseNumber);
                    break;

                default:
                    throw new ArgumentException("Tipo de usuario no válido.");
            }

            return newUser;
        }

        // Métodos específicos para crear entidades

        private async Task CreatePlayer(int userId, string position)
        {
            // Crear el jugador
            var player = new Player
            {
                PlayerId = userId,
                Position = position
            };

            // Insertar el jugador en la base de datos
            uow.PlayerRepository.Insert(player);

            // Insertar el permiso en la tabla UserPermission
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = 1003  // ID del permiso de jugador
            };
            uow.UserPermissionRepository.Insert(userPermission);

            // Guardar todos los cambios en la base de datos
            await uow.SaveAsync();
        }

        private async Task CreateReferee(int userId, string licenseNumber)
        {
            var referee = new Referee
            {
                RefereeId = userId,
                LicenseNumber = licenseNumber
            };

            uow.RefereeRepository.Insert(referee);

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = 1005
            };
            uow.UserPermissionRepository.Insert(userPermission);

            await uow.SaveAsync();
        }

        private async Task CreateOrganizer(int userId, string organizationName)
        {
            var organizer = new Organizer
            {
                OrganizerId = userId,
                OrganizationName = organizationName
            };

            uow.OrganizerRepository.Insert(organizer);

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = 1002
            };
            uow.UserPermissionRepository.Insert(userPermission);
            await uow.SaveAsync();
        }

        private async Task CreateCoach(int userId, string licenseNumber)
        {
            var coach = new Coach
            {
                CoachId = userId,
                LicenseNumber = licenseNumber
            };

            uow.CoachRepository.Insert(coach);

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = 1004
            };
            uow.UserPermissionRepository.Insert(userPermission);
            await uow.SaveAsync();
        }

        // Método para eliminar un usuario por ID
        public async Task<bool> DeleteUser(int userId)
        {
            var user = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException("Usuario no encontrado");

            uow.UserRepository.Delete(user);
            await uow.SaveAsync();

            return true;
        }
        public async Task<User> UpdateUser(int userId, UpdateUserRequestDTO requestDTO)
        {
            // Obtener el usuario existente
            var existingUser = await uow.UserRepository.Get().FirstOrDefaultAsync(u => u.UserId == userId);

            if (existingUser == null)
            {
                throw new ArgumentException("Usuario no encontrado.");
            }

            // Validar y actualizar campos generales del usuario
            if (!string.IsNullOrWhiteSpace(requestDTO.Username))
            {
                existingUser.Username = requestDTO.Username;
            }

            if (!string.IsNullOrWhiteSpace(requestDTO.Email))
            {
                existingUser.Email = requestDTO.Email;
            }

            if (!string.IsNullOrWhiteSpace(requestDTO.FirstName))
            {
                existingUser.FirstName = requestDTO.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(requestDTO.LastName))
            {
                existingUser.LastName = requestDTO.LastName;
            }

            // Actualizar datos específicos según el tipo de usuario
            switch (existingUser.UserType)
            {
                case 1: // Jugador
                    await UpdatePlayerData(userId, requestDTO);
                    break;

                case 2: // Árbitro
                    await UpdateRefereeData(userId, requestDTO);
                    break;

                case 3: // Organizador
                    await UpdateOrganizerData(userId, requestDTO);
                    break;

                case 4: // Entrenador
                    await UpdateCoachData(userId, requestDTO);
                    break;

                default:
                    throw new ArgumentException("Tipo de usuario no válido.");
            }

            // Guardar cambios en la base de datos
            uow.UserRepository.Update(existingUser);
            await uow.SaveAsync();

            return existingUser; // Retornar el usuario actualizado
        }

        // Métodos auxiliares para actualizar datos específicos
        private async Task UpdatePlayerData(int userId, UpdateUserRequestDTO requestDTO)
        {
            if (!string.IsNullOrWhiteSpace(requestDTO.Position))
            {
                var player = await uow.PlayerRepository.Get().FirstOrDefaultAsync(p => p.PlayerId == userId);
                if (player != null)
                {
                    player.Position = requestDTO.Position;
                    // Asegúrate de que el objeto se marque como modificado
                    uow.PlayerRepository.Update(player);
                }
            }
        }

        private async Task UpdateRefereeData(int userId, UpdateUserRequestDTO requestDTO)
        {
            if (!string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
            {
                var referee = await uow.RefereeRepository.Get().FirstOrDefaultAsync(r => r.RefereeId == userId);
                if (referee != null)
                {
                    referee.LicenseNumber = requestDTO.LicenseNumber;
                    // Asegúrate de que el objeto se marque como modificado
                    uow.RefereeRepository.Update(referee);
                }
            }
        }

        private async Task UpdateOrganizerData(int userId, UpdateUserRequestDTO requestDTO)
        {
            if (!string.IsNullOrWhiteSpace(requestDTO.OrganizationName))
            {
                var organizer = await uow.OrganizerRepository.Get().FirstOrDefaultAsync(o => o.OrganizerId == userId);
                if (organizer != null)
                {
                    organizer.OrganizationName = requestDTO.OrganizationName;
                    // Asegúrate de que el objeto se marque como modificado
                    uow.OrganizerRepository.Update(organizer);
                }
            }
        }

        private async Task UpdateCoachData(int userId, UpdateUserRequestDTO requestDTO)
        {
            if (!string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
            {
                var coach = await uow.CoachRepository.Get().FirstOrDefaultAsync(c => c.CoachId == userId);
                if (coach != null)
                {
                    coach.LicenseNumber = requestDTO.LicenseNumber;
                    // Asegúrate de que el objeto se marque como modificado
                    uow.CoachRepository.Update(coach);
                }
            }
        }

        // Método para actualizar un usuario existente
        //public async Task<User> UpdateUser(UpdateUserRequestDTO requestDTO)
        //{
        //    //obten todos los usuarios
        //    var userToUpdate = await uow.UserRepository.Get(u => u.UserId == requestDTO.UserId).FirstOrDefaultAsync();
        //    // Validaciones básicas
        //    if (userToUpdate == null)
        //        throw new ArgumentException("Usuario no encontrado");


        //    //actualizame los datos del usuario

        //    userToUpdate.Username = requestDTO.Username;
        //    userToUpdate.Email = requestDTO.Email;
        //    userToUpdate.FirstName = requestDTO.FirstName;
        //    userToUpdate.LastName = requestDTO.LastName;

        //    //ahora actualiza el usuario
        //    uow.UserRepository.Update(userToUpdate);
        //    await uow.SaveAsync();


        //    // Crear el usuario básico



        //    // Crear entidad específica basada en UserType
        //    switch (userToUpdate.UserType)
        //    {
        //        case 1: // Jugador
        //            await UpdatePlayerData(requestDTO.UserId, requestDTO);
        //            break;

        //        case 2: // Árbitro
        //            await UpdateRefereeData(requestDTO.UserId, requestDTO);
        //            break;

        //        case 3: // Organizador
        //            await UpdateOrganizerData(requestDTO.UserId, requestDTO);
        //            break;

        //        case 4: // Entrenador
        //            await UpdateCoachData(requestDTO.UserId, requestDTO);
        //            break;

        //        default:
        //            throw new ArgumentException("Tipo de usuario no válido.");
        //    }

        //    return userToUpdate;
        //}

        //// Métodos específicos para crear entidades

        //// Métodos auxiliares para actualizar datos específicos
        //private async Task UpdatePlayerData(int userId, UpdateUserRequestDTO requestDTO)
        //{
        //    var player = await uow.PlayerRepository.Get().FirstOrDefaultAsync(p => p.PlayerId == userId);
        //    if (player != null)
        //    {
        //        if (!string.IsNullOrWhiteSpace(requestDTO.Position))
        //        {
        //            player.Position = requestDTO.Position;
        //        }
        //        // Puedes añadir más propiedades si es necesario
        //        uow.PlayerRepository.Update(player);
        //    }
        //}

        //private async Task UpdateRefereeData(int userId, UpdateUserRequestDTO requestDTO)
        //{
        //    var referee = await uow.RefereeRepository.Get().FirstOrDefaultAsync(r => r.RefereeId == userId);
        //    if (referee != null)
        //    {
        //        if (!string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
        //        {
        //            referee.LicenseNumber = requestDTO.LicenseNumber;
        //        }
        //        // Puedes añadir más propiedades si es necesario
        //        uow.RefereeRepository.Update(referee);
        //    }
        //}

        //private async Task UpdateOrganizerData(int userId, UpdateUserRequestDTO requestDTO)
        //{
        //    var organizer = await uow.OrganizerRepository.Get().FirstOrDefaultAsync(o => o.OrganizerId == userId);
        //    if (organizer != null)
        //    {
        //        if (!string.IsNullOrWhiteSpace(requestDTO.OrganizationName))
        //        {
        //            organizer.OrganizationName = requestDTO.OrganizationName;
        //        }
        //        // Puedes añadir más propiedades si es necesario
        //        uow.OrganizerRepository.Update(organizer);
        //    }
        //}

        //private async Task UpdateCoachData(int userId, UpdateUserRequestDTO requestDTO)
        //{
        //    var coach = await uow.CoachRepository.Get().FirstOrDefaultAsync(c => c.CoachId == userId);
        //    if (coach != null)
        //    {
        //        if (!string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
        //        {
        //            coach.LicenseNumber = requestDTO.LicenseNumber;
        //        }
        //        // Puedes añadir más propiedades si es necesario
        //        uow.CoachRepository.Update(coach);
        //    }
        //}


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Método privado para encriptar la contraseña
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<User> ValidateUserCredentials(string email, string password)
        {
            var user = await uow.UserRepository.Get(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
    }
}


//public UserBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
//{
//}

//// Método para obtener todos los usuarios
//public async Task<List<User>> GetAllUsers()
//{
//    return await uow.UserRepository.Get().ToListAsync();
//}

//// Método para crear un nuevo usuario
//public async Task<User> CreateUser(CreateUserRequestDTO requestDTO)
//{
//    if (requestDTO.Name.Trim().Length <= 3)
//        throw new ArgumentException("Nombre inválido");
//    if (requestDTO.LastName.Trim().Length <= 3)
//        throw new ArgumentException("Apellido inválido");

//    var newUser = new User
//    {
//        Name = requestDTO.Name,
//        LastName = requestDTO.LastName
//    };

//    uow.UserRepository.Insert(newUser);
//    await uow.SaveAsync();

//    return newUser;
//}

//// Método para eliminar un usuario por ID
//public async Task<bool> DeleteUser(int userId)
//{
//    var user = await uow.UserRepository.Get(u => u.Id == userId).FirstOrDefaultAsync();
//    if (user == null)
//        throw new ArgumentException("Usuario no encontrado");

//    uow.UserRepository.Delete(user);
//    await uow.SaveAsync();

//    return true;
//}

//// Método para actualizar un usuario existente
//public async Task<User> UpdateUser(int userId, UpdateUserRequestDTO requestDTO)
//{
//    var userToUpdate = await uow.UserRepository.Get(u => u.Id == userId).FirstOrDefaultAsync();
//    if (userToUpdate == null)
//        throw new ArgumentException("Usuario no encontrado");

//    if (requestDTO.Name.Trim().Length <= 3)
//        throw new ArgumentException("Nombre inválido");
//    if (requestDTO.LastName.Trim().Length <= 3)
//        throw new ArgumentException("Apellido inválido");

//    userToUpdate.Name = requestDTO.Name;
//    userToUpdate.LastName = requestDTO.LastName;

//    uow.UserRepository.Update(userToUpdate);
//    await uow.SaveAsync();

//    return userToUpdate;
//}