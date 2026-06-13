namespace MediFlow.App.Helpers;

public static class Theme
{
    public static Color Sidebar        = ColorTranslator.FromHtml("#0F2044");
    public static Color SidebarHover   = ColorTranslator.FromHtml("#1A3260");
    public static Color SidebarActive  = ColorTranslator.FromHtml("#2563EB");
    public static Color Primary        = ColorTranslator.FromHtml("#2563EB");
    public static Color PrimaryHover   = ColorTranslator.FromHtml("#1D4ED8");
    public static Color Success        = ColorTranslator.FromHtml("#16A34A");
    public static Color Danger         = ColorTranslator.FromHtml("#DC2626");
    public static Color Warning        = ColorTranslator.FromHtml("#D97706");
    public static Color Background     = ColorTranslator.FromHtml("#F1F5F9");
    public static Color Surface        = Color.White;
    public static Color Border         = ColorTranslator.FromHtml("#E2E8F0");
    public static Color TextPrimary    = ColorTranslator.FromHtml("#0F172A");
    public static Color TextSecondary  = ColorTranslator.FromHtml("#64748B");
    public static Color TextLight      = Color.White;
    public static Color GridHeader     = ColorTranslator.FromHtml("#1E3A5F");
    public static Color GridAltRow     = ColorTranslator.FromHtml("#F8FAFC");

    public static Font FontRegular  = new Font("Segoe UI", 9.5f,  FontStyle.Regular);
    public static Font FontMedium   = new Font("Segoe UI", 9.5f,  FontStyle.Bold);
    public static Font FontTitle    = new Font("Segoe UI", 14f,   FontStyle.Bold);
    public static Font FontSubtitle = new Font("Segoe UI", 11f,   FontStyle.Regular);
    public static Font FontSmall    = new Font("Segoe UI", 8.5f,  FontStyle.Regular);
    public static Font FontSidebar  = new Font("Segoe UI", 9.5f,  FontStyle.Regular);
    public static Font FontLogo     = new Font("Segoe UI", 15f,   FontStyle.Bold);
}
