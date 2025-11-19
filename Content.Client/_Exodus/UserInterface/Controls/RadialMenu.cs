// © Space Wizards Federation, A MIT license, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/MIT_LICENSE.TXT
// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
// Made by Space Exodus, based on RadialMenu.cs by Space Wizards Federation at https://github.com/space-exodus/space-station-14/tree/15c03d05d0c4481b6965137dbca2a5c9df9e2e9d/Content.Client/UserInterface/Controls/RadialMenu.cs

using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;

namespace Content.Client.Exodus.UserInterface.Controls;
// In short, this all is same as the RadialMenu.cs but instead of using TextureButton in it's base it uses
// ContainerButton, thats all the difference

/// <summary>
/// Base class for radial menu buttons. Excludes all actions except clicks and alt-clicks
/// from interactions.
/// </summary>
[Virtual]
public class RadialMenuContainerButtonBase : ContainerButton
{
    /// <inheritdoc />
    protected RadialMenuContainerButtonBase()
    {
        EnableAllKeybinds = true;
    }

    /// <inheritdoc />
    protected override void KeyBindUp(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.UIClick
            || args.Function == ContentKeyFunctions.AltActivateItemInWorld)
            base.KeyBindUp(args);
    }
}

[Virtual]
public class RadialMenuContainerButton : RadialMenuContainerButtonBase
{
    /// <summary>
    /// Upon clicking this button the radial menu will be moved to the layer of this control.
    /// </summary>
    public Control? TargetLayer { get; set; }

    /// <summary>
    /// Other way to set navigation to other container, as <see cref="TargetLayer"/>,
    /// but using <see cref="Control.Name"/> property of target <see cref="RadialContainer"/>.
    /// </summary>
    public string? TargetLayerControlName { get; set; }

    /// <summary>
    /// A simple texture button that can move the user to a different layer within a radial menu
    /// </summary>
    public RadialMenuContainerButton()
    {
        EnableAllKeybinds = true;
        OnButtonUp += OnClicked;
    }

    private void OnClicked(ButtonEventArgs args)
    {
        if (TargetLayer == null && TargetLayerControlName == null)
            return;

        var parent = FindParentMultiLayerContainer(this);

        if (parent == null)
            return;

        if (TargetLayer != null)
        {
            parent.TryToMoveToNewLayer(TargetLayer);
        }
        else
        {
            parent.TryToMoveToNewLayer(TargetLayerControlName!);
        }
    }

    private RadialMenu? FindParentMultiLayerContainer(Control control)
    {
        foreach (var ancestor in control.GetSelfAndLogicalAncestors())
        {
            if (ancestor is RadialMenu menu)
                return menu;
        }

        return null;
    }
}

[Virtual]
public class RadialMenuContainerButtonWithSector : RadialMenuContainerButton, IRadialMenuItemWithSector
{
    private Vector2[]? _sectorPointsForDrawing;

    private float _angleSectorFrom;
    private float _angleSectorTo;
    private float _outerRadius;
    private float _innerRadius;
    private float _angleOffset;

    private bool _isWholeCircle;
    private Vector2? _parentCenter;

    private Color _backgroundColorSrgb = Color.ToSrgb(new Color(70, 73, 102, 128));
    private Color _hoverBackgroundColorSrgb = Color.ToSrgb(new Color(87, 91, 127, 128));
    private Color _borderColorSrgb = Color.ToSrgb(new Color(173, 216, 230, 70));
    private Color _hoverBorderColorSrgb = Color.ToSrgb(new Color(87, 91, 127, 128));

    /// <summary>
    /// Marker, that controls if border of segment should be rendered. Is false by default.
    /// </summary>
    /// <remarks>
    /// By default color of border is same as color of background. Use <see cref="BorderColor"/>
    /// and <see cref="HoverBorderColor"/> to change it.
    /// </remarks>
    public bool DrawBorder { get; set; } = false;

    /// <summary>
    /// Marker, that control should render background of all sector. Is true by default.
    /// </summary>
    public bool DrawBackground { get; set; } = true;

    /// <summary>
    /// Color of background in non-hovered state. Accepts RGB color, works with sRGB for DrawPrimitive internally.
    /// </summary>
    public Color BackgroundColor
    {
        get => Color.FromSrgb(_backgroundColorSrgb);
        set => _backgroundColorSrgb = Color.ToSrgb(value);
    }

