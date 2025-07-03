using Newtonsoft.Json;
using SME_WEB_News.Models;
using System.Net;

namespace SME_WEB_News.DAO
{
    public class PopupDAO
    {
        private readonly IConfiguration _configuration;
        protected static string APIpath;
        public PopupDAO(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public static ViewPopupModels GetPopup(PopupModels lh, string apipath = null, string flagCount = null, int currentpage = 0, int PageSize = 0, string TokenStr = null)
        {
            ViewPopupModels llh = new ViewPopupModels();

            APIpath = apipath + "MPopup/GetPopup";
        
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            //var refreshtoken = refreshToken();
            //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var Req = new PopupModels();

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

                llh = JsonConvert.DeserializeObject<ViewPopupModels>(result);

            }

            return llh;
        }
        public static PopupModels CreatePopup(PopupModels lh, string apipath = null, string TokenStr = null)
        {
            PopupModels llh = new PopupModels();

            try {
                APIpath = apipath + "MPopup/Create";
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

                    llh = JsonConvert.DeserializeObject<PopupModels>(result);

                }

                return llh;
            }
            catch(Exception ex) 
            
            { throw new Exception("Error in CreatePopup: " + ex.Message); }
          
        }

        public static async Task<bool> DeletePopupAsync(int newsId, string apiPath, string tokenStr)
        {
         
            try
            {
                bool llh;

                APIpath = apiPath + "MPopup/" + newsId.ToString();

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
        public static async Task<bool> UpdateStatusActivePopup(PopupModels models, string apiPath)
        {
            try
            {
                bool llh;

                APIpath = apiPath + "Mpopup/UpdateStatusActivePopup";

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
