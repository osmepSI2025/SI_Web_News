using Newtonsoft.Json;
using SME_WEB_News.Models;
using System.Net;
using System.Reflection;

namespace SME_WEB_News.DAO
{
    public class EmployeeDAO
    {
        private readonly IConfiguration _configuration;
        protected static string APIpath;
        public EmployeeDAO(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public static List<EmployeeRoleModels> GetEmpAllRole( string apipath = null, string flagCount = null, int currentpage = 0, int PageSize = 0, string TokenStr = null)
        {
            List<EmployeeRoleModels> llh = new List<EmployeeRoleModels>();

            APIpath = apipath + "UserManagement/GetEmployeeProfileList";
        
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            //var refreshtoken = refreshToken();
            //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = "GET";

     

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                llh = JsonConvert.DeserializeObject<List<EmployeeRoleModels>>(result);

            }

            return llh;
        }
        public static async Task<ViewEmployeeRoleModels> GetEmpByBu(searchEmployeeRoleModels model,string apipath = null, string code = null)
        {


            ViewEmployeeRoleModels ldata = new ViewEmployeeRoleModels();


            try
            {
                APIpath = apipath + "UserManagement/GetAllEmployeeList";
                //   https://localhost:44390/api/GS-ETL/v1/RawPunch/GetRawPunch
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
                //var refreshtoken = refreshToken();
                //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var Req = model;


                    var json = JsonConvert.SerializeObject(Req, Formatting.Indented);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    ldata = JsonConvert.DeserializeObject<ViewEmployeeRoleModels>(result);
                  
               
                }

                return ldata;
            }
            catch (Exception ex)

            { throw new Exception("Error in CreatePopup: " + ex.Message); }
        }

        public static async Task<int> InsrtRoleEmpByBu(EmployeeRoleModels lh, string apipath = null, string TokenStr = null)
        {
            int resultIns = 0;
            EmployeeRoleModels ldata = new EmployeeRoleModels();
            try
            {
                APIpath = apipath + "UserManagement/Create";
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

                    ldata = JsonConvert.DeserializeObject<EmployeeRoleModels>(result);

                    if (ldata != null)
                    {
                        resultIns = 1;
                    }

                }

                return resultIns;
            }
            catch (Exception ex)

            { throw new Exception("Error in CreatePopup: " + ex.Message); }
        }

        public static async Task<bool> DeleteUserById(int newsId, string apiPath, string tokenStr = null)
        {
            try
            {
                bool llh;

                APIpath = apiPath + "UserManagement/" + newsId.ToString();

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
            } catch (Exception ex) 
            {
                return false;

            }

        }
        public static async Task<EmployeeModels> GetDetaiEmp(EmployeeModels model, string apipath = null)
        {


            EmployeeModels ldata = new EmployeeModels();


            try
            {
                APIpath = apipath + "UserManagement/GetEmployeeDetail";
              
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
                //var refreshtoken = refreshToken();
                //  httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);

                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var Req = model;


                    var json = JsonConvert.SerializeObject(Req, Formatting.Indented);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    ldata = JsonConvert.DeserializeObject<EmployeeModels>(result);


                }

                return ldata;
            }
            catch (Exception ex)

            { throw new Exception("Error in CreatePopup: " + ex.Message); }
        }

    }
}
