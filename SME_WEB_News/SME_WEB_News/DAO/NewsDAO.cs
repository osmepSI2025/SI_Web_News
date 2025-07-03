using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME_WEB_News.Models;
using System.Net;

namespace SME_WEB_News.DAO
{
    public class NewsDAO
    {
        private readonly IConfiguration _configuration;
        protected static string APIpath;
        public NewsDAO(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public static async Task<ViewMNewsModels> GetNews(MNewsModels lh, string apipath = null, string flagCount = null, int currentpage = 0, int PageSize = 0, string TokenStr = null)
        {
            try {
                ViewMNewsModels llh = new ViewMNewsModels();

                APIpath = apipath + "News/GetNews";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
                //var refreshtoken = refreshToken();
                //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var Req = new MNewsModels();

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

                    llh = JsonConvert.DeserializeObject<ViewMNewsModels>(result);

                }

                return llh;
            } catch (Exception ex) 
            {
                return null;
            }
        
        }
        public static MNewsModels CreateNews(MNewsModels lh, string apipath = null, string TokenStr = null)
        {
            MNewsModels llh = new MNewsModels();

            APIpath = apipath + "News/CreateNews";
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

                llh = JsonConvert.DeserializeObject<MNewsModels>(result);

            }

            return llh;
        }

        public static MNewsModels EditNews(MNewsModels lh, string apipath = null, string TokenStr = null)
        {
            MNewsModels llh = new MNewsModels();

            APIpath = apipath + "News/"+ lh.Id;
            //   https://localhost:44390/api/GS-ETL/v1/RawPunch/GetRawPunch
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            //var refreshtoken = refreshToken();
            //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = "PUT";

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

                llh = JsonConvert.DeserializeObject<MNewsModels>(result);

            }

            return llh;
        }

        public static async Task<bool> DeleteNewsAsync(int newsId, string apiPath, string tokenStr)
        {
            try
            {
                bool llh;

                APIpath = apiPath + "news/" + newsId.ToString();

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
                //var refreshtoken = refreshToken();
                //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "DELETE";



                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    int xresult = JsonConvert.DeserializeObject<int>(result);

                    if (xresult != 0)
                    {
                        llh = true;
                    }
                    else
                    {
                        llh = false;
                    }
                }

                return llh;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
        public static int UpdateRangeNews(MNewsModels lh, string apipath = null, string TokenStr = null)
        {
            try
            {
                MNewsModels llh = new MNewsModels();

                APIpath = apipath + "News/UpdateRangeNews";
              
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

                    int x = JsonConvert.DeserializeObject<int>(result);

                }

                return 1;
            }
            catch (Exception ex) { return 0; }
          
        }

        public static async Task<bool> UpdateStatusActiveNews(MNewsModels models, string apiPath)
        {
            try
            {
                bool llh;

                APIpath = apiPath + "news/UpdateStatusActiveNews";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = JsonConvert.SerializeObject(models, Formatting.Indented);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    llh = JsonConvert.DeserializeObject<bool>(result);
                }

                return llh;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
