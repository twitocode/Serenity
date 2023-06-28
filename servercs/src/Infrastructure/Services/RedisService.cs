using Microsoft.Extensions.Configuration;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using Serenity.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Serenity.Infrastructure.Services;

public class RedisService : ICacheService {
	private readonly IDatabase db;

	public RedisService(IConfiguration configuration) {
		ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redisa") ?? "localhost");
		db = redis.GetDatabase();
	}

	public T? GetData<T>(string key) {
		var value = db.StringGet(key);
		if (!string.IsNullOrEmpty(value)) {
			return JsonSerializer.Deserialize<T>(value!);
		}

		return default;
	}

	public bool SetData<T>(string key, T value, DateTimeOffset expirationTime) {
		TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
		var isSet = db.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
		return isSet;
	}

	public bool RemoveData(string key) {
		bool isKeyExist = db.KeyExists(key);
		if (isKeyExist == true) {
			return db.KeyDelete(key);
		}
		return false;
	}
}
