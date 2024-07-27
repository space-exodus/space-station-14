using Content.Shared.Actions;
using Content.Shared.Exodus.Abilities.Events;
using Content.Shared.Magic.Events;
using Robust.Shared.Map;
using System.Numerics;

namespace Content.Shared.Exodus.Abilities;
public abstract partial class SharedAbilitiesSystem
{
    public void InitializeAbilityCast()
    {
        SubscribeLocalEvent<AbilitiesComponent, WorldAbilitiesGroupEvent>(OnWorldAbilitiesGroupEvent);
        SubscribeLocalEvent<AbilitiesComponent, EntityAbilitiesGroupEvent>(OnEntityAbilitiesGroupEvent);
        SubscribeLocalEvent<AbilitiesComponent, EntityToCoordEvent>(OnEntityToCoordEvent);
        SubscribeLocalEvent<AbilitiesComponent, ChangeTargetCoordsToPerformerEvent>(OnChangeTargetCoordsToPerformerEvent);

        SubscribeLocalEvent<AbilitiesComponent, ConstantDirectionEvent>(OnConstantDirectionEvent);
        SubscribeLocalEvent<AbilitiesComponent, ConstantWorldEvent>(OnConstantWorldEvent);

        SubscribeLocalEvent<AbilitiesComponent, DelayedWorldTargetAbilityEvent>(OnDelayedAbilityEvent);
        SubscribeLocalEvent<AbilitiesComponent, DelayedEntityAbilityEvent>(OnDelayedAbilityEvent);
        SubscribeLocalEvent<AbilitiesComponent, DelayedInstantAbilityEvent>(OnDelayedAbilityEvent);
        SubscribeLocalEvent<AbilitiesComponent, DelayedDirectionalAbilityEvent>(OnDelayedAbilityEvent);

        SubscribeLocalEvent<AbilitiesComponent, WorldMultishootEvent>(OnWorldMultyshoot);

        SubscribeLocalEvent<AbilitiesComponent, DirectionProjectileSpellEvent>(OnDirectionProjectileSpellEvent);
        SubscribeLocalEvent<AbilitiesComponent, DirectionalLineSpawnEvent>(OnDirectionalLineSpawnEvent);


    }

    public void CopyEventArgs(BaseActionEvent parentEvent, ref BaseActionEvent childrenEvent)
    {
        if (parentEvent is EntityTargetActionEvent parentEntTargEv &&
            childrenEvent is EntityTargetActionEvent childrenEntTargEv)
        {
            childrenEntTargEv.Target = parentEntTargEv.Target;
            childrenEvent = childrenEntTargEv;
        }
        else if (parentEvent is WorldTargetActionEvent parentWorldTargEv &&
                 childrenEvent is WorldTargetActionEvent childrenWorldTargEv)
        {
            childrenWorldTargEv.Target = parentWorldTargEv.Target;
            childrenEvent = childrenWorldTargEv;
        }
        else if (parentEvent is InstantActionEvent parentInstEv &&
                 childrenEvent is InstantActionEvent childrenInstEv)
        {
            childrenEvent = childrenInstEv;
        }
        else if (parentEvent is DirectionActionEvent parentDirectEv &&
                 childrenEvent is DirectionActionEvent childrenDirectEv)
        {
            childrenDirectEv.Direction = parentDirectEv.Direction;
            childrenEvent = childrenDirectEv;
        }
        else
        {
            Logger.Error("Cannot load action event args");
        }

        childrenEvent.Performer = parentEvent.Performer;
    }

    #region AbilityModifyers
    private void OnEntityAbilitiesGroupEvent(EntityUid uid, AbilitiesComponent component, EntityAbilitiesGroupEvent args)
    {
        foreach (var ev in args.Events)
        {
            if (ev is EntityTargetActionEvent entEv)
            {
                var targ = new SimpleEntityTargetActionEvent() { Target = args.Target };
                RaiseAbilityEvent(uid, entEv, targ);
            }
        }
        args.Handled = true;
    }

    private void OnWorldAbilitiesGroupEvent(EntityUid uid, AbilitiesComponent component, WorldAbilitiesGroupEvent args)
    {
        foreach (var ev in args.Events)
        {
            if (ev is BaseActionEvent worldEv)
            {
                var targ = new SimpleWorldTargetActionEvent() { Target = args.Target };
                RaiseAbilityEvent(uid, worldEv, targ);
            }
        }
        args.Handled = true;
    }

