namespace ZeeDash.API.Grains.Services;

using System.Threading.Tasks;
using NRedisGraph;
using StackExchange.Redis;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Grains.Extensions;

public interface IAccessControlService {

    Task<List<AccessControlMember>> GetMembersAsync(MembershipId id);

    Task AddMembershipAsync(MembershipId id, UserId userId, AccessLevel level);

    Task RemoveMembershipAsync(MembershipId id, UserId userId);

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

    private const string GetMembersQueryTemplate =
        "MATCH (i:@@ITEM_TYPE@@ { id: $id })" +
        "MATCH (i)-[:BELONGS_TO*0..]->()<-[r:IS_MEMBER_OF]-(m:Member)" +
        "   WHERE r.relatedTo IS NOT NULL" +
        "RETURN m.id, r.level, r.relatedTo" +
        "ORDER BY m.id";

    async Task<List<AccessControlMember>> IAccessControlService.GetMembersAsync(MembershipId id) {
        this.EnsureIsNotDisposed();

        var graphDd = $"iam-{id.TenantId.AsUlid()}";
        var relatedTo = id.IsRelatedTo;
        var query = GetMembersQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> { { "id", id.Value } };
        var result = await this.graph!.GraphReadOnlyQueryAsync(graphDd, query, parameters);
        return result.Select(r => new AccessControlMember {
            MemberId = r.GetString("id"),
            Level = r.GetString("level").AsAccessLevel(),
            Kind = r.GetString("relatedTo") == relatedTo ? AccessLevelKind.Direct : AccessLevelKind.Inherited
        }).ToList();
    }

    private const string AddMembershipQueryTemplate =
        "MATCH (i:@@ITEM_TYPE@@ { id: $itemId })" +
        "MATCH (m:Member { id: $memberId })" +
        "CREATE (m)-[r:BELONGS_TO { type: $level, relatedTo: $relatedTo}]->(i)";

    async Task IAccessControlService.AddMembershipAsync(MembershipId id, UserId userId, AccessLevel level) {
        this.EnsureIsNotDisposed();

        var graphDd = $"iam-{id.TenantId.AsUlid()}";
        var relatedTo = id.IsRelatedTo;
        var query = AddMembershipQueryTemplate.Replace("@@ITEM_TYPE@@", relatedTo, StringComparison.Ordinal);

        var parameters = new Dictionary<string, object> {
            { "itemId", id.ParentValue },
            { "memberId", userId.Value },
            { "level", level.ToString() },
            { "relatedTo", relatedTo}
        };
        var result = await this.graph!.GraphReadOnlyQueryAsync(graphDd, query, parameters);

        if (result.Statistics.RelationshipsCreated != 1) {
            // TODO : Add Signal to mark incoherence on graph databse
        }
    }

    Task IAccessControlService.RemoveMembershipAsync(MembershipId id, UserId userId) {
        return Task.CompletedTask;
    }

    private const string AddBelongshipQueryTemplate =
        "MATCH (s:@@SOURCE_TYPE@@ { id: $sourceId })" +
        "MATCH (t:@@TARGET_TYPE@@ { id: $targetId })" +
        "CREATE (s)-[:BELONGS_TO]->(b)";

    Task IAccessControlService.CreateTenantAsync(TenantId id) {
        throw new NotImplementedException();
    }

    Task IAccessControlService.CreateBusinessUnitAsync(BusinessUnitId id) {
        throw new NotImplementedException();
    }

    Task IAccessControlService.CreateDashboardAsync(DashboardId id) {
        throw new NotImplementedException();
    }
}
