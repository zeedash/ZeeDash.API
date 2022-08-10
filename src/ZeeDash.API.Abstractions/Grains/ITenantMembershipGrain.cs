namespace ZeeDash.API.Abstractions.Domains.Tenants;

using Orleans;
using ZeeDash.API.Abstractions.Grains;

public interface ITenantMembershipGrain : IGrainWithStringKey, IMembershipView { }

public interface IBusinessUnitMembershipGrain : IGrainWithStringKey, IMembershipView { }

public interface IDashboardMembershipGrain : IGrainWithStringKey, IMembershipView { }
