
using GoldRoger.Data;
using GoldRogerServer.Business.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore;





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

    }
}
