namespace ZeeDash.API.Grains.Extensions;

using ZeeDash.API.Abstractions.Domains.IAM;

public static class EnumExtensions {

    public static AccessLevel AsAccessLevel(this string value) {
        if (Enum.TryParse(value, out AccessLevel accessLevel)) {
            return accessLevel;
        }
        return AccessLevel.None;
    }
}
