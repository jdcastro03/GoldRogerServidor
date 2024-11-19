
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldRoger.Data;
using GoldRoger.Entity.Entities;
using GoldRogerServer.Business.Core;
using Microsoft.EntityFrameworkCore;
    




namespace GoldRogerServer.Business

{
    public class PlayerBusiness : BaseBusiness
    {

        public PlayerBusiness(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<string?> GetPlayerPositionById(int playerId)
        {
            // Busca el jugador en la base de datos usando el PlayerId
            var player = await uow.PlayerRepository.Get(p => p.PlayerId == playerId).FirstOrDefaultAsync();

            // Si no se encuentra el jugador, lanza una excepción o devuelve null
            if (player == null)
                throw new ArgumentException("Jugador no encontrado");

            // Devuelve la posición del jugador
            return player.Position;
        }



    }
}