    /// <summary>
    /// Color of background in hovered state. Accepts RGB color, works with sRGB for DrawPrimitive internally.
    /// </summary>
    public Color HoverBackgroundColor
    {
        get => Color.FromSrgb(_hoverBackgroundColorSrgb);
        set => _hoverBackgroundColorSrgb = Color.ToSrgb(value);
    }

    /// <summary>
    /// Color of button border. Accepts RGB color, works with sRGB for DrawPrimitive internally.
    /// </summary>
    public Color BorderColor
    {
        get => Color.FromSrgb(_borderColorSrgb);
        set => _borderColorSrgb = Color.ToSrgb(value);
    }

    /// <summary>
    /// Color of button border when button is hovered. Accepts RGB color, works with sRGB for DrawPrimitive internally.
    /// </summary>
    public Color HoverBorderColor
    {
        get => Color.FromSrgb(_hoverBorderColorSrgb);
        set => _hoverBorderColorSrgb = Color.ToSrgb(value);
    }

    /// <summary>
    /// Color of separator lines.
    /// Separator lines are used to visually separate sector of radial menu items.
    /// </summary>
    public Color SeparatorColor { get; set; } = new Color(128, 128, 128, 128);

    /// <inheritdoc />
    float IRadialMenuItemWithSector.AngleSectorFrom
    {
        set
        {
            _angleSectorFrom = value;
            _isWholeCircle = IsWholeCircle(value, _angleSectorTo);
        }
    }

    /// <inheritdoc />
    float IRadialMenuItemWithSector.AngleSectorTo
    {
        set
        {
            _angleSectorTo = value;
            _isWholeCircle = IsWholeCircle(_angleSectorFrom, value);
        }
    }

    /// <inheritdoc />
    float IRadialMenuItemWithSector.OuterRadius { set => _outerRadius = value; }

    /// <inheritdoc />
    float IRadialMenuItemWithSector.InnerRadius { set => _innerRadius = value; }

    /// <inheritdoc />
    public float AngleOffset { set => _angleOffset = value; }

    /// <inheritdoc />
    Vector2 IRadialMenuItemWithSector.ParentCenter { set => _parentCenter = value; }

    /// <summary>
    /// A simple texture button that can move the user to a different layer within a radial menu
    /// </summary>
    public RadialMenuContainerButtonWithSector()
    {
    }

