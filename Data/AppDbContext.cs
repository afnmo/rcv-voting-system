using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
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

    // Vote -> WinningOption (optional, no back-reference)
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

    // Ballot -> BallotRankings
    modelBuilder.Entity<BallotRanking>()
        .HasOne(br => br.Ballot)
        .WithMany(b => b.Rankings)
        .HasForeignKey(br => br.BallotId)
        .OnDelete(DeleteBehavior.Cascade);

    // Option -> BallotRankings
    modelBuilder.Entity<BallotRanking>()
        .HasOne(br => br.Option)
        .WithMany(o => o.BallotRankings)
        .HasForeignKey(br => br.OptionId)
        .OnDelete(DeleteBehavior.Cascade);

    // Constraints

    // One ballot per user per vote
    modelBuilder.Entity<Ballot>()
        .HasIndex(b => new { b.VoteId, b.UserId })
        .IsUnique();

    // One rank number per ballot
    modelBuilder.Entity<BallotRanking>()
        .HasIndex(br => new { br.BallotId, br.RankNumber })
        .IsUnique();

    // One option per ballot
    modelBuilder.Entity<BallotRanking>()
        .HasIndex(br => new { br.BallotId, br.OptionId })
        .IsUnique();
}

}