using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Infrastructure.Data.Config;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
  public void Configure(EntityTypeBuilder<Player> builder)
  {
    builder.Property(p => p.Id)
      .HasVogenConversion()
      .ValueGeneratedNever()
      .IsRequired();

    builder.Property(p => p.Name)
      .HasVogenConversion()
      .HasMaxLength(PlayerName.MaxLength)
      .IsRequired();

    builder.Property(p => p.GamesPlayed).IsRequired();
    builder.Property(p => p.Wins).IsRequired();
    builder.Property(p => p.Losses).IsRequired();
    builder.Property(p => p.CreatedAt).IsRequired();
    builder.Property(p => p.LastSeenAt).IsRequired();

    builder.Ignore(p => p.WinRate);

    builder.HasIndex(p => p.Wins);
  }
}
