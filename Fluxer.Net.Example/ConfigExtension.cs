namespace Fluxer.Net.Example
{
	public static class ConfigExtension
	{
		//Load YML configuration from file
		public static Dictionary<string, string>? LoadConfig()
		{
			//NOTE: This path may need to be modified depending on where you run the example from
			if (File.Exists("../../../config.yml"))
			{
				string[] lines = File.ReadAllLines("../../../config.yml");
				Dictionary<string, string> configValues = ParseYaml(lines);

				// Use the values from the config
				if (configValues.TryGetValue("Token", out var token))
				{
					Console.WriteLine($"Token: {token}");
				}

				return configValues;
			}

			return null;
		}

		//Custom YML parser
		private static Dictionary<string, string> ParseYaml(string[] lines)
		{
			Dictionary<string, string> configValues = new Dictionary<string, string>();
			string currentKey = "";

			foreach (string line in lines)
			{
				if (line.Trim().StartsWith('#'))
				{
					// Skip comments
					continue;
				}

				if (line.Contains(":"))
				{
					string[] parts = line.Split(':');
					currentKey = parts[0].Trim();
					string value = parts.Length > 1 ? parts[1].Trim() : "";
					configValues[currentKey] = value;
				}
				else if (!string.IsNullOrEmpty(currentKey))
				{
					// Multi-line values
					configValues[currentKey] += Environment.NewLine + line.Trim();
				}
			}

			return configValues;
		}
	}
}
