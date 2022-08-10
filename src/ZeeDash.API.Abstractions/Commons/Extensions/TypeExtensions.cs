namespace ZeeDash.API.Abstractions.Commons.Extensions;

public static class TypeExtensions {
    /// <summary>
    /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
    /// any type parameters).
    /// </summary>
    /// <param name="baseType">The base type class for which the check is made.</param>
    /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType) {
        while (toCheck != typeof(object)) {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (baseType == cur) {
                return true;
            }

#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            toCheck = toCheck.BaseType;
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            if (toCheck == null) {
                break;
            }
        }

        return false;
    }

    public static Type? GetSubclassOfRawGeneric(this Type toCheck, Type baseType) {
        while (toCheck != typeof(object)) {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (baseType == cur) {
                return toCheck;
            }

#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            toCheck = toCheck.BaseType;
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            if (toCheck == null) {
                break;
            }
        }

        return null;
    }
}
