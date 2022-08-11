namespace ZeeDash.API.Abstractions.Domains.Tenants;

using Orleans;
using ZeeDash.API.Abstractions.Grains.Common;

public interface ITenantMembershipViewGrain : IGrainWithStringKey, IMembershipView { }

public interface IBusinessUnitMembershipViewGrain : IGrainWithStringKey, IMembershipView { }

public interface IDashboardMembershipViewGrain : IGrainWithStringKey, IMembershipView { }
