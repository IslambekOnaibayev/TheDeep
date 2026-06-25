using Vogen;
using TheDeep.Core.PlayerAggregate;

namespace TheDeep.Infrastructure.Data.Config;

[EfCoreConverter<PlayerId>]
[EfCoreConverter<PlayerName>]
internal partial class VogenEfCoreConverters;
