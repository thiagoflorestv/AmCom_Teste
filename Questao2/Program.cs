using Newtonsoft.Json;
using Questao2;

public class Program
{
    private static readonly HttpClient client = new HttpClient();
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await getTotalScoredGoalsAsync(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await getTotalScoredGoalsAsync(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> getTotalScoredGoalsAsync(string team, int year)
    {
        int totalGoals = 0;
        int page = 1;
        bool morePages = true;


        while (morePages)
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}&page={page}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            ApiResponse? apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

            foreach (var match in apiResponse?.Data!)
            {
                if (match.Team1!.Equals(team, StringComparison.OrdinalIgnoreCase))
                {
                    totalGoals += match.Team1Goals;
                }
                if (match.Team2!.Equals(team, StringComparison.OrdinalIgnoreCase))
                {
                    totalGoals += match.Team2Goals;
                }
            }

            if (page < apiResponse.TotalPages)
            {
                page++;
            }
            else
            {
                morePages = false;
            }

        }

        return totalGoals;

    }

}