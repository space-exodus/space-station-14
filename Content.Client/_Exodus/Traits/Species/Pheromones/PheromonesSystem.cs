// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Traits.Species.Pheromones;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Exodus.Traits.Species.Pheromones;

public sealed partial class PheromonesSystem : SharedPheromonesSystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IClyde _clyde = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PheromonesComponent, ComponentStartup>(PheromonesStartup);
        SubscribeLocalEvent<PheromonesComponent, ComponentShutdown>(PheromonesShutdown);
        SubscribeLocalEvent<PheromonesCommunicationComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<PheromonesCommunicationComponent, PlayerAttachedEvent>(OnAttached);
        SubscribeLocalEvent<PheromonesCommunicationComponent, PlayerDetachedEvent>(OnDetached);
        SubscribeLocalEvent<PheromonesCommunicationComponent, ComponentShutdown>(OnShutdown);
    }

    public void RefreshPheromonesVisibility()
    {
        var show = CanSeePheromones(_player.LocalEntity);

        var pheromones = EntityQueryEnumerator<PheromonesComponent>();

        while (pheromones.MoveNext(out var uid, out var pheromone))
        {
            if (!TryComp<SpriteComponent>(uid, out var sprite))
                continue;

            pheromone.OldSpriteColor = sprite.Color;
            sprite.Color = pheromone.Color;

            if (pheromone.Hidden)
                sprite.Visible = show;
        }
    }

    private void PheromonesStartup(Entity<PheromonesComponent> entity, ref ComponentStartup args)
    {
        if (!TryComp<SpriteComponent>(entity, out var sprite))
            return;

        var show = CanSeePheromones(_player.LocalEntity);

        if (entity.Comp.Hidden && !show)
        {
            sprite.Visible = false;
        }
        if (show)
        {
            entity.Comp.OldSpriteColor = sprite.Color;
            sprite.Color = entity.Comp.Color;
        }
    }

    private void PheromonesShutdown(Entity<PheromonesComponent> entity, ref ComponentShutdown args)
    {
        if (!TryComp<SpriteComponent>(entity, out var sprite))
            return;

        if (entity.Comp.Hidden)
        {
            sprite.Visible = true;
        }

        if (entity.Comp.OldSpriteColor != null)
            sprite.Color = entity.Comp.OldSpriteColor.Value;
    }

    private void OnMapInit(Entity<PheromonesCommunicationComponent> entity, ref MapInitEvent args)
    {
        if (_player.LocalEntity == entity.Owner)
            RefreshPheromonesVisibility();
    }

    private void OnShutdown(Entity<PheromonesCommunicationComponent> entity, ref ComponentShutdown args)
    {
        if (_player.LocalEntity == entity.Owner)
            RefreshPheromonesVisibility();
    }

    private void OnAttached(Entity<PheromonesCommunicationComponent> entity, ref PlayerAttachedEvent args)
    {
        if (_player.LocalEntity == entity.Owner)
            RefreshPheromonesVisibility();
    }

    private void OnDetached(Entity<PheromonesCommunicationComponent> entity, ref PlayerDetachedEvent args)
    {
        RefreshPheromonesVisibility();
    }
}
