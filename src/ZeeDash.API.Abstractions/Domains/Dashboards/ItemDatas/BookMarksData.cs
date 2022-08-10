namespace ZeeDash.API.Abstractions.Domains.Dashboards.ItemDatas;

using System.Collections.Generic;

public class BookMarksData
    : ItemData {
    private const string UrlPropertyName = "url";
    private const string IconPropertyName = "icon";
    private const string DescriptionPropertyName = "description";
    private static readonly string[] DefaultKeys = new string[3] { UrlPropertyName, IconPropertyName, DescriptionPropertyName };

    public BookMarksData() : base(DefaultKeys) {
    }

    public BookMarksData(Dictionary<string, object?> values) : base(values, DefaultKeys) {
    }

    public string Url {
        get => this.Get(UrlPropertyName) as string ?? string.Empty;
        set => this.Set(UrlPropertyName, value);
    }

    public string Icon {
        get => this.Get(IconPropertyName) as string ?? string.Empty;
        set => this.Set(IconPropertyName, value);
    }

    public string Description {
        get => this.Get(DescriptionPropertyName) as string ?? string.Empty;
        set => this.Set(DescriptionPropertyName, value);
    }
}
