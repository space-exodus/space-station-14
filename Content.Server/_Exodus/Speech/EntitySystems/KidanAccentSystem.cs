using System.Text.RegularExpressions;
using Content.Server.Exodus.Speech.Components;
using Content.Shared.Speech;

namespace Content.Server.Exodus.Speech.EntitySystems;

public sealed class KidanAccentSystem : EntitySystem
{
    private static readonly Regex RegexLowerZs = new("з+");
    private static readonly Regex RegexUpperZs = new("З+");
    private static readonly Regex RegexLowerVs = new("в+");
    private static readonly Regex RegexUpperVs = new("В+");
    private static readonly Regex RegexLowerSs = new("с+");
    private static readonly Regex RegexUpperSs = new("С+");
    private static readonly Regex RegexLowerCs = new("ц+");
    private static readonly Regex RegexUpperCs = new("Ц+");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<KidanAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, KidanAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // з => зз
        message = RegexLowerZs.Replace(message, "зз");
        // З => ЗЗ
        message = RegexUpperZs.Replace(message, "ЗЗ");
        // в => вв
        message = RegexLowerVs.Replace(message, "вв");
        // В => ВВ
        message = RegexUpperVs.Replace(message, "ВВ");
        // c => зз
        message = RegexLowerSs.Replace(message, "зз");
        // С => ЗЗ
        message = RegexUpperSs.Replace(message, "ЗЗ");
        // ц => зз
        message = RegexLowerCs.Replace(message, "зз");
        // Ц => ЗЗ
        message = RegexUpperCs.Replace(message, "ЗЗ");

        args.Message = message;
    }
}
