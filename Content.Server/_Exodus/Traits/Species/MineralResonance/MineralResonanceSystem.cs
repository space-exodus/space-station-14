using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Shared.Exodus.Mining;
using Content.Shared.Exodus.Traits.Species.MineralResonance;

namespace Contetn.Server.Exodus.Traits.Species.MineralResonance;

public sealed partial class MineralResonanceSystem : EntitySystem
{
    [Dependency] private readonly MiningScannerViewerSystem _miningScanner = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MineralResonanceComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<MineralResonanceComponent, EmoteEvent>(OnEmote);
        SubscribeLocalEvent<MineralResonanceComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<MineralResonanceComponent, MineralResonanceUseEvent>(OnAction);
    }

    private void OnEmote(Entity<MineralResonanceComponent> entity, ref EmoteEvent args)
    {
        if (args.Emote != entity.Comp.TriggerEmote)
            return;

        ApplyMineralResonance(entity);
    }

    private void OnAction(Entity<MineralResonanceComponent> entity, ref MineralResonanceUseEvent args)
    {
        _chat.TryEmoteWithChat(entity, entity.Comp.TriggerEmote);
    }

    private void ApplyMineralResonance(Entity<MineralResonanceComponent> entity)
    {
        _miningScanner.CreateScan(entity, entity.Comp.ViewRange, entity.Comp.Delay);
    }

    private void OnMapInit(Entity<MineralResonanceComponent> entity, ref MapInitEvent args)
    {
        _actions.AddAction(entity, ref entity.Comp.ActionEntity, entity.Comp.ActionPrototype);
    }

    private void OnShutdown(Entity<MineralResonanceComponent> entity, ref ComponentShutdown args)
    {
        if (entity.Comp.ActionEntity != null && entity.Comp.ActionEntity.Value.IsValid())
        {
            _actions.RemoveAction(entity.Comp.ActionEntity.Value);
        }
    }
}