    private void OnEntityToCoordEvent(EntityUid uid, AbilitiesComponent component, EntityToCoordEvent args)
    {
        if (args.Event is WorldTargetActionEvent worldEv)
        {
            var targ = new SimpleWorldTargetActionEvent() { Target = Transform(args.Target).Coordinates };
            RaiseAbilityEvent(uid, worldEv, targ);
        }
        args.Handled = true;
    }

    private void OnChangeTargetCoordsToPerformerEvent(EntityUid uid, AbilitiesComponent component, ChangeTargetCoordsToPerformerEvent args)
    {
        if (args.Event is not null)
        {
            var targ = new SimpleWorldTargetActionEvent() { Target = Transform(uid).Coordinates };
            RaiseAbilityEvent(uid, args.Event, targ);
        }
        args.Handled = true;
    }

    private void OnDelayedAbilityEvent(EntityUid uid, AbilitiesComponent component, IDelayedAbilityEvent args)
    {
        BaseActionEvent? argsBaseAction = null;
        BaseActionEvent? abilityToRise = null;
        BaseActionEvent? target = null;

        if (args is BaseActionEvent @event)
            argsBaseAction = @event;
        else return;

        if (args.BaseEvent is InstantActionEvent instant && args is DelayedInstantAbilityEvent argsInstant)
        {
            instant.Handled = false;
            instant.Performer = argsInstant.Performer;
            target = new SimpleInstantActionEvent();
            abilityToRise = instant;
        }
        if (args.BaseEvent is EntityTargetActionEvent entityTarget && args is DelayedEntityAbilityEvent argsEntityTarget)
        {
            entityTarget.Handled = false;
            entityTarget.Performer = argsEntityTarget.Performer;
            target = new SimpleEntityTargetActionEvent() { Target = argsEntityTarget.Target };
            abilityToRise = entityTarget;
        }
        if (args.BaseEvent is WorldTargetActionEvent worldTarget && args is DelayedWorldTargetAbilityEvent argsWorldTarget)
        {
            worldTarget.Handled = false;
            worldTarget.Performer = argsWorldTarget.Performer;
            target = new SimpleWorldTargetActionEvent() { Target = argsWorldTarget.Target };
            abilityToRise = worldTarget;
        }
        if (args.BaseEvent is DirectionActionEvent direct && args is DelayedDirectionalAbilityEvent argsDirect)
        {
            direct.Handled = false;
            direct.Performer = argsDirect.Performer;
            target = new SimpleDirectionActionEvent() { Direction = argsDirect.Direction };
            abilityToRise = direct;
        }

        if (abilityToRise == null || target == null)
            return;

        AddOccuringAbility(uid, args.Delay, abilityToRise, target);

        argsBaseAction.Handled = true;
    }

    private void OnConstantDirectionEvent(EntityUid uid, AbilitiesComponent component, ConstantDirectionEvent args)
    {
        if (args.Event == null)
            return;

        var targ = new SimpleDirectionActionEvent() { Direction = args.ConstDirection };

        args.Handled = RaiseAbilityEvent(uid, args.Event, targ);
    }

    private void OnConstantWorldEvent(EntityUid uid, AbilitiesComponent component, ConstantWorldEvent args)
    {
        if (args.Event == null)
            return;

        var xform = Transform(uid);

        var targ = new SimpleWorldTargetActionEvent() { Target = new(xform.Coordinates.EntityId, xform.Coordinates.Position + args.ConstOffset) };

        args.Handled = RaiseAbilityEvent(uid, args.Event, targ);
    }

    #endregion


    #region ExecutedAbilities
    private void OnDirectionProjectileSpellEvent(EntityUid uid, AbilitiesComponent component, DirectionProjectileSpellEvent args)
    {
        var perfXform = Transform(uid);
        var grid = perfXform.GridUid;

        var throwProjectile = new ProjectileSpellEvent()
        {
            Prototype = args.Prototype,
            Speech = args.Speech
        };

        var targ = new SimpleWorldTargetActionEvent()
        {
            Target = grid == null
                     ? new EntityCoordinates(perfXform.Coordinates.EntityId, args.Direction + perfXform.Coordinates.Position)
                     : new EntityCoordinates(perfXform.Coordinates.EntityId, Vector2.Transform(args.Direction, Transform(grid.Value).InvLocalMatrix) + perfXform.Coordinates.Position)
        };

        RaiseAbilityEvent(uid, throwProjectile, targ);
    }

