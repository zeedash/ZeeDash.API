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

    Task CreateMemberAsync(UserId userId);

    Task CreateMemberAsync(GroupId groupId);

    Task AddBelongshipAsync(TenantId tenantId, GroupId groupId);

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
    private const string GraphDBName = "iam";
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

        var relatedTo = id.IsRelatedTo;
        var query = GetMembersQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> { { "id", id.ParentValue } };
        var result = await this.graph!.GraphReadOnlyQueryAsync(GraphDBName, query, parameters);
        return result.Select(r => new AccessControlMember {
            MemberId = r.GetString("m.id"),
            Level = r.GetString("r.level").AsAccessLevel(),
            Kind = r.GetString("r.relatedTo") == relatedTo ? AccessLevelKind.Direct : AccessLevelKind.Inherited
        }).ToList();
    }

    private static readonly string DoesMemberExistsQueryTemplate =
        "MATCH (m:Member { id: $id })" + Environment.NewLine +
        "RETURN m.id";

    private async Task<bool> DoesMemberExistsAsync(string memberIdValue) {
        this.EnsureIsNotDisposed();

        var query = DoesMemberExistsQueryTemplate;

        var parameters = new Dictionary<string, object> { { "id", memberIdValue } };
        var result = await this.graph!.GraphReadOnlyQueryAsync(GraphDBName, query, parameters);
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
        var relatedTo = id.IsRelatedTo;
        // Note : Subtile hack there...
        if (relatedTo == nameof(Group)) {
            relatedTo = nameof(Member);
        }
        var query = AddMembershipQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "itemId", id.ParentValue },
            { "memberId", memberIdValue },
            { "memberType", memberType },
            { "level", level.ToString() },
            { "relatedTo", relatedTo}
        };
        var result = await this.graph!.GraphQueryAsync(GraphDBName, query, parameters);

        if (result.Statistics.RelationshipsCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    private static readonly string DeleteMembershipQueryTemplate =
        "MATCH (s:@@SOURCE_TYPE@@ { id: $sourceId })" + Environment.NewLine +
        "MATCH (t:@@TARGET_TYPE@@ { id: $targetId })" + Environment.NewLine +
        "MATCH (m)-[r:IS_MEMBER_OF]->(i)" + Environment.NewLine +
        "DELETE r";

    async Task IAccessControlService.RemoveMembershipAsync(MembershipId id, UserId userId) {
        this.EnsureIsNotDisposed();

        await this.RemoveMembershipAsync(id, nameof(User), userId.Value);
    }

    async Task IAccessControlService.RemoveMembershipAsync(MembershipId id, GroupId groupId) {
        this.EnsureIsNotDisposed();

        await this.RemoveMembershipAsync(id, nameof(Group), groupId.Value);
    }

    private async Task RemoveMembershipAsync(MembershipId id, string relatedToTarget, string targetId) {
        var relatedTo = id.IsRelatedTo;
        var query = DeleteMembershipQueryTemplate
            .Replace("@@SOURCE_TYPE@@", relatedTo, StringComparison.Ordinal)
            .Replace("@@TARGET_TYPE@@", relatedToTarget, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "sourceId", id.Value },
            { "targetId", targetId },
        };
        var result = await this.graph!.GraphQueryAsync(GraphDBName, query, parameters);

        if (result.Statistics.RelationshipsDeleted != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    async Task IAccessControlService.CreateTenantAsync(TenantId id) {
        this.EnsureIsNotDisposed();

        await this.CreateAsync(nameof(Tenant), id.Value);
    }

    async Task IAccessControlService.CreateBusinessUnitAsync(BusinessUnitId id) {
        this.EnsureIsNotDisposed();

        await this.CreateAsync(nameof(BusinessUnit), id.Value);
        await this.AddBelongshipAsync(nameof(Tenant), id.TenantId.Value, nameof(BusinessUnit), id.Value);
    }

    async Task IAccessControlService.CreateDashboardAsync(DashboardId id) {
        this.EnsureIsNotDisposed();

        await this.CreateAsync(nameof(Dashboard), id.Value);
        if (id.BusinessUnitId.IsEmpty) {
            await this.AddBelongshipAsync(nameof(Tenant), id.TenantId.Value, nameof(Dashboard), id.Value);
        } else {
            await this.AddBelongshipAsync(nameof(BusinessUnit), id.BusinessUnitId.Value, nameof(Dashboard), id.Value);
        }
    }

    private const string CreateQueryTemplate =
        "CREATE (i:@@ITEM_TYPE@@ { id: $id, type: $memberType })";

    private async Task CreateAsync(string relatedTo, string itemId) {
        var query = CreateQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "id", itemId },
            { "memberType", relatedTo },
        };
        var result = await this.graph!.GraphQueryAsync(GraphDBName, query, parameters);

        if (result.Statistics.NodesCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    private const string CreateMemberQueryTemplate =
        "CREATE (m:Member { id: $id, type: $memberType })";

    private async Task CreateMemberAsync(string memberId, string memberType) {
        var query = CreateMemberQueryTemplate;

        var parameters = new Dictionary<string, object> {
            { "id", memberId },
            { "memberType", memberType },
        };
        var result = await this.graph!.GraphQueryAsync(GraphDBName, query, parameters);

        if (result.Statistics.NodesCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    private static readonly string AddBelongshipQueryTemplate =
        "MATCH (s:@@SOURCE_TYPE@@ { id: $sourceId })" + Environment.NewLine +
        "MATCH (t:@@TARGET_TYPE@@ { id: $targetId })" + Environment.NewLine +
        "CREATE (t)-[:BELONGS_TO]->(s)";

    private async Task AddBelongshipAsync(string relatedToSource, string sourceId, string relatedToTarget, string targetId) {
        var query = AddBelongshipQueryTemplate
            .Replace("@@SOURCE_TYPE@@", relatedToSource, StringComparison.Ordinal)
            .Replace("@@TARGET_TYPE@@", relatedToTarget, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "sourceId", sourceId },
            { "targetId", targetId },
        };
        var result = await this.graph!.GraphQueryAsync(GraphDBName, query, parameters);

        if (result.Statistics.RelationshipsCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    async Task IAccessControlService.CreateMemberAsync(UserId userId) {
        this.EnsureIsNotDisposed();

        await this.CreateMemberAsync(userId.Value, nameof(User));
    }

    async Task IAccessControlService.CreateMemberAsync(GroupId groupId) {
        this.EnsureIsNotDisposed();

        await this.CreateMemberAsync(groupId.Value, nameof(Group));
    }

    async Task IAccessControlService.AddBelongshipAsync(TenantId tenantId, GroupId groupId) {
        this.EnsureIsNotDisposed();

        await this.AddBelongshipAsync(nameof(Tenant), tenantId.Value, nameof(Member), groupId.Value);
    }
}
