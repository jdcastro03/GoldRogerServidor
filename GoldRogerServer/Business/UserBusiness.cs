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

            // Crear nuevo usuario
            var newUser = new User
            {
                Username = requestDTO.Username,
                PasswordHash = HashPassword(requestDTO.PasswordHash), // Hashea la contraseña
                Email = requestDTO.Email,
                FirstName = requestDTO.FirstName,
                LastName = requestDTO.LastName,
                UserType = requestDTO.UserType
            };

            uow.UserRepository.Insert(newUser);
            await uow.SaveAsync();

            // Lógica específica por tipo de usuario
            switch (requestDTO.UserType)
            {
                case 1: // Jugador
                    if (string.IsNullOrWhiteSpace(requestDTO.Position))
                        throw new ArgumentException("La posición es obligatoria para un jugador");

                    var newPlayer = new Player
                    {
                        PlayerId = newUser.UserId,
                        Position = requestDTO.Position,
                        TeamId = requestDTO.TeamId ?? 0
                    };
                    uow.PlayerRepository.Insert(newPlayer);
                    break;

                case 2: // Entrenador
                    if (string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
                        throw new ArgumentException("El número de licencia es obligatorio para un entrenador");

                    var newCoach = new Coach
                    {
                        CoachId = newUser.UserId,
                        LicenseNumber = requestDTO.LicenseNumber
                    };
                    uow.CoachRepository.Insert(newCoach);
                    break;

                case 3: // Organizador
                    if (string.IsNullOrWhiteSpace(requestDTO.OrganizationName))
                        throw new ArgumentException("El nombre de la organización es obligatorio para un organizador");

                    var newOrganizer = new Organizer
                    {
                        OrganizerId = newUser.UserId,
                        OrganizationName = requestDTO.OrganizationName
                    };
                    uow.OrganizerRepository.Insert(newOrganizer);
                    break;

                case 4: // Árbitro
                    if (string.IsNullOrWhiteSpace(requestDTO.LicenseNumber))
                        throw new ArgumentException("El número de licencia es obligatorio para un árbitro");

                    var newReferee = new Referee
                    {
                        RefereeId = newUser.UserId,
                        LicenseNumber = requestDTO.LicenseNumber
                    };
                    uow.RefereeRepository.Insert(newReferee);
                    break;

                default:
                    throw new ArgumentException("Tipo de usuario no válido");
            }

            // Guarda todos los cambios
            await uow.SaveAsync();

            return newUser;
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
        public async Task<User?> Get(string email, string password)
        {
            return await uow.UserRepository
                .Get(u => u.Email == email && u.PasswordHash == password)
                .FirstOrDefaultAsync();
        }

        // Método para actualizar un usuario existente
        public async Task<User> UpdateUser(int userId, UpdateUserRequestDTO requestDTO)
        {
            var userToUpdate = await uow.UserRepository.Get(u => u.UserId == userId).FirstOrDefaultAsync();
            if (userToUpdate == null)
                throw new ArgumentException("Usuario no encontrado");

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(requestDTO.Username) || requestDTO.Username.Trim().Length <= 3)
                throw new ArgumentException("Nombre de usuario inválido");

            if (!string.IsNullOrWhiteSpace(requestDTO.Email) && !IsValidEmail(requestDTO.Email))
                throw new ArgumentException("Correo electrónico inválido");

            if (requestDTO.FirstName.Trim().Length <= 3)
                throw new ArgumentException("Nombre inválido");

            if (requestDTO.LastName.Trim().Length <= 3)
                throw new ArgumentException("Apellido inválido");

            // Actualizar campos
            userToUpdate.Username = requestDTO.Username;
            if (!string.IsNullOrWhiteSpace(requestDTO.PasswordHash))
            {
                userToUpdate.PasswordHash = HashPassword(requestDTO.PasswordHash); // Hashea la nueva contraseña si fue cambiada
            }
            userToUpdate.Email = requestDTO.Email;
            userToUpdate.FirstName = requestDTO.FirstName;
            userToUpdate.LastName = requestDTO.LastName;
            userToUpdate.UserType = requestDTO.UserType;

            uow.UserRepository.Update(userToUpdate);
            await uow.SaveAsync();

            return userToUpdate;
        }

        // Método privado para validar formato de correo electrónico
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