using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Data;

public class AppDbContext 
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    

    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<Ballot> Ballots => Set<Ballot>();
    public DbSet<BallotRanking> BallotRankings => Set<BallotRanking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> Votes (creator)
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Creator)
            .WithMany(u => u.CreatedVotes)
            .HasForeignKey(v => v.CreatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Vote -> Options
        modelBuilder.Entity<Option>()
            .HasOne(o => o.Vote)
            .WithMany(v => v.Options)
            .HasForeignKey(o => o.VoteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Vote -> WinningOption
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.WinningOption)
            .WithMany()
            .HasForeignKey(v => v.WinningOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> Ballots
        modelBuilder.Entity<Ballot>()
            .HasOne(b => b.User)
            .WithMany(u => u.Ballots)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vote -> Ballots
        modelBuilder.Entity<Ballot>()
            .HasOne(b => b.Vote)
            .WithMany(v => v.Ballots)
            .HasForeignKey(b => b.VoteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ballot -> Rankings
        modelBuilder.Entity<BallotRanking>()
            .HasOne(br => br.Ballot)
            .WithMany(b => b.Rankings)
            .HasForeignKey(br => br.BallotId)
            .OnDelete(DeleteBehavior.Cascade);

        // Option -> Rankings
        modelBuilder.Entity<BallotRanking>()
            .HasOne(br => br.Option)
            .WithMany(o => o.BallotRankings)
            .HasForeignKey(br => br.OptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Constraints
        modelBuilder.Entity<Ballot>()
            .HasIndex(b => new { b.VoteId, b.UserId })
            .IsUnique();

        modelBuilder.Entity<BallotRanking>()
            .HasIndex(br => new { br.BallotId, br.RankNumber })
            .IsUnique();

        modelBuilder.Entity<BallotRanking>()
            .HasIndex(br => new { br.BallotId, br.OptionId })
            .IsUnique();
    }
}
