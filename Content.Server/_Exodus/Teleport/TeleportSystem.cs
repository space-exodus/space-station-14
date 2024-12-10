using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Server.Explosion.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server.Exodus.Teleport;

public sealed partial class TeleportSystem : EntitySystem
{
    [Dependency] private readonly MapSystem _map = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TeleportOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(EntityUid uid, TeleportOnTriggerComponent component, ref TriggerEvent ev)
    {
        // если предмет был активирован без пользователя, то оканчиваем выполнение
        // если пользователь есть, то мы записываем его как NON-null переменную user
        if (ev.User is not { } user)
            return;

        // получаем компонент Transform пользоватея
        var xform = Transform(user);

        // если пользователь не находится на каком-либо гриде, то билет просто сгорает
        if (xform.GridUid == null)
            return;

        // получаем случайную позицию, если позицию получить нельзя, то ничего не происходит
        if (TryGetRandomTeleportPosition(xform.GridUid.Value, out var position))
        {
            // телепортируем
            _transform.SetLocalPosition(user, position.Value);
        }
    }

    private bool TryGetRandomTeleportPosition(EntityUid gridUid, [NotNullWhen(true)] out Vector2? position, MapGridComponent? mapGrid = null)
    {
        position = null;

        if (!Resolve(gridUid, ref mapGrid))
            return false;

        var tiles = _map.GetAllTiles(gridUid, mapGrid);
        var tile = _random.Pick(tiles.ToList());

        position = new(tile.X, tile.Y);

        return true;
    }
}
