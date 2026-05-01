namespace UltrasoundProtocol.Application.Services.Templates;

public interface ITemplateService
{
    IEnumerable<ProtocolTemplate> GetAll();
    ProtocolTemplate? GetByKey(string key);
}

public class ProtocolTemplate
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public string Findings { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
}

public class TemplateService : ITemplateService
{
    private static readonly List<ProtocolTemplate> Templates =
    [
        new()
        {
            Key = "liver_normal",
            Name = "Jigar — Norma",
            BodyPart = "Jigar",
            Findings = "Jigar o'lchamlari normal chegarada. O'ng bo'lak — 14.5 sm, chap bo'lak — 6.8 sm. " +
                        "Konturlari tekis, parenximasi bir xil, exogenizligi normal. " +
                        "Intragepatik o't yo'llari kengaymagan. Portal vena — 11 mm.",
            Conclusion = "Jigar patologiyasi aniqlanmadi. Norma."
        },
        new()
        {
            Key = "liver_fatty",
            Name = "Jigar — Yog'li gepatoz",
            BodyPart = "Jigar",
            Findings = "Jigar o'lchamlari kattalashgan. O'ng bo'lak — 17.2 sm. " +
                        "Parenxima exogenizligi diffuz oshgan, tomirlar ko'rinishi yomonlashgan. " +
                        "Jigar konturlari tekis, lekin tuzilishi heterogen.",
            Conclusion = "Diffuz o'zgarishlar — yog'li gepatoz belgisi. Gastroenterolog konsultatsiyasi tavsiya etiladi."
        },
        new()
        {
            Key = "kidney_normal",
            Name = "Buyrak — Norma",
            BodyPart = "Buyrak",
            Findings = "O'ng buyrak — 10.5 x 4.8 sm, chap buyrak — 10.8 x 5.0 sm. " +
                        "Konturlari tekis, parenxima qalinligi 1.5-1.8 sm, exogenizligi normal. " +
                        "CLS kengaymagan. Konkrement aniqlanmadi.",
            Conclusion = "Buyraklar patologiyasi aniqlanmadi. Norma."
        },
        new()
        {
            Key = "kidney_stone",
            Name = "Buyrak — Tosh (Nefrolitiaz)",
            BodyPart = "Buyrak",
            Findings = "O'ng buyrak CLS sohasida giperexogen tuzilma — 8 mm, akustik soya beradi. " +
                        "CLS 1-darajali kengayish. Parenxima qalinligi saqlangan.",
            Conclusion = "O'ng buyrak nefrolitiazi. Urolog konsultatsiyasi tavsiya etiladi."
        },
        new()
        {
            Key = "gallbladder_normal",
            Name = "O't pufagi — Norma",
            BodyPart = "O't pufagi",
            Findings = "O't pufagi o'lchamlari — 7.5 x 2.8 sm. Devori qalinligi — 2 mm. " +
                        "Ichida konkrement aniqlanmadi. Choledox — 5 mm.",
            Conclusion = "O't pufagi patologiyasi aniqlanmadi. Norma."
        },
        new()
        {
            Key = "gallbladder_stone",
            Name = "O't pufagi — Tosh (Xolelityaz)",
            BodyPart = "O't pufagi",
            Findings = "O't pufagi devori qalinlashgan — 4 mm. Ichida 12 mm va 8 mm giperexogen " +
                        "tuzilmalar, akustik soya beradi. Choledox — 6 mm.",
            Conclusion = "O't tosh kasalligi (Xolelityaz). Jarrohlik konsultatsiyasi tavsiya etiladi."
        },
        new()
        {
            Key = "thyroid_normal",
            Name = "Qalqonsimon bez — Norma",
            BodyPart = "Qalqonsimon bez",
            Findings = "Qalqonsimon bez o'lchamlari: o'ng bo'lak — 1.8 x 1.6 x 4.5 sm, " +
                        "chap bo'lak — 1.7 x 1.5 x 4.3 sm. Isthmus — 3 mm. " +
                        "Parenxima bir xil, exogenizligi normal. Tugunlar aniqlanmadi.",
            Conclusion = "Qalqonsimon bez patologiyasi aniqlanmadi. Norma."
        },
        new()
        {
            Key = "heart_normal",
            Name = "Yurak — Norma (EchoCG)",
            BodyPart = "Yurak",
            Findings = "Chap qorincha: EDR — 4.9 sm, ESR — 3.2 sm. EF — 62%. " +
                        "Klapanlar tuzilishi va funksiyasi saqlangan. " +
                        "Perikard bo'shlig'ida suyuqlik yo'q.",
            Conclusion = "EchoCG ko'rsatkichlari norma chegarasida."
        },
        new()
        {
            Key = "pregnancy_first",
            Name = "Homiladorlik — 1-trimester",
            BodyPart = "Homiladorlik",
            Findings = "Bachadon bo'shlig'ida bitta homila aniqlanadi. CRL — __ mm, " +
                        "gestatsion yosh — __ hafta __ kun. Yurak urishi — __ marta/min. " +
                        "Xorion — ____ devorida. Sariq tana — __ mm.",
            Conclusion = "__ haftalik homiladorlik. Rivojlanish gestatsion yoshga mos."
        },
        new()
        {
            Key = "bladder_normal",
            Name = "Siydik pufagi — Norma",
            BodyPart = "Siydik pufagi",
            Findings = "Siydik pufagi to'la holatda tekshirildi. Devori tekis, qalinligi — 3 mm. " +
                        "Ichida patologik tuzilma aniqlanmadi. Qoldiq siydik — 15 ml.",
            Conclusion = "Siydik pufagi patologiyasi aniqlanmadi."
        }
    ];

    public IEnumerable<ProtocolTemplate> GetAll() => Templates;

    public ProtocolTemplate? GetByKey(string key) =>
        Templates.FirstOrDefault(t => t.Key == key);
}
