namespace ZeeDash.API.Grains.Services;

using System.Threading.Tasks;
using NRedisGraph;
using NUlid;
using StackExchange.Redis;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Grains.Extensions;

public interface IAccessControlService {

    Task<List<AccessControlMember>> GetMembersAsync(MembershipId id);

    Task AddMembershipAsync(MembershipId id, UserId userId, AccessLevel level);

    Task AddMembershipAsync(MembershipId id, GroupId groupId, AccessLevel level);

    Task RemoveMembershipAsync(MembershipId id, UserId userId);

    Task RemoveMembershipAsync(MembershipId id, GroupId groupId);

    Task CreateTenantAsync(TenantId id);

    Task CreateBusinessUnitAsync(BusinessUnitId id);

    Task CreateDashboardAsync(DashboardId id);
}

public class AccessControlService
    : Disposable<AccessControlService>
    , IAccessControlService {
    private RedisGraph? graph;

    public AccessControlService(IConnectionMultiplexer mux) {
        this.graph = new RedisGraph(mux.GetDatabase(0));
    }

    protected override void OnDisposing() {
        this.graph = null;
    }

    private static readonly string GetMembersQueryTemplate =
        "MATCH (i:@@ITEM_TYPE@@ { id: $id })" + Environment.NewLine +
        "MATCH (i)-[:BELONGS_TO*0..]->()<-[r:IS_MEMBER_OF]-(m:Member)" + Environment.NewLine +
        "RETURN m.id, m.type, r.level, r.relatedTo" + Environment.NewLine +
        "ORDER BY m.id";

    async Task<List<AccessControlMember>> IAccessControlService.GetMembersAsync(MembershipId id) {
        this.EnsureIsNotDisposed();

        var graphDd = $"iam-{id.TenantId.AsUlid()}";
        var relatedTo = id.IsRelatedTo;
        var query = GetMembersQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> { { "id", id.ParentValue } };
        var result = await this.graph!.GraphReadOnlyQueryAsync(graphDd, query, parameters);
        return result.Select(r => new AccessControlMember {
            MemberId = r.GetString("m.id"),
            Level = r.GetString("r.level").AsAccessLevel(),
            Kind = r.GetString("r.relatedTo") == relatedTo ? AccessLevelKind.Direct : AccessLevelKind.Inherited
        }).ToList();
    }

    private static readonly string DoesMemberExistsQueryTemplate =
        "MATCH (m:Member { id: $id })" + Environment.NewLine +
        "RETURN m.id";

    private async Task<bool> DoesMemberExistsAsync(MembershipId id, string memberIdValue) {
        this.EnsureIsNotDisposed();

        var graphDd = $"iam-{id.TenantId.AsUlid()}";
        var relatedTo = id.IsRelatedTo;
        var query = DoesMemberExistsQueryTemplate;

        var parameters = new Dictionary<string, object> { { "id", memberIdValue } };
        var result = await this.graph!.GraphReadOnlyQueryAsync(graphDd, query, parameters);
        return result.Count > 0;
    }

    private static readonly string AddMembershipQueryTemplate =
        "MATCH (i:@@ITEM_TYPE@@ { id: $itemId })" + Environment.NewLine +
        "MATCH (m:Member { id: $memberId })" + Environment.NewLine +
        "CREATE (m)-[r:IS_MEMBER_OF { level: $level, relatedTo: $relatedTo}]->(i)";

    async Task IAccessControlService.AddMembershipAsync(MembershipId id, UserId userId, AccessLevel level) {
        this.EnsureIsNotDisposed();

        await this.AddMembershipAsync(id, userId.Value, nameof(User), level);
    }

    async Task IAccessControlService.AddMembershipAsync(MembershipId id, GroupId groupId, AccessLevel level) {
        this.EnsureIsNotDisposed();

        await this.AddMembershipAsync(id, groupId.Value, nameof(Group), level);
    }

    private async Task AddMembershipAsync(MembershipId id, string memberIdValue, string memberType, AccessLevel level) {
        this.EnsureIsNotDisposed();

        var exists = await this.DoesMemberExistsAsync(id, memberIdValue);
        if (!exists) {
            await this.CreateMemberAsync(id.TenantId.AsUlid(), memberIdValue, nameof(User));
        }

        var graphDd = $"iam-{id.TenantId.AsUlid()}";
        var relatedTo = id.IsRelatedTo;
        var query = AddMembershipQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "itemId", id.ParentValue },
            { "memberId", memberIdValue },
            { "memberType", memberType },
            { "level", level.ToString() },
            { "relatedTo", relatedTo}
        };
        var result = await this.graph!.GraphQueryAsync(graphDd, query, parameters);

        if (result.Statistics.RelationshipsCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    private static readonly string DeleteMembershipQueryTemplate =
        "MATCH (s:@@ITEM_TYPE@@ { id: $id })" + Environment.NewLine +
        "DELETE s";

    async Task IAccessControlService.RemoveMembershipAsync(MembershipId id, UserId userId) {
        this.EnsureIsNotDisposed();

        await this.RemoveMembershipAsync(id, userId.Value);
    }

    async Task IAccessControlService.RemoveMembershipAsync(MembershipId id, GroupId groupId) {
        this.EnsureIsNotDisposed();

        await this.RemoveMembershipAsync(id, groupId.Value);
    }

    private async Task RemoveMembershipAsync(MembershipId id, string idValue) {
        var graphDd = $"iam-{id.TenantId.AsUlid()}";
        var relatedTo = id.IsRelatedTo;
        var query = DeleteMembershipQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "id", idValue },
        };
        var result = await this.graph!.GraphQueryAsync(graphDd, query, parameters);

        if (result.Statistics.NodesDeleted != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    async Task IAccessControlService.CreateTenantAsync(TenantId id) {
        this.EnsureIsNotDisposed();

        await this.CreateAsync(id.AsUlid(), nameof(Tenant), id.Value);
    }

    async Task IAccessControlService.CreateBusinessUnitAsync(BusinessUnitId id) {
        this.EnsureIsNotDisposed();

        await this.CreateAsync(id.TenantId.AsUlid(), nameof(BusinessUnit), id.Value);
        await this.AddBelongshipAsync(id.TenantId.AsUlid(), nameof(Tenant), id.TenantId.Value, nameof(BusinessUnit), id.Value);
    }

    async Task IAccessControlService.CreateDashboardAsync(DashboardId id) {
        this.EnsureIsNotDisposed();

        await this.CreateAsync(id.TenantId.AsUlid(), nameof(Dashboard), id.Value);
        if (id.BusinessUnitId.IsEmpty) {
            await this.AddBelongshipAsync(id.TenantId.AsUlid(), nameof(Tenant), id.TenantId.Value, nameof(Dashboard), id.Value);
        } else {
            await this.AddBelongshipAsync(id.TenantId.AsUlid(), nameof(BusinessUnit), id.BusinessUnitId.Value, nameof(Dashboard), id.Value);
        }
    }

    private const string CreateQueryTemplate =
        "CREATE (i:@@ITEM_TYPE@@ { id: $id })";

    private async Task CreateAsync(Ulid tenantId, string relatedTo, string itemId) {
        var graphDd = $"iam-{tenantId}";
        var query = CreateQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "id", itemId },
        };
        var result = await this.graph!.GraphQueryAsync(graphDd, query, parameters);

        if (result.Statistics.NodesCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    private const string CreateMemberQueryTemplate =
        "CREATE (m:Member { id: $id, type: $memberType })";

    private async Task CreateMemberAsync(Ulid tenantId, string memberId, string memberType) {
        var graphDd = $"iam-{tenantId}";
        var query = CreateMemberQueryTemplate;

        var parameters = new Dictionary<string, object> {
            { "id", memberId },
            { "memberType", memberType },
        };
        var result = await this.graph!.GraphQueryAsync(graphDd, query, parameters);

        if (result.Statistics.NodesCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    private static readonly string AddBelongshipQueryTemplate =
        "MATCH (s:@@SOURCE_TYPE@@ { id: $sourceId })" + Environment.NewLine +
        "MATCH (t:@@TARGET_TYPE@@ { id: $targetId })" + Environment.NewLine +
        "CREATE (s)-[:BELONGS_TO]->(b)";

    private async Task AddBelongshipAsync(Ulid tenantId, string relatedToSource, string sourceId, string relatedToTarget, string targetId) {
        var graphDd = $"iam-{tenantId}";
        var query = AddBelongshipQueryTemplate
            .Replace("@@SOURCE_TYPE@@", relatedToSource, StringComparison.Ordinal)
            .Replace("@@TARGET_TYPE@@", relatedToTarget, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "sourceId", sourceId },
            { "targetId", targetId },
        };
        var result = await this.graph!.GraphQueryAsync(graphDd, query, parameters);

        if (result.Statistics.RelationshipsCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }
}
