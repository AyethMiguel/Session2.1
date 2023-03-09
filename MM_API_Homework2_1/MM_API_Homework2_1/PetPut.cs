using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MM_API_Homework2_1
{
    [TestClass]
    public class PetPut
    {
        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));
         
        private readonly List<PetInfo> cleanUpList = new List<PetInfo>();

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetEndpoint}/{data.Id}"));
            }
        }

        [TestMethod]
        public async Task PutMethod()
        {
            #region create data

            // Create Json Object
            PetInfo petData = new PetInfo()
            {
                Category = new Category() {Id = 1, Name = "string"},
                Name = "Ming-ming",
                PhotoUrls = new string[] {"string"},
                Tags = new Category[] {new Category() { Id = 1, Name = "string"}},
                Status = "available"
            };

            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            var httpResponse = await httpClient.PostAsync(GetURL(PetEndpoint), postRequest);
            var petDataFromPost = JsonConvert.DeserializeObject<PetInfo>(httpResponse.Content.ReadAsStringAsync().Result);
           
            #endregion

            #region send put request to update data

            // Update value of userData
            petData = new PetInfo()
            {
                Id = petDataFromPost.Id,
                Category = petDataFromPost.Category,
                Name = "Ming-ming - UPDATE BY PUT",
                PhotoUrls = petDataFromPost.PhotoUrls,
                Tags = petDataFromPost.Tags,
                Status = petDataFromPost.Status
            };

            // Serialize Content
            request = JsonConvert.SerializeObject(petData);
            var putRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Put Request
            httpResponse = await httpClient.PutAsync(GetURL(PetEndpoint), putRequest);

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            #endregion

            #region get updated data

            // Get Request
            var getResponse = await httpClient.GetAsync(GetURI($"{PetEndpoint}/{petDataFromPost.Id}"));

            // Deserialize Content
            var listPetInfo = JsonConvert.DeserializeObject<PetInfo>(getResponse.Content.ReadAsStringAsync().Result);

            #endregion

            #region cleanup data

            // Add data to cleanup list
            cleanUpList.Add(listPetInfo);

            #endregion

            #region assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 200");
            Assert.AreNotEqual(petDataFromPost.Name, listPetInfo.Name, "Pet names should not be equal.");
            Assert.AreEqual(petDataFromPost.Category.Id, listPetInfo.Category.Id, "Category Id does not match.");
            Assert.AreEqual(petDataFromPost.Category.Name, listPetInfo.Category.Name, "Category Name does not match.");
            Assert.AreEqual(petDataFromPost.PhotoUrls[0], listPetInfo.PhotoUrls[0], "Photo URL does not match.");
            Assert.AreEqual(petDataFromPost.Tags[0].Id, listPetInfo.Tags[0].Id, "Tags Id does not match.");
            Assert.AreEqual(petDataFromPost.Tags[0].Name, listPetInfo.Tags[0].Name, "Tags Name does not match.");
            Assert.AreEqual(petDataFromPost.Status, listPetInfo.Status, "Status does not match.");

            #endregion

        }

    }
}
