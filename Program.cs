using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Pets
{
    class Program
    {
        // create Pet and AnimalLover to map to the json data provided from API
        class Pet
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        class AnimalLover
        {
            public string Name { get; set; }
            public string Gender { get; set; }
            public int Age { get; set; }
            public List<Pet> Pets { get; set; }
        }

        private static readonly HttpClient Client = new HttpClient();

        static async Task Main(string[] args)
        {
            // Add the Microsoft.AspNet.WebApi.Client to get access to the http client factory from nuget library. 
            // (See Dependencies | Packages
            // this give us the access to the data from the web
            var httpClient = HttpClientFactory.Create();

            try
            {
                var url = "http://agl-developer-test.azurewebsites.net/people.json";
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);

                // check the response of the status code of http = 200 
                // so we can safely read the content of the response message using the ReadAsAsync method into the data 
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var content = httpResponseMessage.Content;
                    // Use the ReadAsSync method to consume json data and pass into this method as a List as an argument
                    var data = await content.ReadAsAsync<List<AnimalLover>>();

                    // now call our private function to process the data
                    ProcessAllCats(data);
                }
            } 
            catch(Exception e)  // catch any http request exception that could be raised
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// ProcessAllCats()
        /// This routine takes the list of animal lovers and splits it into two lists - male and female lists.
        /// Invokes the getCatNamesByAscendingOrder() funtion to get their cat's names if they own the pet.
        /// Then console out the their names
        /// </summary>
        /// <param name="animalLoverList"></param>
        private static void ProcessAllCats(List<AnimalLover> animalLoverList)
        {
            List<string> petNames = new List<string>();

            var maleAnimalLovers = animalLoverList.Where(m => m.Gender == "Male").ToList();
            petNames = getCatNamesByAscendingOrder(maleAnimalLovers);
            Console.WriteLine("--- Male ---");
            printToConsole(petNames);

            var femaleAnimalLovers = animalLoverList.Where(m => m.Gender == "Female").ToList();
            petNames = getCatNamesByAscendingOrder(femaleAnimalLovers);
            Console.WriteLine("--- Female ---");
            printToConsole(petNames);
        }

        /// <summary>
        /// getCatNamesByAscendingOrder() 
        /// This routine iterates through the given list of animalLovers and picks out the owner who has cat
        /// and add the cat's name to a List of string. Then sort the names list in ascending order. 
        /// 
        /// </summary>
        /// <param name="animalLovers"></param>
        /// <returns>The sorted List of cat's names</returns>
        private static List<string> getCatNamesByAscendingOrder(List<AnimalLover> animalLovers)
        {
            List<string> petNames = new List<string>();
            foreach (var person in animalLovers)
            {
                if (person.Pets != null)   // person owns pet
                {
                    var catList = person.Pets.Where(p => p.Type == "Cat").OrderBy(p=> p.Name).ToList();
                    foreach (var cat in catList)
                    {
                        petNames.Add(cat.Name);
                    }
                }
            }
            return petNames.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// printToConsole()
        /// this routine console out the cat names list per line.
        /// </summary>
        /// <param name="catNames"></param>
        private static void printToConsole(List<string> catNames)
        {
            foreach(var name in catNames)
            {
                Console.WriteLine(name);
            }
        }
    }
}