    /// <inheritdoc />
    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);

        if (_parentCenter == null)
        {
            return;
        }

        // draw sector where space that button occupies actually is
        var containerCenter = (_parentCenter.Value - Position) * UIScale;

        var angleFrom = _angleSectorFrom + _angleOffset;
        var angleTo = _angleSectorTo + _angleOffset;
        if (DrawBackground)
        {
            var segmentColor = DrawMode == DrawModeEnum.Hover
                ? _hoverBackgroundColorSrgb
                : _backgroundColorSrgb;

            DrawAnnulusSector(handle, containerCenter, _innerRadius * UIScale, _outerRadius * UIScale, angleFrom, angleTo, segmentColor);
        }

        if (DrawBorder)
        {
            var borderColor = DrawMode == DrawModeEnum.Hover
                ? _hoverBorderColorSrgb
                : _borderColorSrgb;
            DrawAnnulusSector(handle, containerCenter, _innerRadius * UIScale, _outerRadius * UIScale, angleFrom, angleTo, borderColor, false);
        }

        if (!_isWholeCircle && DrawBorder)
        {
            DrawSeparatorLines(handle, containerCenter, _innerRadius * UIScale, _outerRadius * UIScale, angleFrom, angleTo, SeparatorColor);
        }
    }

    /// <inheritdoc />
    protected override bool HasPoint(Vector2 point)
    {
        if (_parentCenter == null)
        {
            return base.HasPoint(point);
        }

        var outerRadiusSquared = _outerRadius * _outerRadius;
        var innerRadiusSquared = _innerRadius * _innerRadius;

        var distSquared = (point + Position - _parentCenter.Value).LengthSquared();
        var isInRadius = distSquared < outerRadiusSquared && distSquared > innerRadiusSquared;
        if (!isInRadius)
        {
            return false;
        }

        // difference from the center of the parent to the `point`
        var pointFromParent = point + Position - _parentCenter.Value;

        // Flip Y to get from ui coordinates to natural coordinates
        var angle = MathF.Atan2(-pointFromParent.Y, pointFromParent.X) - _angleOffset;
        if (angle < 0)
        {
            // atan2 range is -pi->pi, while angle sectors are
            // 0->2pi, so remap the result into that range
            angle = MathF.PI * 2 + angle;
        }

        var isInAngle = angle >= _angleSectorFrom && angle < _angleSectorTo;
        return isInAngle;
    }

    /// <summary>
    /// Draw segment between two concentrated circles from and to certain angles.
    /// </summary>
    /// <param name="drawingHandleScreen">Drawing handle, to which rendering should be delegated.</param>
    /// <param name="center">Point where circle center should be.</param>
    /// <param name="radiusInner">Radius of internal circle.</param>
    /// <param name="radiusOuter">Radius of external circle.</param>
    /// <param name="angleSectorFrom">Angle in radian, from which sector should start.</param>
    /// <param name="angleSectorTo">Angle in radian, from which sector should start.</param>
    /// <param name="color">Color for drawing.</param>
    /// <param name="filled">Should figure be filled, or have only border.</param>
    private void DrawAnnulusSector(
        DrawingHandleScreen drawingHandleScreen,
        Vector2 center,
        float radiusInner,
        float radiusOuter,
        float angleSectorFrom,
        float angleSectorTo,
        Color color,
        bool filled = true
    )
    {
        const float minimalSegmentSize = MathF.Tau / 128f;

        var requestedSegmentSize = angleSectorTo - angleSectorFrom;
        var segmentCount = (int)(requestedSegmentSize / minimalSegmentSize) + 1;
        var anglePerSegment = requestedSegmentSize / (segmentCount - 1);

        var bufferSize = segmentCount * 2;
        if (_sectorPointsForDrawing == null || _sectorPointsForDrawing.Length != bufferSize)
        {
            _sectorPointsForDrawing ??= new Vector2[bufferSize];
        }

        for (var i = 0; i < segmentCount; i++)
        {
            var angle = angleSectorFrom + anglePerSegment * i;

            // Flip Y to get from ui coordinates to natural coordinates
            var unitPos = new Vector2(MathF.Cos(angle), -MathF.Sin(angle));
            var outerPoint = center + unitPos * radiusOuter;
            var innerPoint = center + unitPos * radiusInner;
            if (filled)
            {
                // to make filled sector we need to create strip from triangles
                _sectorPointsForDrawing[i * 2] = outerPoint;
                _sectorPointsForDrawing[i * 2 + 1] = innerPoint;
            }
            else
            {
                // to make border of sector we need points ordered as sequences on radius
                _sectorPointsForDrawing[i] = outerPoint;
                _sectorPointsForDrawing[bufferSize - 1 - i] = innerPoint;
            }
        }

        var type = filled
            ? DrawPrimitiveTopology.TriangleStrip
            : DrawPrimitiveTopology.LineStrip;
        drawingHandleScreen.DrawPrimitives(type, _sectorPointsForDrawing, color);
    }

    private static void DrawSeparatorLines(
        DrawingHandleScreen drawingHandleScreen,
        Vector2 center,
        float radiusInner,
        float radiusOuter,
        float angleSectorFrom,
        float angleSectorTo,
        Color color
    )
    {
        var fromPoint = new Angle(-angleSectorFrom).RotateVec(Vector2.UnitX);
        drawingHandleScreen.DrawLine(
            center + fromPoint * radiusOuter,
            center + fromPoint * radiusInner,
            color
        );

        var toPoint = new Angle(-angleSectorTo).RotateVec(Vector2.UnitX);
        drawingHandleScreen.DrawLine(
            center + toPoint * radiusOuter,
            center + toPoint * radiusInner,
            color
        );
    }

    private static bool IsWholeCircle(float angleSectorFrom, float angleSectorTo)
    {
        return new Angle(angleSectorFrom).EqualsApprox(new Angle(angleSectorTo));
    }
}
