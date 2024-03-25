namespace BeamOs.Application.Common.Commands;

public record GuidBasedIdCommand(Guid Id)
{
    public GuidBasedIdCommand(string id)
        : this(Guid.Parse(id)) { }
};
