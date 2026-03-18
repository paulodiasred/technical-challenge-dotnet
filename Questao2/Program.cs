using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private const string BaseUrl = "https://jsonmock.hackerrank.com/api/football_matches";

    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        return GetTotalScoredGoalsAsync(team, year).GetAwaiter().GetResult();
    }

    private static async Task<int> GetTotalScoredGoalsAsync(string team, int year)
    {
        int goalsAsTeam1 = await GetGoalsByTeamPosition(team, year, "team1");
        int goalsAsTeam2 = await GetGoalsByTeamPosition(team, year, "team2");
        return goalsAsTeam1 + goalsAsTeam2;
    }

    private static async Task<int> GetGoalsByTeamPosition(string team, int year, string teamField)
    {
        int totalGoals = 0;
        int currentPage = 1;
        int totalPages = 1;

        while (currentPage <= totalPages)
        {
            string requestUrl = $"{BaseUrl}?year={year}&{teamField}={Uri.EscapeDataString(team)}&page={currentPage}";
            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            FootballMatchApiResponse? apiResponse = JsonConvert.DeserializeObject<FootballMatchApiResponse>(json);
            if (apiResponse == null)
            {
                throw new InvalidOperationException("Nao foi possivel desserializar a resposta da API.");
            }

            totalPages = apiResponse.TotalPages;

            foreach (FootballMatch match in apiResponse.Data)
            {
                if (teamField == "team1")
                {
                    totalGoals += ParseGoals(match.Team1Goals);
                }
                else
                {
                    totalGoals += ParseGoals(match.Team2Goals);
                }
            }

            currentPage++;
        }

        return totalGoals;
    }

    private static int ParseGoals(string goals)
    {
        return int.TryParse(goals, out int parsedGoals) ? parsedGoals : 0;
    }
}

public class FootballMatchApiResponse
{
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("total_pages")]
    public int TotalPages { get; set; }

    [JsonProperty("data")]
    public List<FootballMatch> Data { get; set; } = new List<FootballMatch>();
}

public class FootballMatch
{
    [JsonProperty("team1")]
    public string Team1 { get; set; } = string.Empty;

    [JsonProperty("team2")]
    public string Team2 { get; set; } = string.Empty;

    [JsonProperty("team1goals")]
    public string Team1Goals { get; set; } = "0";

    [JsonProperty("team2goals")]
    public string Team2Goals { get; set; } = "0";
}