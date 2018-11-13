using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml;


namespace BPMConnect
{
    class ResponseStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Exception { get; set; }
        public object PasswordChangeUrl { get; set; }
        public object RedirectUrl { get; set; }
    }

    class DataResult
    {
        public string GetDataResult { get; set; }
    }

    [ComVisible(true)]
    [Guid("f1864f7a-c8fe-49aa-86ad-b745cab1fb9f")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("BPMConnect")]
    public class BPMRequest
    {

        public static CookieContainer AuthCookie = new CookieContainer();

        private static string _supervisorLogin;
        private static string SupervisorLogin
        {
            get { return _supervisorLogin ?? "SAMServerSDK6"; }
            set { _supervisorLogin = value; }
        }

        private static string _supervisorPassword;
        private static string SupervisorPassword
        {
           get{ return _supervisorPassword ?? "HappyNewRobot2019";}
           set{ _supervisorPassword = value;}
        }

        private static string _baseUri;
        private static string BaseUri
        {
            get { return _baseUri ?? "http://localhost:82/"; }
            set { _baseUri = value; }
        }
        private static string GetDataUri
        {
            get 
            {
                return BaseUri + @"/0/rest/G2ConnectorService/GetData";
            }
        }
        private static string AuthServiceUri
        {
            get
            {
                return BaseUri + @"/ServiceModel/AuthService.svc/Login";
            }
        }

        //private const string authServiceUri = baseUri + @"/ServiceModel/AuthService.svc/Login";

        //private const string UserListUri = baseUri + @"/0/rest/CustomConfigurationService/GetTransformValue";

        //private const string UserListUri = baseUri + @"/0/rest/G2ConnectorService/GetData";

        [ComVisible(true)]
        public string SendRequest(string postData, string queryType, string baseUri = null, string supervisorLogin = null, string supervisorPassword = null)
        {
            SupervisorLogin = supervisorLogin;
            SupervisorPassword = supervisorPassword;
            BaseUri = baseUri;

            if (TryLogin(SupervisorLogin, SupervisorPassword))
            {
                return GetData(postData, queryType);
            }
            else
                return "Authentification error";

        }
        // Выполняет запрос на аутентификацию пользователя.
        public static bool TryLogin(string userName, string userPassword)
        {
            // Создание экземпляра запроса к сервису аутентификации.
            var authRequest = HttpWebRequest.Create(AuthServiceUri) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            // Включение использования cookie в запросе.
            authRequest.CookieContainer = AuthCookie;

            // Помещение в тело запроса учетной информации пользователя.
            using (var requestStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStream))
                {
                    var str = @"{""UserName"":""" + userName + @""",""UserPassword"":""" + userPassword + @"""}";
                    writer.Write(str);
                }
            }

            // Вспомогательный объект, в который будут десериализованы данные HTTP-ответа.
            ResponseStatus status = null;
            // Получение ответа от сервера. Если аутентификация проходит успешно, в свойство AuthCookie будут
            // помещены cookie, которые могут быть использованы для последующих запросов.
            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    // Десериализация HTTP-ответа во вспомогательный объект.
                    string responseText = reader.ReadToEnd();
                    status = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);
                }
            }

            // Проверка статуса аутентификации.
            if (status != null)
            {
                // Успешная аутентификация.
                if (status.Code == 0)
                {
                    return true;
                }
                // Сообщение о неудачной аутентификации.
            }
            return false;
        }

        private static XmlNode GetNode(string NodeName, XmlNode xRoot)
        {
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == NodeName)
                {
                    return xnode;
                }
            }
            return xRoot;
        }

        private static XDocument GetUsersRequest()
        {
        XDocument xdoc = new XDocument(new XElement("DataRequest",
        new XElement("Table",
        new XAttribute("Name", "Users"),
        new XAttribute("Destination", "Users"),
        new XElement("Row",
                new XElement("Field",
                    new XAttribute("Name", "ID"),
                    new XAttribute("Destination", "ID"),
                    new XAttribute("Type", "STANDART")),
                new XElement("Field",
                    new XAttribute("Name", "FirstName"),
                    new XAttribute("Destination", "FirstName"),
                    new XAttribute("Type", "STANDART")),
                new XElement("Field",
                    new XAttribute("Name", "LastName"),
                    new XAttribute("Destination", "LastName"),
                    new XAttribute("Type", "STANDART"))
                    )
        )));
        //xdoc.Save(@"C:\XXXXX\Users.xml");
        return xdoc;
        }

        private static string GetUsersResponce(string PostString)
        {   
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(PostString);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            var tableNode = GetNode("Table", xRoot);
            var rowNode = GetNode("Row", tableNode);

            XmlNode cNode = rowNode.CloneNode(true);

            foreach (XmlNode cAttr in cNode.ChildNodes)
            {
                XmlNode attr = cAttr.Attributes.GetNamedItem("Name");
                if (attr != null)
                    switch (attr.Value)
                {
                    case "ID":
                        //cAttr.InnerText = dataReader.GetColumnValue<string>("ID");
                        cAttr.InnerText = "ID1";
                        break;
                    case "FirstName":
                        //cAttr.InnerText = dataReader.GetColumnValue<string>("Name");
                        cAttr.InnerText = "Supervisor";
                        break;
                }
            }

            tableNode.AppendChild(cNode);
            tableNode.RemoveChild(rowNode);
            //xDoc.Save(@"C:\XXXXX\Users1.xml");

            return xDoc.OuterXml;
        }

        public static string GetData(string PostString, string QueryType = "")
        {
            var authRequest = HttpWebRequest.Create(GetDataUri) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            authRequest.CookieContainer = AuthCookie;

            if (PostString == "") // для отладки
            {
                PostString = GetUsersRequest().ToString();
            };

            if (QueryType == "Debug")
            {
                return GetUsersResponce(PostString);
            }

            using (var requestStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(@"{
                    ""PostString"":""" + PostString + @""",
                    ""QueryType"":""" + QueryType + @"""
                       }");
                }
            }

            string responseText;
            DataResult status = null;

            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    //string 
                        responseText = reader.ReadToEnd();
                        status = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<DataResult>(responseText);
                }

            }

            if (status != null)
            {
                // Успех
                return status.GetDataResult;
            }

            return responseText; 

        }
    }
}
