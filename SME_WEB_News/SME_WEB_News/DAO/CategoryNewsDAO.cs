using Newtonsoft.Json;
using SME_WEB_News.Models;
using System.Net;

namespace SME_WEB_News.DAO
{
    public class CategoryNewsDAO
    {
        private readonly IConfiguration _configuration;
        protected static string APIpath;
        public CategoryNewsDAO(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public static ViewCategoryNewsModels GetCategoryNews(CategoryNewsModels lh, string apipath = null, string flagCount = null, int currentpage = 0, int PageSize = 0, string TokenStr = null)
        {
            ViewCategoryNewsModels llh = new ViewCategoryNewsModels();

            APIpath = apipath + "Categories/GetCategory";
        
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            //var refreshtoken = refreshToken();
            //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var Req = new CategoryNewsModels();

                if (lh != null)
                {
                    Req = lh;
                }
                if (flagCount != "")
                {
                    Req.rowOFFSet = (currentpage - 1) * PageSize;
                    Req.rowFetch = PageSize;
                }
                else
                {
                    Req.rowOFFSet = 0;
                    Req.rowFetch = 0;
                }


                var json = JsonConvert.SerializeObject(Req, Formatting.Indented);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                llh = JsonConvert.DeserializeObject<ViewCategoryNewsModels>(result);

            }

            return llh;
        }
        public static CategoryNewsModels CreateCategoryNews(CategoryNewsModels lh, string apipath = null, string TokenStr = null)
        {
            CategoryNewsModels llh = new CategoryNewsModels();

            APIpath = apipath + "Categories/Create";
            //   https://localhost:44390/api/GS-ETL/v1/RawPunch/GetRawPunch
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            //var refreshtoken = refreshToken();
            //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var Req = lh;


                var json = JsonConvert.SerializeObject(Req, Formatting.Indented);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                llh = JsonConvert.DeserializeObject<CategoryNewsModels>(result);

            }

            return llh;
        }

        public static async Task<int> DeleteNewsAsync(int newsId, string apiPath, string tokenStr)
        {
            // if (string.IsNullOrEmpty(apiPath) || string.IsNullOrEmpty(tokenStr))
            if (string.IsNullOrEmpty(apiPath))
                throw new ArgumentException("API path and token must not be null or empty.");

            using (HttpClient client = new HttpClient())
            {
                //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStr);

                string requestUrl = $"{apiPath}News/{newsId}";

                HttpResponseMessage response = await client.DeleteAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error deleting news: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }

                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<int>(result);
            }
        }

    }
}
