namespace Application.Library.Reports.Shared;






public static class ReportColumnTitles
{
    
    private static readonly Dictionary<string, string> Titles = new(StringComparer.OrdinalIgnoreCase)
    {
        
        ["No"] = "شماره",
        ["Name"] = "عنوان",
        ["Author"] = "نویسنده",
        ["Editor"] = "سردبیر",
        ["Translator"] = "مترجم",
        ["Corrector"] = "مصحح",
        ["PublisherName"] = "ناشر",
        ["PublishPlace"] = "محل نشر",
        ["PublishPlaceName"] = "محل نشر",
        ["PublishYear"] = "سال چاپ",
        ["PublishDate"] = "تاریخ نشر",
        ["PublishOrder"] = "نوبت چاپ",
        ["Language"] = "زبان",
        ["Subject"] = "موضوع",
        ["Year"] = "سال",
        ["VolumeCount"] = "تعداد نسخه",
        ["Isbn"] = "شماره شابک",
        ["BookShelfRow"] = "ردیف قفسه",
        ["PageCount"] = "تعداد صفحه",
        ["Size"] = "اندازه",
        ["TypeName"] = "نوع",
        ["Period"] = "دوره",
        ["JournalNo"] = "شماره مجله",

        
        ["VersionNo"] = "شماره نسخه خطی",
        ["Gap"] = "فاصله",
        ["WritingPlace"] = "محل نگارش",

        
        ["Concessionaire"] = "صاحب امتیاز",
        ["ResponsibleDirector"] = "مدیر مسئول",

        
        ["Text"] = "مقدار",
        ["Value"] = "تعداد",
    };

    
    public static string Resolve(string propertyName) =>
        Titles.TryGetValue(propertyName, out string? title) ? title : propertyName;
}