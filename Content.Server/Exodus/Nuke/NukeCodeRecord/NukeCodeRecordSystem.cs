using Content.Server.Nuke;
using Content.Server.Station.Systems;
using Content.Shared.Examine;

namespace Content.Server.Exodus.Nuke.NukeCodeRecord;

public sealed class NukeCodeRecordSystem : EntitySystem
{
    [Dependency] private readonly StationSystem _station = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NukeCodeRecordComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<NukeCodeRecordComponent, ExaminedEvent>(OnExamine);
    }

    private void OnInit(EntityUid uid, NukeCodeRecordComponent component, ComponentInit args)
    {
        TrySetRelativeNukeCode(uid, component);
    }

    private void OnExamine(EntityUid uid, NukeCodeRecordComponent component, ExaminedEvent args)
    {
        if (component.NukeName != null && component.NukeCodes != null)
            args.PushMarkup(Loc.GetString("nuke-codes-record-examine-filled", ("nukeName", component.NukeName), ("nukeCode", component.NukeCodes)));
        else
            args.PushText(Loc.GetString("nuke-codes-record-examine-empty"));
    }

    /// <summary>
    /// Get nuke codes and set them to NukeCodeRecord component
    /// </summary>
    /// <param name="uid">Entity that have component</param>
    /// <param name="component">NukeCodeRecord component</param>
    /// <param name="transform">Tranform component, if we have it</param>
    /// <returns></returns>
    private bool TrySetRelativeNukeCode(
            EntityUid uid,
            NukeCodeRecordComponent component,
            TransformComponent? transform = null)
    {
        if (!Resolve(uid, ref transform))
        {
            return false;
        }

        bool nukeFound = false;
        var owningStation = _station.GetOwningStation(uid);
        var nukes = new List<Entity<NukeComponent>>();
        var query = EntityQueryEnumerator<NukeComponent>();

        if (!query.MoveNext(out var nukeUid, out var nuke))
            return false;

        component.NukeName = MetaData(nukeUid).EntityName;
        component.NukeCodes = nuke.Code;

        return true;
    }
}
