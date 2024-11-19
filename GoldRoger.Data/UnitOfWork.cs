using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using GoldRoger.Data.Repositories.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using System.Security;
using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoldRoger.Data.Repositories;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRoger.Entity.Entities.Security;




namespace GoldRoger.Data
{
    public class UnitOfWork : IDisposable
    {
        public GoldRogerContext dbcontext;
        private IDbContextTransaction? _currentTransaction;



        public GenericRepository<UserType>? userTypeRepository;
        public GenericRepository<Organizer>? organizerRepository;
        public GenericRepository<Player>? playerRepository;
        public GenericRepository<Team>? teamRepository;
        public GenericRepository<Tournament>? tournamentRepository;
        public GenericRepository<TournamentType>? tournamentTypeRepository;
        public GenericRepository<Match>? matchRepository;
        public GenericRepository<TeamStats>? teamStatsRepository;
        public GenericRepository<PlayerStats>? playerStatsRepository;
        public GenericRepository<Coach>? coachRepository;
        public GenericRepository<Referee>? refereeRepository;
        public GenericRepository<MatchReferee>? matchRefereeRepository;
        public GenericRepository<User>? userRepository;
        public GenericRepository<Permission>? permissionRepository;
        public GenericRepository<UserPermission>? userPermissionRepository;
        public GenericRepository<LeagueStanding>? leagueStandingRepository;






        public UnitOfWork(GoldRogerContext betsContext)
        {
            dbcontext = betsContext;
        }

        public void Save()
        {
            dbcontext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await dbcontext.SaveChangesAsync();
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbcontext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            if (_currentTransaction == null)
            {
                _currentTransaction = dbcontext.Database.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                // Manejar cualquier excepción aquí
                RollbackTransaction();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(dbcontext);
                }
                return userRepository;
            }
        }

        public GenericRepository<UserType> UserTypeRepository
        {
            get
            {
                if (this.userTypeRepository == null)
                {
                    this.userTypeRepository = new GenericRepository<UserType>(dbcontext);
                }
                return userTypeRepository;
            }

        }
        public GenericRepository<Organizer> OrganizerRepository
        {
            get
            {
                if (this.organizerRepository == null)
                {
                    this.organizerRepository = new GenericRepository<Organizer>(dbcontext);
                }
                return organizerRepository;
            }

        }

        public GenericRepository<Player> PlayerRepository
        {
            get
            {
                if (this.playerRepository == null)
                {
                    this.playerRepository = new GenericRepository<Player>(dbcontext);
                }
                return playerRepository;
            }

        }

        public GenericRepository<Team> TeamRepository
        {
            get
            {
                if (this.teamRepository == null)
                {
                    this.teamRepository = new GenericRepository<Team>(dbcontext);
                }
                return teamRepository;
            }

        }

        public GenericRepository<Tournament> TournamentRepository
        {
            get
            {
                if (this.tournamentRepository == null)
                {
                    this.tournamentRepository = new GenericRepository<Tournament>(dbcontext);
                }
                return tournamentRepository;
            }

        }

        public GenericRepository<TournamentType> TournamentTypeRepository
        {
            get
            {
                if (this.tournamentTypeRepository == null)
                {
                    this.tournamentTypeRepository = new GenericRepository<TournamentType>(dbcontext);
                }
                return tournamentTypeRepository;
            }

        }

        public GenericRepository<Match> MatchRepository
        {
            get
            {
                if (this.matchRepository == null)
                {
                    this.matchRepository = new GenericRepository<Match>(dbcontext);
                }
                return matchRepository;
            }

        }
        public GenericRepository<TeamStats> TeamStatsRepository
        {
            get
            {
                if (this.teamStatsRepository == null)
                {
                    this.teamStatsRepository = new GenericRepository<TeamStats>(dbcontext);
                }
                return teamStatsRepository;
            }

        }
        public GenericRepository<PlayerStats> PlayerStatsRepository
        {
            get
            {
                if (this.playerStatsRepository == null)
                {
                    this.playerStatsRepository = new GenericRepository<PlayerStats>(dbcontext);
                }
                return playerStatsRepository;
            }







        }
        public GenericRepository<Coach> CoachRepository
        {
            get
            {
                if (this.coachRepository == null)
                {
                    this.coachRepository = new GenericRepository<Coach>(dbcontext);
                }
                return coachRepository;
            }

        }
        public GenericRepository<Referee> RefereeRepository
        {
            get
            {
                if (this.refereeRepository == null)
                {
                    this.refereeRepository = new GenericRepository<Referee>(dbcontext);
                }
                return refereeRepository;
            }

        }
        public GenericRepository<MatchReferee> MatchRefereeRepository
        {
            get
            {
                if (this.matchRefereeRepository == null)
                {
                    this.matchRefereeRepository = new GenericRepository<MatchReferee>(dbcontext);
                }
                return matchRefereeRepository;
            }

        }

        public GenericRepository<Permission> PermissionRepository
        {
            get
            {
                if (this.permissionRepository == null)
                {
                    this.permissionRepository = new GenericRepository<Permission>(dbcontext);
                }
                return permissionRepository;
            }

        }

        public GenericRepository<UserPermission> UserPermissionRepository
        {
            get
            {
                if (this.userPermissionRepository == null)
                {
                    this.userPermissionRepository = new GenericRepository<UserPermission>(dbcontext);
                }
                return userPermissionRepository;
            }
        }






    }
}