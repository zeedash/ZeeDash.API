namespace ZeeDash.API.Abstractions.Commons.Identities;

using ZeeDash.API.Abstractions.Commons.ValueObjects;

public interface IIdentity
    : ISingleValueObject<string> {
    new string Value { get; set; }
}
