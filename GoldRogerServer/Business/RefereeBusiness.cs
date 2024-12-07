
using GoldRoger.Data;
using GoldRogerServer.Business.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using GoldRogerServer.DTOs.Match;
using System.Text.RegularExpressions;
using GoldRogerServer.DTOs.Referee;





namespace GoldRogerServer.Business
{
    public class RefereeBusiness : BaseBusiness
    {
        public RefereeBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<string?> GetLicenseNumberById(int refereeId)
        {
            // Busca el árbitro en la base de datos usando el RefereeId
            var referee = await uow.RefereeRepository.Get(r => r.RefereeId == refereeId).FirstOrDefaultAsync();

            // Si no se encuentra el árbitro, lanza una excepción o devuelve null
            if (referee == null)
                throw new ArgumentException("Árbitro no encontrado");

            // Devuelve el número de licencia del árbitro
            return referee.LicenseNumber;
        }

        //metodo para obtener todos los refereeid de la tabla referees y con es id devolver el firstname y lastname, me devolveras los refereeid, y con ese id en la tabla users me devolveras el firstname y lastname usa apiresponse

   public async Task<List<RefereeDTO>> GetAllReferees()
{
    // Busca todos los árbitros en la base de datos, incluyendo la relación con 'User'
    var referees = await uow.RefereeRepository
        .Get()
        .Include(r => r.User) // Asegura que se cargue la relación 'User'
        .ToListAsync();

    // Crea una lista de DTOs de árbitros
    var refereeDTOs = new List<RefereeDTO>();

    // Por cada árbitro en la lista de árbitros
    foreach (var referee in referees)
    {
        // Verificar si referee.User no es null
        if (referee.User != null)
        {
            // Crea un DTO de árbitro
            var refereeDTO = new RefereeDTO
            {
                RefereeId = referee.RefereeId,
                FirstName = referee.User.FirstName,
                LastName = referee.User.LastName
            };

            // Agrega el DTO de árbitro a la lista de DTOs de árbitros
            refereeDTOs.Add(refereeDTO);
        }
    }

    // Devuelve la lista de DTOs de árbitros
    return refereeDTOs;
}




        //metodo para insertar matchid y refereeid usa apiresponse y insertrefereedto
        public async Task InsertMatchReferee(InsertRefereeDTO insertRefereeDTO)
        {
            // Busca el árbitro en la base de datos usando el RefereeId
            var referee = await uow.RefereeRepository.Get(r => r.RefereeId == insertRefereeDTO.RefereeId).FirstOrDefaultAsync();

            // Si no se encuentra el árbitro, lanza una excepción
            if (referee == null)
                throw new ArgumentException("Árbitro no encontrado");

            // Busca el partido en la base de datos usando el MatchId
            var match = await uow.MatchRepository.Get(m => m.MatchId == insertRefereeDTO.MatchId).FirstOrDefaultAsync();

            // Si no se encuentra el partido, lanza una excepción
            if (match == null)
                throw new ArgumentException("Partido no encontrado");

            // Crea una nueva relación entre el árbitro y el partido
            var matchReferee = new MatchReferee
            {
                MatchId = insertRefereeDTO.MatchId,
                RefereeId = insertRefereeDTO.RefereeId
            };

            // Agrega la relación entre el árbitro y el partido a la base de datos
            uow.MatchRefereeRepository.Insert(matchReferee);

            // Guarda los cambios en la base de datos
            await uow.SaveAsync();
        }



        //metodo para actualizar el refereeid de la tabla matchreferee utiliza el insertrefereedto y apiresponse
        public async Task UpdateMatchReferee(int matchId, int newRefereeId)
        {
            // Busca la relación entre el partido y el árbitro en la base de datos usando el MatchId
            var matchReferee = await uow.MatchRefereeRepository.Get(mr => mr.MatchId == matchId).FirstOrDefaultAsync();

            // Si no se encuentra la relación entre el partido y el árbitro, lanza una excepción
            if (matchReferee == null)
                throw new ArgumentException("Relación entre partido y árbitro no encontrada");

            // Busca el árbitro en la base de datos usando el RefereeId
            var referee = await uow.RefereeRepository.Get(r => r.RefereeId == newRefereeId).FirstOrDefaultAsync();

            // Si no se encuentra el árbitro, lanza una excepción
            if (referee == null)
                throw new ArgumentException("Árbitro no encontrado");

            // Actualiza el árbitro en la relación entre el partido y el árbitro
            matchReferee.RefereeId = newRefereeId;

            // Asegúrate de que la entidad esté siendo rastreada correctamente
            uow.dbcontext.Entry(matchReferee).State = EntityState.Modified;

            // Actualiza la relación entre el partido y el árbitro en la base de datos
            uow.MatchRefereeRepository.Update(matchReferee);

            // Guarda los cambios
            await uow.SaveAsync();
        }












        //metodo pasara de parametro matchid, y buscaras que refereeid coincide con ese en matchreferees, posteriormente usaras ese refereeid para buscar en la tabla user el firstname y lastname, haciendo una relacion entre las tablas, usa apiresponse y refereedto
        public async Task<List<RefereeDTO>> GetRefereeByMatchId(int matchId)
        {
            // Busca las relaciones entre partidos y árbitros en la base de datos usando el MatchId
            var matchReferees = await uow.MatchRefereeRepository.Get(mr => mr.MatchId == matchId).ToListAsync();

            // Crea una lista de DTOs de árbitros
            var refereeDTOs = new List<RefereeDTO>();

            // Por cada relación entre partido y árbitro en la lista de relaciones
            foreach (var matchReferee in matchReferees)
            {
                // Busca el árbitro en la base de datos usando el RefereeId
                var referee = await uow.RefereeRepository.Get(r => r.RefereeId == matchReferee.RefereeId).FirstOrDefaultAsync();

                // Si no se encuentra el árbitro, continúa con la siguiente relación
                if (referee == null)
                    continue;

                // Busca el usuario en la base de datos usando el RefereeId
                var user = await uow.UserRepository.Get(u => u.UserId == referee.RefereeId).FirstOrDefaultAsync();

                // Si no se encuentra el usuario, continúa con la siguiente relación
                if (user == null)
                    continue;

                // Crea un DTO de árbitro
                var refereeDTO = new RefereeDTO
                {
                    RefereeId = referee.RefereeId,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                // Agrega el DTO de árbitro a la lista de DTOs de árbitros
                refereeDTOs.Add(refereeDTO);
            }

            // Devuelve la lista de DTOs de árbitros
            return refereeDTOs;
        }















    }
}
