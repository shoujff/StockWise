using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace StockWise.App.Services;

public enum AppTheme { Dark, Light }

public class ThemeService
{
    private AppTheme _current = AppTheme.Dark;

    public AppTheme Current => _current;

    public event Action<AppTheme>? ThemeChanged;

    public void Initialize(AppTheme theme = AppTheme.Dark)
    {
        _current = theme;
        ApplyTheme();
    }

    public void Toggle()
    {
        _current = _current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        ApplyTheme();
        ThemeChanged?.Invoke(_current);
    }

    public void SetTheme(AppTheme theme)
    {
        if (_current == theme) return;
        _current = theme;
        ApplyTheme();
        ThemeChanged?.Invoke(_current);
    }

    private void ApplyTheme()
    {
        var variant = _current == AppTheme.Dark ? ThemeVariant.Dark : ThemeVariant.Light;
        Application.Current.RequestedThemeVariant = variant;

        var res = Application.Current.Resources;

        if (_current == AppTheme.Dark)
        {
            // ── Base Backgrounds ──
            res["BgPrimary"] = new SolidColorBrush(Color.Parse("#0A0A0F"));
            res["BgSecondary"] = new SolidColorBrush(Color.Parse("#0D0D14"));
            res["BgTertiary"] = new SolidColorBrush(Color.Parse("#14141F"));
            res["BgElevated"] = new SolidColorBrush(Color.Parse("#181828"));

            // ── Surfaces ──
            res["CardBg"] = new SolidColorBrush(Color.Parse("#0D0D14"));
            res["SidebarBg"] = new SolidColorBrush(Color.Parse("#0C0C14"));
            res["SidebarHover"] = new SolidColorBrush(Color.Parse("#181828"));
            res["SidebarSelected"] = new SolidColorBrush(Color.Parse("#0D1420"));
            res["SidebarText"] = new SolidColorBrush(Color.Parse("#6A6A80"));
            res["SidebarTextSelected"] = new SolidColorBrush(Color.Parse("#E8E8F0"));
            res["WorkspaceBg"] = new SolidColorBrush(Color.Parse("#0A0A0F"));
            res["HeaderBg"] = new SolidColorBrush(Color.Parse("#0D0D14"));
            res["PageHeaderBg"] = new SolidColorBrush(Color.Parse("#0D0D14"));

            // ── Borders ──
            res["BorderPrimary"] = new SolidColorBrush(Color.Parse("#1A1A2E"));
            res["BorderSecondary"] = new SolidColorBrush(Color.Parse("#22223A"));
            res["BorderAccent"] = new SolidColorBrush(Color.Parse("#00FFFF33"));

            // ── Text ──
            res["TextPrimary"] = new SolidColorBrush(Color.Parse("#E8E8F0"));
            res["TextSecondary"] = new SolidColorBrush(Color.Parse("#8A8AA0"));
            res["TextTertiary"] = new SolidColorBrush(Color.Parse("#6A6A80"));
            res["TextMuted"] = new SolidColorBrush(Color.Parse("#555566"));
            res["TextHint"] = new SolidColorBrush(Color.Parse("#444458"));
            res["TextInverse"] = new SolidColorBrush(Color.Parse("#0A0A0F"));

            // ── Fields ──
            res["FieldBg"] = new SolidColorBrush(Color.Parse("#14141F"));
            res["FieldBgFocus"] = new SolidColorBrush(Color.Parse("#181828"));
            res["FieldBorder"] = new SolidColorBrush(Color.Parse("#1A1A2E"));
            res["FieldBorderFocus"] = new SolidColorBrush(Color.Parse("#00FFFF"));

            // ── Accent Colors ──
            res["AccentCyan"] = new SolidColorBrush(Color.Parse("#00FFFF"));
            res["AccentMagenta"] = new SolidColorBrush(Color.Parse("#FF00FF"));
            res["AccentGreen"] = new SolidColorBrush(Color.Parse("#39FF14"));
            res["AccentAmber"] = new SolidColorBrush(Color.Parse("#FFBF00"));
            res["AccentRed"] = new SolidColorBrush(Color.Parse("#FF3366"));
            res["AccentBlue"] = new SolidColorBrush(Color.Parse("#0088FF"));

            // ── Gradient Resources (via LinearGradientBrush) ──
            res["AccentGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#00FFFF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#FF00FF"), Offset = 1 }
                }
            };
            res["ButtonGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#00CCFF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#0066FF"), Offset = 1 }
                }
            };
            res["ButtonGradientHover"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#00FFFF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#0088FF"), Offset = 1 }
                }
            };
            res["ButtonDangerGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#FF3366"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#CC0044"), Offset = 1 }
                }
            };
            res["PanelGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#0D0D14"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#14141F"), Offset = 1 }
                }
            };

            // ── Shadows & Glows ──
            res["ShadowSmColor"] = new SolidColorBrush(Color.Parse("#00FFFF1A"));
            res["ShadowMdColor"] = new SolidColorBrush(Color.Parse("#00FFFF14"));
            res["ShadowGlowColor"] = new SolidColorBrush(Color.Parse("#00FFFF33"));

            // ── Misc ──
            res["DividerColor"] = new SolidColorBrush(Color.Parse("#1A1A2E"));
            res["DividerColorLight"] = new SolidColorBrush(Color.Parse("#22223A"));
            res["SkeletonColor"] = new SolidColorBrush(Color.Parse("#181828"));
            res["ButtonSecondaryBg"] = new SolidColorBrush(Color.Parse("#14141F"));
            res["ErrorBg"] = new SolidColorBrush(Color.Parse("#1A0A0A"));
            res["ErrorText"] = new SolidColorBrush(Color.Parse("#FF3366"));
            res["SuccessBg"] = new SolidColorBrush(Color.Parse("#0A1A0A"));
            res["SuccessText"] = new SolidColorBrush(Color.Parse("#39FF14"));
            res["WarningBg"] = new SolidColorBrush(Color.Parse("#1A1400"));
            res["WarningText"] = new SolidColorBrush(Color.Parse("#FFBF00"));
            res["OverlayBg"] = new SolidColorBrush(Color.Parse("#000000CC"));
            res["GlassBg"] = new SolidColorBrush(Color.Parse("#0D0D14CC"));
            res["TableRowHover"] = new SolidColorBrush(Color.Parse("#14141F"));
            res["TableRowSelected"] = new SolidColorBrush(Color.Parse("#00FFFF0D"));
            res["TableHeaderBg"] = new SolidColorBrush(Color.Parse("#0F0F1A"));
            res["ActiveIndicator"] = new SolidColorBrush(Color.Parse("#00FFFF"));
            res["AccentLine"] = new SolidColorBrush(Color.Parse("#00FFFF33"));
        }
        else
        {
            // ── Light theme (techno-light, high contrast for accessibility) ──
            res["BgPrimary"] = new SolidColorBrush(Color.Parse("#F0F2F5"));
            res["BgSecondary"] = new SolidColorBrush(Color.Parse("#FFFFFF"));
            res["BgTertiary"] = new SolidColorBrush(Color.Parse("#E8EAEF"));
            res["BgElevated"] = new SolidColorBrush(Color.Parse("#FFFFFF"));

            res["CardBg"] = new SolidColorBrush(Color.Parse("#FFFFFF"));
            res["SidebarBg"] = new SolidColorBrush(Color.Parse("#1A1A2E"));
            res["SidebarHover"] = new SolidColorBrush(Color.Parse("#2A2A42"));
            res["SidebarSelected"] = new SolidColorBrush(Color.Parse("#00FFFF0D"));
            res["SidebarText"] = new SolidColorBrush(Color.Parse("#8A8AA0"));
            res["SidebarTextSelected"] = new SolidColorBrush(Color.Parse("#FFFFFF"));
            res["WorkspaceBg"] = new SolidColorBrush(Color.Parse("#F0F2F5"));
            res["HeaderBg"] = new SolidColorBrush(Color.Parse("#FFFFFF"));
            res["PageHeaderBg"] = new SolidColorBrush(Color.Parse("#FFFFFF"));

            res["BorderPrimary"] = new SolidColorBrush(Color.Parse("#D0D5DD"));
            res["BorderSecondary"] = new SolidColorBrush(Color.Parse("#E4E8EE"));
            res["BorderAccent"] = new SolidColorBrush(Color.Parse("#0088FF33"));

            res["TextPrimary"] = new SolidColorBrush(Color.Parse("#1A1A2E"));
            res["TextSecondary"] = new SolidColorBrush(Color.Parse("#555566"));
            res["TextTertiary"] = new SolidColorBrush(Color.Parse("#888898"));
            res["TextMuted"] = new SolidColorBrush(Color.Parse("#9999AA"));
            res["TextHint"] = new SolidColorBrush(Color.Parse("#AAAABB"));
            res["TextInverse"] = new SolidColorBrush(Color.Parse("#FFFFFF"));

            res["FieldBg"] = new SolidColorBrush(Color.Parse("#F5F6F8"));
            res["FieldBgFocus"] = new SolidColorBrush(Color.Parse("#FFFFFF"));
            res["FieldBorder"] = new SolidColorBrush(Color.Parse("#D0D5DD"));
            res["FieldBorderFocus"] = new SolidColorBrush(Color.Parse("#0088FF"));

            res["AccentCyan"] = new SolidColorBrush(Color.Parse("#0088FF"));
            res["AccentMagenta"] = new SolidColorBrush(Color.Parse("#CC00CC"));
            res["AccentGreen"] = new SolidColorBrush(Color.Parse("#00AA00"));
            res["AccentAmber"] = new SolidColorBrush(Color.Parse("#CC8800"));
            res["AccentRed"] = new SolidColorBrush(Color.Parse("#DD2244"));
            res["AccentBlue"] = new SolidColorBrush(Color.Parse("#0066DD"));

            res["AccentGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#0088FF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#CC00CC"), Offset = 1 }
                }
            };
            res["ButtonGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#0088FF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#0066DD"), Offset = 1 }
                }
            };
            res["ButtonGradientHover"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#00AAFF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#0077EE"), Offset = 1 }
                }
            };
            res["ButtonDangerGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#DD2244"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#AA0033"), Offset = 1 }
                }
            };
            res["PanelGradient"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.Parse("#FFFFFF"), Offset = 0 },
                    new GradientStop { Color = Color.Parse("#F5F6F8"), Offset = 1 }
                }
            };

            res["ShadowSmColor"] = new SolidColorBrush(Color.Parse("#0000001A"));
            res["ShadowMdColor"] = new SolidColorBrush(Color.Parse("#00000014"));
            res["ShadowGlowColor"] = new SolidColorBrush(Color.Parse("#0088FF33"));

            res["DividerColor"] = new SolidColorBrush(Color.Parse("#E4E8EE"));
            res["DividerColorLight"] = new SolidColorBrush(Color.Parse("#EEF0F4"));
            res["SkeletonColor"] = new SolidColorBrush(Color.Parse("#E4E8EE"));
            res["ButtonSecondaryBg"] = new SolidColorBrush(Color.Parse("#F0F2F5"));
            res["ErrorBg"] = new SolidColorBrush(Color.Parse("#FFF0F0"));
            res["ErrorText"] = new SolidColorBrush(Color.Parse("#DD2244"));
            res["SuccessBg"] = new SolidColorBrush(Color.Parse("#F0FFF0"));
            res["SuccessText"] = new SolidColorBrush(Color.Parse("#00AA00"));
            res["WarningBg"] = new SolidColorBrush(Color.Parse("#FFFBEA"));
            res["WarningText"] = new SolidColorBrush(Color.Parse("#CC8800"));
            res["OverlayBg"] = new SolidColorBrush(Color.Parse("#00000066"));
            res["GlassBg"] = new SolidColorBrush(Color.Parse("#FFFFFFCC"));
            res["TableRowHover"] = new SolidColorBrush(Color.Parse("#F5F6F8"));
            res["TableRowSelected"] = new SolidColorBrush(Color.Parse("#0088FF0D"));
            res["TableHeaderBg"] = new SolidColorBrush(Color.Parse("#F0F2F5"));
            res["ActiveIndicator"] = new SolidColorBrush(Color.Parse("#0088FF"));
            res["AccentLine"] = new SolidColorBrush(Color.Parse("#0088FF33"));
        }
    }
}
