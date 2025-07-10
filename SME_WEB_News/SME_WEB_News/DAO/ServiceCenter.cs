using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SME_WEB_News.DAO
{
    public class ServiceCenter
    {
        private readonly IConfiguration _configuration;
        private readonly CallAPIService _callAPIService;
        protected static string APIpath;

        public ServiceCenter(IConfiguration configuration, CallAPIService callAPIService)
        {
            _configuration = configuration;
            _callAPIService = callAPIService;
        }
        public static vDropdownDTO GetLookups(string Ltype = null, string apipath = null, string TokenStr = null)
        {


            APIpath = apipath + "Dropdown/LookUp/" + Ltype;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            httpWebRequest.ContentType = "application/json";
         //   httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);
            httpWebRequest.Method = "GET";
            vDropdownDTO Llist = new vDropdownDTO();
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {

                    var result = streamReader.ReadToEnd();
                    Llist = JsonConvert.DeserializeObject<vDropdownDTO>(result);
                    return Llist;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

    
        #region Email
        public static MMailModels GetMailAll(string apipath = null, string TokenStr = null, string code = null)
        {

            APIpath = apipath + "ServiceCommon/GetMailAll/" + code;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(APIpath);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + TokenStr);
            httpWebRequest.Method = "GET";
            MMailModels Llist = new MMailModels();
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {

                    var result = streamReader.ReadToEnd();
                    Llist = JsonConvert.DeserializeObject<MMailModels>(result);
                    return Llist;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool sendmail(MMailModels GetBodymail, string toEmail, MailSettingModels xmailSetting)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {

                    mail.From = new MailAddress(xmailSetting.EmailAdminfrom, xmailSetting.EmailAdminfromName);

                    mail.To.Add(toEmail);
                    mail.CC.Add(toEmail);
                    mail.Subject = GetBodymail.MailSubject;
                    mail.Body = GetBodymail.MailDetail;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(xmailSetting.Host, int.Parse(xmailSetting.Port)))
                    {
                        smtp.Credentials = new NetworkCredential(xmailSetting.EmailAdminfrom, xmailSetting.EmailPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion Email



        public async Task<ApiListBusinessUnitResponse> GetDepartmentModelAsync(string apipath = null, string code = null) 
        {
            try {
                var resultDepart = await GetExternalAPI(apipath, code); // Await the Task<string> result

                return JsonConvert.DeserializeObject<ApiListBusinessUnitResponse>(resultDepart);
            } catch (Exception ex) 
            {
                return new ApiListBusinessUnitResponse();
            }
        }
        public async Task<string> GetExternalAPI(string apipath = null, string code = null)
        {
            var LApi = await GetExternalInformationAsync(apipath, code);

            // Use the injected _callAPIService instance to call GetDataApiAsync
            var resultApi = await _callAPIService.GetDataApiAsync(LApi, null);
            return resultApi;
        }

        public static async Task<MapiInformationModels?> GetExternalInformationAsync(string apipath = null, string code = null)
        {
            if (string.IsNullOrEmpty(apipath) || string.IsNullOrEmpty(code))
                return null;

            string apiUrl = $"{apipath}ApiInformation/{code}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MapiInformationModels>(result);
                }
                catch
                {
                    return null;
                }
            }
        }
        #region Convert time Hour Date
        public static string ConvertdDigittimeinttoString(string xHr, string xMin)
        {
            string Hr = "";
            string mm = "";
            string HHmm = "";

            if (xHr.Length == 1)
            {
                Hr = "0" + xHr;
            }
            else
            {
                Hr = xHr;
            }
            if (xMin.Length == 1)
            {
                mm = "0" + xMin;
            }
            else
            {
                mm = xMin;
            }
            HHmm = Hr + ":" + mm;
            return HHmm;

        }
        public static TimeOnly ConverttimeinttoString(string xHr, string xMin)
        {
            TimeOnly timeOnly = new TimeOnly();
            try
            {
                // แยกชั่วโมงและนาทีจาก xtime
                int hours = int.Parse(xHr);
                int minutes = int.Parse(xMin);
                // แปลงนาทีเกินเป็นชั่วโมงเพิ่มเติม
                hours += minutes / 60;       // เพิ่มชั่วโมงจากนาทีที่เกิน
                minutes = minutes % 60;       // คำนวณนาทีที่เหลือ
                string sethour;               // สร้าง TimeOnly โดยใช้ชั่วโมงและนาที
                string setMinus;

                if (hours.ToString().Length == 1)
                {
                    sethour = "0" + hours.ToString();
                }
                else
                {
                    sethour = hours.ToString();
                }

                //set minus
                if (minutes.ToString().Length == 1)
                {
                    setMinus = "0" + minutes.ToString();
                }
                else
                {
                    setMinus = minutes.ToString();
                }

                DateTime dateTime = DateTime.ParseExact(sethour + ":" + setMinus.ToString(), "HH:mm", null);
                timeOnly = new TimeOnly(dateTime.Hour, dateTime.Minute);


            }
            catch (Exception ex)
            {

            }
            return timeOnly;

        }
        public static int ConvertToMinutes(string time)
        {
            // แยกเวลาเป็นชั่วโมงและนาทีโดยใช้ :
            string[] parts = time.Split(':');

            // แปลงชั่วโมงและนาทีจาก string เป็น int
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            // คำนวณนาทีทั้งหมด
            return hours * 60 + minutes;
        }
        public static int ConvertMinutesToHouse(string time)
        {
            // แยกเวลาเป็นชั่วโมงและนาทีโดยใช้ :
            string[] parts = time.Split(':');

            // แปลงชั่วโมงและนาทีจาก string เป็น int
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            // คำนวณนาทีทั้งหมด
            return hours * 60 + minutes;
        }
        #endregion Convert time Hour Date

        #region Datateble Save
        public static int Bulkdata(string TbName, DataTable DtData)
        {
            var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
            int i = 0;
            string connectionString = config.GetSection("AppSettings:ConnectionString").Value; // แทนที่ด้วย connection string ของคุณ


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = TbName; // กำหนดชื่อของตารางปลายทาง

                    try
                    {
                        bulkCopy.WriteToServer(DtData); // คัดลอกข้อมูลจาก DataTable ไปยังฐานข้อมูล
                        Console.WriteLine("Bulk data inserted successfully.");
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return 0;
                    }
                }
            }

        }
        #endregion Datateble Save

        public static PagingModel LoadPagingViewModel(int rowcount, int? currentpage, int PageSize)
        {
            PagingModel page = new PagingModel();

            double dPageSize = PageSize;
            double totalRow;
            totalRow = rowcount;

            page.TotalPage = Math.Ceiling(totalRow / dPageSize); // แสดงค่าหน้าทั้งหมด
            page.CurrentPageNumber = (currentpage == 0) ? 1 : currentpage;
            page.PageSize = PageSize;


            return page;
        }

        #region GetHR data
        public async Task<vDropdownDTO> GetDdlDepartment(string apipath = null, string code = null)
        {
            vDropdownDTO Llist = new vDropdownDTO
            {
                DropdownList = new List<DropdownModels>()
            };
            try
            {
                ApiListBusinessUnitResponse dataDepart = await GetDepartmentModelAsync(apipath, code);
                if (dataDepart?.Results != null)
                {
                    foreach (var item in dataDepart.Results)
                    {
                        if (item != null)
                        {
                            Llist.DropdownList.Add(new DropdownModels
                            {
                                Code = item.BusinessUnitId,
                                Name = item.NameTh
                            });
                        }
                    }
                }
                return Llist;
            }
            catch
            {
                return new vDropdownDTO { DropdownList = new List<DropdownModels>() };
            }
        }

        public async Task<vDropdownDTO> GetPosition(string apipath = null, string code = null)
        {
            vDropdownDTO Llist = new vDropdownDTO
            {
                DropdownList = new List<DropdownModels>()
            };
            try
            {
                ApiListBusinessUnitResponse dataDepart = await GetDepartmentModelAsync(apipath, code);
                if (dataDepart?.Results != null)
                {
                    foreach (var item in dataDepart.Results)
                    {
                        if (item != null)
                        {
                            Llist.DropdownList.Add(new DropdownModels
                            {
                                Code = item.BusinessUnitCode,
                                Name = item.NameTh
                            });
                        }
                    }
                }
                return Llist;
            }
            catch
            {
                return new vDropdownDTO { DropdownList = new List<DropdownModels>() };
            }
        }
        #endregion

        #region Oauth2 Access Token

        public string getAccessToken(string clientId,string clientSecret,string authCode)
        {
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://122.155.9.179/rest/oauth/token");
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "POST";

            var request = (HttpWebRequest)WebRequest.Create("https://122.155.9.179/rest/oauth/token");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //string postData = $"grant_type=authorization_code" +
            //        $"&client_id={WebUtility.UrlEncode(clientId)}" +
            //        $"&client_secret={WebUtility.UrlEncode(clientSecret)}" +
            //        $"&redirect_url={WebUtility.UrlEncode(redirectUrl)}" +
            //        $"&code={WebUtility.UrlEncode(authCode)}" +
            //        $"&scope=openid email profile";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers["Authorization"] = $"Basic {credentials}";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    grant_type = "authorization_code",
                    client_id = clientId,
                    client_secret = clientSecret,
                    code = authCode
                });
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            dynamic jtoken = JsonConvert.DeserializeObject(result);
            Console.WriteLine(jtoken.access_token);
            return jtoken.access_token;
        }
        #endregion Oauth2 Access Token
        public static string EncodeBase64(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeBase64(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData)) return string.Empty;
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
