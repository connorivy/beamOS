using System.Collections;
using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public sealed class ModelProposalElement1dStore : IReadOnlyDictionary<Element1dId, Element1d>
{
    public ModelProposalElement1dStore(IList<Element1d> element1ds)
    {
        this.element1dIdToElement1dDict = element1ds.ToDictionary(element1d => element1d.Id);
    }

    private readonly Dictionary<Element1dId, Element1d> element1dIdToElement1dDict;
    private readonly Dictionary<Element1dId, Element1dProposal> modifyElement1dProposalCache = [];
    private readonly List<Element1dProposal> newElement1dProposals = [];

    public Element1d this[Element1dId key] =>
        this.ApplyExistingProposal(this.element1dIdToElement1dDict[key], out _);

    public IEnumerable<Element1dId> Keys => this.element1dIdToElement1dDict.Keys;

    public IEnumerable<Element1d> Values =>
        this.element1dIdToElement1dDict.Values.Select(element1d =>
            this.ApplyExistingProposal(element1d, out _)
        );

    public int Count => this.element1dIdToElement1dDict.Count;

    public bool ContainsKey(Element1dId key) => this.element1dIdToElement1dDict.ContainsKey(key);

    public bool TryGetValue(Element1dId key, [MaybeNullWhen(false)] out Element1d value) =>
        this.element1dIdToElement1dDict.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<Element1dId, Element1d>> GetEnumerator() =>
        this
            .element1dIdToElement1dDict.Select(kvp => new KeyValuePair<Element1dId, Element1d>(
                kvp.Key,
                this.ApplyExistingProposal(kvp.Value, out _)
            ))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public Element1d ApplyExistingProposal(Element1d element1d, out bool isModifiedInProposal)
    {
        if (this.modifyElement1dProposalCache.TryGetValue(element1d.Id, out var proposal))
        {
            isModifiedInProposal = true;
            return proposal.ToDomain(null, null, null);
        }

        isModifiedInProposal = false;
        return element1d;
    }

    public Element1dProposal CreateModifyElement1dProposal(
        Element1d element1d,
        ModelProposalId modelProposalId,
        ExistingOrProposedNodeId? startNodeId = null,
        ExistingOrProposedNodeId? endNodeId = null,
        ExistingOrProposedMaterialId? materialId = null,
        ExistingOrProposedSectionProfileId? sectionProfileId = null
    )
    {
        if (this.modifyElement1dProposalCache.TryGetValue(element1d.Id, out var existingProposal))
        {
            return new Element1dProposal(
                existingProposal,
                startNodeId,
                endNodeId,
                materialId,
                sectionProfileId
            );
        }
        return new Element1dProposal(
            element1d,
            modelProposalId,
            startNodeId,
            endNodeId,
            materialId,
            sectionProfileId
        );
    }

    public void AddElement1dProposals(Element1dProposal proposal)
    {
        if (proposal.ExistingId is not null)
        {
            this.modifyElement1dProposalCache[proposal.ExistingId.Value] = proposal;
        }
        else
        {
            this.newElement1dProposals.Add(proposal);
        }
    }

    public IEnumerable<Element1dProposal> GetElement1dProposals() =>
        this.newElement1dProposals.Concat(this.modifyElement1dProposalCache.Values);
}
