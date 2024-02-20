using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOs.Identity.Domain.UserAggregate.ValueObjects;

public class BeamOsUserId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<BeamOsUserId, Guid>,
        IEquatable<BeamOsUserId>
{
    public static BeamOsUserId Construct(Guid t1) => new(t1);

    public bool Equals(BeamOsUserId? other) => this.Equals((object?)other);
}
