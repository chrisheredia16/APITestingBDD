using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace APITestingBDD.Steps
{
    [Binding]
    public class OpenWeatherMapAPI
    {
        private string apiKey = "e5b4a1e464cac94a034da43fe5f1b31e";
        private UriBuilder OpenWeatherMapUri { get; } = new UriBuilder("http://api.openweathermap.org/data/2.5/forecast");
        private IRestResponse Response { get; set; }
        private string City { get; set; }
        private JObject ResponseBody { get; set; }

        [Given(@"I want to know the weather in a city")]
        public void GivenIWantToKnowTheWeatherIn(Table table)
        {
            dynamic data = table.CreateDynamicInstance();
            City = data.city;
            OpenWeatherMapUri.Query = "q=" + City + "&units=metric&appid=" + apiKey;
        }

        [When(@"I pass the city to the OpenWeatherMap API")]
        public void WhenIPassTheCityNameToTheOpenWeatherMapAPI()
        {
            var client = new RestClient(OpenWeatherMapUri.Uri);
            var request = new RestRequest(Method.GET);
            Response = client.Execute(request);
        }

        [Then(@"The result should match with the city sent")]
        public void ThenTheResultShouldBeReturn()
        {
            Response.StatusCode.Should().Be(HttpStatusCode.OK, "Assertion needed to know is the Response status is OK");

            //assert for json validation.
            IsValidJson(Response.Content).Should().BeTrue();

            ResponseBody = JObject.Parse(Response.Content);
            string[] cityAndCountry = City.Split(',');
            ResponseBody["city"]["name"].Value<string>()
                .Should()
                .Be(cityAndCountry[0], "returned forecast should of same city");
            if (cityAndCountry.Length > 1)
                ResponseBody["city"]["country"].Value<string>()
                    .Should()
                    .Be(cityAndCountry[1], "returned forecast should of same country");
        }

        private static bool IsValidJson(string jsonString)
        {
            jsonString = jsonString.Trim();
            if ((jsonString.StartsWith("{") && jsonString.EndsWith("}")) || //For object
                (jsonString.StartsWith("[") && jsonString.EndsWith("]"))) //For array
            {
                try
                {
                    JToken.Parse(jsonString);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return false;
        }

    }
}
