using Content.Server.Objectives.Components;
using Content.Shared._DV.Roles;
using Content.Shared.Ninja.Components;
using Content.Shared.Objectives.Components;
using Content.Shared.Roles;
using Content.Shared.Warps;
using Robust.Shared.Random;

namespace Content.Server.Objectives.Systems;

public sealed class CosmicCultObjectiveSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly NumberObjectiveSystem _number = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedRoleSystem _roles = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CosmicEffigyConditionComponent, RequirementCheckEvent>(OnEffigyRequirementCheck);
        SubscribeLocalEvent<CosmicEffigyConditionComponent, ObjectiveAfterAssignEvent>(OnEffigyAfterAssign);

        SubscribeLocalEvent<CosmicEntropyConditionComponent, ObjectiveGetProgressEvent>(OnGetEntropyProgress);
        SubscribeLocalEvent<CosmicConversionConditionComponent, ObjectiveGetProgressEvent>(OnGetConversionProgress);
        SubscribeLocalEvent<CosmicTierConditionComponent, ObjectiveGetProgressEvent>(OnGetTierProgress);
        SubscribeLocalEvent<CosmicVictoryConditionComponent, ObjectiveGetProgressEvent>(OnGetVictoryProgress);
    }

    private void OnGetEntropyProgress(Entity<CosmicEntropyConditionComponent> ent, ref ObjectiveGetProgressEvent args) =>
        args.Progress = Progress(ent.Comp.Siphoned, _number.GetTarget(ent.Owner));

    private void OnGetTierProgress(Entity<CosmicTierConditionComponent> ent, ref ObjectiveGetProgressEvent args) =>
        args.Progress = Progress(ent.Comp.Tier, _number.GetTarget(ent.Owner));

    private void OnGetVictoryProgress(Entity<CosmicVictoryConditionComponent> ent, ref ObjectiveGetProgressEvent args) =>
        args.Progress = ent.Comp.Victory ? 1f : 0f;

    private static float Progress(int recruited, int target)
    {
        // prevent divide-by-zero
        if (target == 0)
            return 1f;

        return MathF.Min(recruited / (float) target, 1f);
    }
}
