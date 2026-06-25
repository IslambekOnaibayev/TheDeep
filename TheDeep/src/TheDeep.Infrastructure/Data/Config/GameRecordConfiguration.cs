using TheDeep.Core.GameRecordAggregate;

namespace TheDeep.Infrastructure.Data.Config;

public class GameRecordConfiguration : IEntityTypeConfiguration<GameRecord>
{
  public void Configure(EntityTypeBuilder<GameRecord> builder)
  {
    builder.Property(r => r.Id)
      .ValueGeneratedNever()
      .IsRequired();

    builder.Property(r => r.WinnerId)
      .HasVogenConversion()
      .IsRequired();

    builder.Property(r => r.LoserId)
      .HasVogenConversion()
      .IsRequired();

    builder.Property(r => r.WinnerName).HasMaxLength(64).IsRequired();
    builder.Property(r => r.LoserName).HasMaxLength(64).IsRequired();
    builder.Property(r => r.GridWidth).IsRequired();
    builder.Property(r => r.GridHeight).IsRequired();
    builder.Property(r => r.EndedByForfeit).IsRequired();
    builder.Property(r => r.FinishedAt).IsRequired();

    builder.HasIndex(r => r.WinnerId);
    builder.HasIndex(r => r.LoserId);
    builder.HasIndex(r => r.FinishedAt);
  }
}
