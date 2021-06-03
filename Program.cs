using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace dotnet__fetch_data_subset
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please input one of these words: name, currency, code or capital");
                return;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://restcountries.eu");
                var response = await client.GetAsync($"rest/v2/{args[0].ToLower()}/{args[1].ToLower()}?fields=name;capital;population;region");
                // Gets a value that indicates whether the HTTP response was successful(type bool)
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("There was an error fetching data.");
                    return;
                }
                var content = await response.Content.ReadAsStringAsync();
                try
                { // This is a case to return a list of objects
                    var countryList = JsonSerializer.Deserialize<List<Country>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    foreach (var i in countryList)
                    {
                        Console.WriteLine($"name: {i.Name}, capital: {i.Capital}, region: {i.Region}, population: {i.Population}");
                    }
                } 
                catch (JsonException)
                { // This is a case to rutrn one object (not a list)
                    var country = JsonSerializer.Deserialize<Country>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Console.WriteLine($"name: {country.Name}, capital: {country.Capital}, region: {country.Region}, population: {country.Population}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // re-throw an exception handled in a catch block
                    throw;
                }
            }
        }
    }
}
