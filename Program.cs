using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Timer = System.Timers.Timer;

class Program
{
    private static Timer _timer;
    private static HttpClient _httpClient = new HttpClient();
    //private static string _apiKey = "3d88548e05fea099ed638f676acf00b2"; // Replace with your OpenWeatherMap API key
    private static string _url = $"https://official-joke-api.appspot.com/random_joke";
    // Get the current directory of 'project' folder
    static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    static string projectRootDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

    private static string _outputFolder = projectRootDirectory+"\\resultData"; // Replace with your desired output folder
    static async Task Main(string[] args)
    {
        // Ensure output directory exists
        Directory.CreateDirectory(projectRootDirectory);
        // Set up a timer to trigger every minute (60000 milliseconds)
        _timer = new Timer(6000);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;

        // Keep the application running
        Console.WriteLine("Service started. Press [Enter] to exit.");
        Console.ReadLine();
    }

    private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        try
        {
            Console.WriteLine("Fetching data at: " + DateTime.Now);
            var jsonResponse = await FetchDataAsync(_url);
            if (jsonResponse != null)
            {
                var filePath = Path.Combine(_outputFolder, $"data_{DateTime.Now:yyyyMMdd}.json");
                await File.AppendAllTextAsync(filePath, jsonResponse);
                Console.WriteLine($"Data written to {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task<string> FetchDataAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();

        // Optionally parse the response to extract specific data (like temperature)
        var json = JObject.Parse(jsonResponse);
        var setup = json["setup"].ToString();
        var punchline = json["punchline"].ToString();
        // Create a simplified JSON object with only the relevant data
        var result = new JObject
        {
            ["setup"] = setup,
            ["punchline"] = punchline,
            ["timestamp"] = DateTime.Now.ToString("o")
        };

        return result.ToString(Formatting.Indented);
    }
}