namespace ZeeDash.API.Abstractions.Domains.Dashboards.ItemDatas;

public static class ItemDataExtensions {

    public static BookMarksData AsBookmarksData(this ItemData itemData) {
        return new BookMarksData(itemData);
    }
}
