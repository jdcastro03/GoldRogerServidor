using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security;
using GoldRoger.Entity.Entities;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using GoldRoger.Data.Maps;
using GoldRoger.Entity.Entities.Security;
using GoldRoger.Data.Maps.Security;



namespace GoldRoger.Data
{
    public class GoldRogerContext : DbContext
    {
        public GoldRogerContext(DbContextOptions<GoldRogerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            new UserTypeMap().Configure(modelBuilder.Entity<UserType>());
            new UserMap().Configure(modelBuilder.Entity<User>());
            new TournamentMap().Configure(modelBuilder.Entity<Tournament>());
            new TournamentTypeMap().Configure(modelBuilder.Entity<TournamentType>());
            new TeamMap().Configure(modelBuilder.Entity<Team>());
            new TeamStatsMap().Configure(modelBuilder.Entity<TeamStats>());
            new RefereeMap().Configure(modelBuilder.Entity<Referee>());
            new PlayerStatsMap().Configure(modelBuilder.Entity<PlayerStats>());
            new PlayerMap().Configure(modelBuilder.Entity<Player>());
            new OrganizerMap().Configure(modelBuilder.Entity<Organizer>());
            new MatchRefereeMap().Configure(modelBuilder.Entity<MatchReferee>());
            new MatchMap().Configure(modelBuilder.Entity<Match>());
            new CoachMap().Configure(modelBuilder.Entity<Coach>());
            new PermissionMap().Configure(modelBuilder.Entity<Permission>());
            new UserPermissionMap().Configure(modelBuilder.Entity<UserPermission>());
            new LeagueStandingMap().Configure(modelBuilder.Entity<LeagueStanding>());
            

        }

        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Team { get; set; }
            

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentType> TournamentType { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<TeamStats> TeamStats { get; set; }
        public DbSet<PlayerStats> PlayerStats { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Referee> Referees { get; set; }
        public DbSet<MatchReferee> MatchReferees { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<UserPermission> UserPermission { get; set; }
        public DbSet<Permission> Permission { get; set; }

        public DbSet<LeagueStanding> LeagueStanding { get; set; }




    }
}

