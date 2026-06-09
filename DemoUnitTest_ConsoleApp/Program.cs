using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DemoUnitTest_ConsoleApp;

public class Program
{
    public static async Task Main()
    {
        string? calculatorPath = FindUpwardFile(AppContext.BaseDirectory, "Calculator.cs");
        if (calculatorPath == null)
        {
            Console.WriteLine("Calculator.cs not found");
            return;
        }

        string methodCode = await File.ReadAllTextAsync(calculatorPath, Encoding.UTF8);

        var prompt = $"""
/no_think
Write a complete C# xUnit test file for the following code.
Return only compilable C# code. Do not use Markdown.
Use namespace UnitTest.
Include using DemoUnitTest_ConsoleApp; and using Xunit;.
Do not use Moq. Instantiate Calculator and assert results.

Code:
{methodCode}
""";

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(6) };
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "lm-studio");

        string model = Environment.GetEnvironmentVariable("LM_STUDIO_MODEL") ?? "openai/gpt-oss-20b";

        var body = new
        {
            model,
            messages = new[] { new { role = "user", content = prompt } },
            max_tokens = 1000,
            stream = false,
            temperature = 0.2
        };

        string json = System.Text.Json.JsonSerializer.Serialize(body);
        using var resp = await client.PostAsync(
            "http://localhost:1234/v1/chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json"));

        resp.EnsureSuccessStatusCode();

        string text = await resp.Content.ReadAsStringAsync();
        string raw = JObject.Parse(text)["choices"]![0]!["message"]!["content"]!.ToString();
        string unitTestCode = StripCodeFence(raw);
        if (string.IsNullOrWhiteSpace(unitTestCode))
        {
            throw new InvalidOperationException("LM Studio returned empty test code.");
        }

        string unitTestDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(calculatorPath)!, "..", "UnitTest"));
        Directory.CreateDirectory(unitTestDir);

        string outFile = Path.Combine(unitTestDir, "UnitTest_Generated.cs");
        await File.WriteAllTextAsync(outFile, unitTestCode, Encoding.UTF8);
        Console.WriteLine($"Saved: {outFile}");
    }

    private static string? FindUpwardFile(string start, string name, int max = 8)
    {
        var directory = new DirectoryInfo(start);
        for (int i = 0; i < max && directory != null; i++, directory = directory.Parent)
        {
            string candidate = Path.Combine(directory.FullName, name);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static string StripCodeFence(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        int start = value.IndexOf("```", StringComparison.Ordinal);
        if (start >= 0)
        {
            int end = value.IndexOf("```", start + 3, StringComparison.Ordinal);
            if (end > start)
            {
                value = value.Substring(start + 3, end - start - 3);
            }

            string[] lines = value.Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Split('\n');
            if (lines.Length > 0 &&
                (string.Equals(lines[0].Trim(), "csharp", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(lines[0].Trim(), "cs", StringComparison.OrdinalIgnoreCase)))
            {
                value = string.Join(Environment.NewLine, lines.Skip(1));
            }
        }

        return value.Trim();
    }
}
