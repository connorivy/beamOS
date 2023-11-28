namespace BeamOS.Common.Application.Commands;
public record GuidBasedIdCommand(Guid Id)
{
    public GuidBasedIdCommand(string id) : this(Guid.Parse(id))
    {
    }
};