    private void OnDirectionalLineSpawnEvent(EntityUid uid, AbilitiesComponent component, DirectionalLineSpawnEvent args)
    {
        var performerCoord = Transform(uid).Coordinates;

        var lineSpawn = new WorldLineSpawnEvent()
        {
            SpawnProto = args.SpawnProto,
            LenMultiply = args.LenMultiply,
            StepDelay = args.StepDelay,
            Step = args.Step
        };

        var targ = new SimpleWorldTargetActionEvent()
        {
            Target = new EntityCoordinates(performerCoord.EntityId, args.Direction + performerCoord.Position)
        };

        RaiseAbilityEvent(uid, lineSpawn, targ);
    }

    #endregion


    private void OnWorldMultyshoot(EntityUid uid, AbilitiesComponent component, WorldMultishootEvent args)
    {
        var targetCoord = args.Target;
        var performerCoord = Transform(args.Performer).Coordinates;

        var direction = TransformSystem.ToMapCoordinates(targetCoord).Position - TransformSystem.ToMapCoordinates(performerCoord).Position;

        var shootEnumerator = new ShootEnumerator(direction, args.Angle, args.ShootCnt, args.Clockwise);

        while (shootEnumerator.MoveNext())
        {
            var curDirection = shootEnumerator.Current;
            var curNum = shootEnumerator.CurrentNum;

            if (args.Event is not null)
            {
                var targ = new SimpleDirectionActionEvent()
                {
                    Direction = curDirection
                };
                AddOccuringAbility(uid, args.ShootDelay * (curNum - 1), args.Event, targ);
            }
        }
        args.Handled = true;
    }


    private bool RaiseAbilityEvent(EntityUid performer, BaseActionEvent abilityEvent, BaseActionEvent target)
    {
        BaseActionEvent? eventToRise = null;

        switch (abilityEvent)
        {
            case WorldTargetActionEvent worldTargetEv:
                if (target is WorldTargetActionEvent targ)
                {
                    worldTargetEv.Target = targ.Target;
                    eventToRise = worldTargetEv;
                }
                break;
            case EntityTargetActionEvent entityTargetEv:
                if (target is EntityTargetActionEvent ent)
                {
                    entityTargetEv.Target = ent.Target;
                    eventToRise = entityTargetEv;
                }
                break;
            case DirectionActionEvent directinEv:
                if (target is DirectionActionEvent direct)
                {
                    directinEv.Direction = direct.Direction;
                    eventToRise = directinEv;
                }
                break;
            case InstantActionEvent instantActionEv:
                if (target is InstantActionEvent)
                {
                    eventToRise = instantActionEv;
                }
                break;
            default:
                break;
        }

        if (eventToRise == null)
            return false;

        eventToRise.Performer = performer;
        eventToRise.Handled = false;

        RaiseLocalEvent(performer, (object) eventToRise, true);

        return eventToRise.Handled;
    }

    public struct ShootEnumerator()
    {
        private Vector2 _targetDir, _targetVector;
        private readonly float _centreAngle, _stepAngle;
        private float _curAngle;
        private readonly float _lenShoot;
        private int _curShoot;
        private int _shootCnt;
        private readonly int _clockNum;

        public ShootEnumerator(Vector2 targetVector, float angle, int shootCnt, bool clockwise) : this()
        {
            _targetVector = targetVector;
            _lenShoot = targetVector.Length();
            _targetDir = _targetVector / _lenShoot;

            _centreAngle = (float) Math.Acos(_targetDir.X);

            _centreAngle *= Math.Sign(Math.Asin(_targetDir.Y));

            _clockNum = clockwise ? -1 : 1;

            _shootCnt = shootCnt;
            _curShoot = 0;

            _curAngle = (_shootCnt > 1) ? (-angle * _clockNum / 2) : 0;
            _curAngle += _centreAngle;
            _stepAngle = (_shootCnt > 1) ? (angle / (_shootCnt - 1)) : 0;
        }

        public readonly Vector2 Current => new(_lenShoot * (float) Math.Cos(_curAngle), _lenShoot * (float) Math.Sin(_curAngle));
        public readonly int CurrentNum => _curShoot;

        public bool MoveNext()
        {
            if (_curShoot >= _shootCnt)
                return false;

            if (_curShoot != 0)
                _curAngle += _clockNum * _stepAngle;

            _curShoot += 1;

            return true;
        }
    }

}
