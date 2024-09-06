namespace BeamOs.Contracts.Editor;

public record SetColorFilter(
    ICollection<string> BeamOsIds,
    bool ColorAllOthers,
    string ColorHex,
    bool Ghost
);

public record ClearFilters(ICollection<string> BeamOsIds, bool ColorAllOthers);
