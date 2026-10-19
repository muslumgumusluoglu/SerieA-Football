
using Microsoft.EntityFrameworkCore;
using SerieA.API.Entities;

namespace SerieA.API.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<MatchGoal> MatchGoals { get; set; }

        public DbSet<MatchCard> MatchCards { get; set; }

        public DbSet<Substitution> Substitutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchGoal>()
                .HasOne(g => g.Match)
                .WithMany()
                .HasForeignKey(g => g.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchGoal>()
                .HasOne(g => g.Team)
                .WithMany()
                .HasForeignKey(g => g.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchCard>()
                .HasOne(c => c.Match)
                .WithMany()
                .HasForeignKey(c => c.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchCard>()
                .HasOne(c => c.Team)
                .WithMany()
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.Match)
                .WithMany()
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.Team)
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
