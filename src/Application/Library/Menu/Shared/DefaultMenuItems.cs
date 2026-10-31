namespace Application.Library.Menu.Shared;





public static class DefaultMenuItems
{
    public static IReadOnlyList<DefaultMenuItem> Root { get; } = new DefaultMenuItem[]
    {
        new("داشبورد", "/", "FaHome", false, 0, Array.Empty<DefaultMenuItem>()),
        new("کتابها", null, "FaBook", false, 1, new DefaultMenuItem[]
        {
            new("مدیریت کتابها", "/books", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("جستجوی کتاب", "/books/search", null, false, 1, Array.Empty<DefaultMenuItem>()),
            new("آمار کتابها", "/books/statistics", null, false, 2, Array.Empty<DefaultMenuItem>()),
        }),
        new("نشریات", null, "FaNewspaper", false, 2, new DefaultMenuItem[]
        {
            new("مدیریت نشریات", "/publications", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("جستجوی نشریه", "/publications/search", null, false, 1, Array.Empty<DefaultMenuItem>()),
            new("آمار نشریات", "/publications/statistics", null, false, 2, Array.Empty<DefaultMenuItem>()),
        }),
        new("نسخ خطی", null, "FaScroll", false, 3, new DefaultMenuItem[]
        {
            new("مدیریت نسخ خطی", "/manuscripts", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("جستجوی نسخه خطی", "/manuscripts/search", null, false, 1, Array.Empty<DefaultMenuItem>()),
            new("آمار نسخ خطی", "/manuscripts/statistics", null, false, 2, Array.Empty<DefaultMenuItem>()),
        }),
        new("اطلاعات پایه", null, "FaTags", false, 4, new DefaultMenuItem[]
        {
            new("شهرها", "/cities", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("زبانها", "/languages", null, false, 1, Array.Empty<DefaultMenuItem>()),
            new("موضوعات", "/subjects", null, false, 2, Array.Empty<DefaultMenuItem>()),
            new("ناشران", "/publishers", null, false, 3, Array.Empty<DefaultMenuItem>()),
            new("فاصلهها", "/gaps", null, false, 4, Array.Empty<DefaultMenuItem>()),
            new("انواع نشریات", "/publication-types", null, false, 5, Array.Empty<DefaultMenuItem>()),
        }),
        new("کاربران", null, "FaUsers", false, 5, new DefaultMenuItem[]
        {
            new("مدیریت کاربران", "/users", null, false, 0, Array.Empty<DefaultMenuItem>()),
        }),
        new("پروفایل", "/profile", "FaUser", false, 6, Array.Empty<DefaultMenuItem>()),
        new("گزارشها", null, "FaFileAlt", false, 7, new DefaultMenuItem[]
        {
            new("جستجوی کتاب", "/books/search", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("جستجوی نسخه خطی", "/manuscripts/search", null, false, 1, Array.Empty<DefaultMenuItem>()),
            new("جستجوی نشریه", "/publications/search", null, false, 2, Array.Empty<DefaultMenuItem>()),
            new("گزارشات آماری کتاب", "/books/statistics", null, false, 3, Array.Empty<DefaultMenuItem>()),
            new("گزارشات آماری نسخ خطی", "/manuscripts/statistics", null, false, 4, Array.Empty<DefaultMenuItem>()),
            new("گزارشات آماری نشریه", "/publications/statistics", null, false, 5, Array.Empty<DefaultMenuItem>()),
        }),
        new("تنظیمات", null, "FaCog", false, 8, new DefaultMenuItem[]
        {
            new("تنظیمات عمومی", "/settings", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("تنظیمات منو", "/menu-settings", null, true, 1, Array.Empty<DefaultMenuItem>()),
        }),
        new("مدیریت ادمین", null, "FaShieldAlt", false, 9, new DefaultMenuItem[]
        {
            new("مدیریت نقشها", "/admin/roles", null, false, 0, Array.Empty<DefaultMenuItem>()),
            new("نقشدهی به کاربران", "/admin/user-roles", null, false, 1, Array.Empty<DefaultMenuItem>()),
        }),
    };
}