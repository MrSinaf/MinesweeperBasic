using System.Text.Json;

namespace MinesweeperBasic;

public static class Local
{
	private static readonly Dictionary<string, string> data = [];
	
	public static void Load(string path)
	{
		var filePath = Path.Combine("assets/locales", path + ".json");
		if (!File.Exists(filePath))
			throw new FileNotFoundException($"Localization file not found TwT: {filePath}");
		
		data.Clear();
		
		var json = File.ReadAllText(filePath);
		using var document = JsonDocument.Parse(json);
		
		if (document.RootElement.ValueKind != JsonValueKind.Object)
			throw new InvalidDataException("Localization file must contain a JSON object at root.");
		
		Flatten(document.RootElement, string.Empty);
	}
	
	public static string Get(string key)
		=> string.IsNullOrWhiteSpace(key) ? key : data.GetValueOrDefault(key, key);
	
	public static string Get(string key, params object[] args)
	{
		var text = Get(key);
		return args.Length == 0 ? text : string.Format(text, args);
	}
	
	private static void Flatten(JsonElement element, string prefix)
	{
		foreach (var property in element.EnumerateObject())
		{
			var currentKey = string.IsNullOrEmpty(prefix)
					? property.Name
					: $"{prefix}.{property.Name}";
			
			switch (property.Value.ValueKind)
			{
				case JsonValueKind.Object:
					Flatten(property.Value, currentKey);
					break;
				case JsonValueKind.String:
					data[currentKey] = property.Value.GetString() ?? string.Empty;
					break;
				case JsonValueKind.Null:
					data[currentKey] = string.Empty;
					break;
				default:
					data[currentKey] = property.Value.ToString();
					break;
			}
		}
	}
}