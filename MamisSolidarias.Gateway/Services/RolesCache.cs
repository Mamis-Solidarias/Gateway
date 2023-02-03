using StackExchange.Redis;

namespace MamisSolidarias.Gateway.Services;

internal class RolesCache : IRolesCache
{
	private readonly ConnectionMultiplexer _redis;
	public RolesCache(ConnectionMultiplexer redis)
	{
		_redis = redis;
	}
	
	public async Task<IEnumerable<string>> GetRoles(int userId)
	{
		var db = _redis.GetDatabase();
		return (await db.ListRangeAsync($"roles-{userId}")).Select(t => t.ToString());
	}
	public Task ClearRoles(int userId)
	{
		var db = _redis.GetDatabase();
		return db.KeyDeleteAsync($"roles-{userId}");
	}
}

internal interface IRolesCache
{
	Task<IEnumerable<string>> GetRoles(int userId);
	Task ClearRoles(int userId);
}