using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using BPMConnect;


namespace RequestAuthentification
{
    // Вспомогательный класс для десериализации JSON-объекта из HTTP-ответа.
    class ResponseStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Exception { get; set; }
        public object PasswordChangeUrl { get; set; }
        public object RedirectUrl { get; set; }
    }

    // Основной класс программы.
    class Program
    {
        // HTTP-адрес приложения.
        private const string baseUri = "http://localhost:82/";
        // Контейнер для Cookie аутентификации bpm'online. Необходимо использовать в последующих запросах.
        // Это самый важный результирующий объект, для формирования свойств которого разработана
        // вся остальная функциональность примера.
        public static CookieContainer AuthCookie = new CookieContainer();
        // Строка запроса к методу Login сервиса AuthService.svc.
        private const string authServiceUri = baseUri + @"/ServiceModel/AuthService.svc/Login";

        private const string UserListUri = baseUri + @"/0/rest/CustomConfigurationService/GetTransformValue";

        // Выполняет запрос на аутентификацию пользователя.
        public static bool TryLogin(string userName, string userPassword)
        {
            // Создание экземпляра запроса к сервису аутентификации.
            var authRequest = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;
            // Определение метода запроса.
            authRequest.Method = "POST";
            // Определение типа контента запроса.
            authRequest.ContentType = "application/json";
            // Включение использования cookie в запросе.
            authRequest.CookieContainer = AuthCookie;

            // Помещение в тело запроса учетной информации пользователя.
            using (var requestStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStream))
                {
                    //var str = @"{
                    //""UserName"":""" + userName + @""",
                    //""UserPassword"":""" + userPassword + @"""
                    //}";
                    var str = @"{""UserName"":""" + userName + @""",""UserPassword"":""" + userPassword + @"""}" ;
                    writer.Write(str);
                    //writer.Write("<UserName>Supervisor</UserName> <UserPassword>Supervisor</UserPassword>");
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
                Console.WriteLine(status.Message);
            }
            return false;
        }

        public static string GetData()
        {
            var authRequest = HttpWebRequest.Create(UserListUri) as HttpWebRequest;
            // Определение метода запроса.
            authRequest.Method = "POST";
            // Определение типа контента запроса.
            //'authRequest.ContentType = "application/json";
             authRequest.ContentType = "application/json";
            // Включение использования cookie в запросе.
            authRequest.CookieContainer = AuthCookie;
            string InputParam = "Test str";

            string responseText;

            // Помещение в тело запроса учетной информации пользователя.
            using (var requestStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(@"{
                    ""GetTransformValue"":""" + InputParam + @"""
                       }");
                    //writer.Write("<UserName>Supervisor</UserName> <UserPassword>Supervisor</UserPassword>");
                }
            }

            ResponseStatus status = null;
            // Получение ответа от сервера. Если аутентификация проходит успешно, в свойство AuthCookie будут
            // помещены cookie, которые могут быть использованы для последующих запросов.
            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    // Десериализация HTTP-ответа во вспомогательный объект.
                    responseText = reader.ReadToEnd();
                    status = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);
                }

            }

            // Проверка статуса аутентификации.
            if (status != null)
            {
                // Успешная аутентификация.
                if (status.Code == 0)
                {
                    return responseText;
                }
                // Сообщение о неудачной аутентификации.
                Console.WriteLine(status.Message);
            }
            return responseText;

        }
        // Метод входа в приложение.
        static void Main(string[] args)
        {
            // Вызов метода аутентификации.

            BPMConnect.BPMRequest req = new BPMConnect.BPMRequest();
            var str = req.SendRequest();
            Console.WriteLine(str);
            //Console.WriteLine("Успешна ли аутентификация?: {0}", TryLogin("Supervisor", "Supervisor"));
            //Console.WriteLine(GetData());
            Console.WriteLine("Для выхода нажмите ENTER...");
            Console.ReadLine();
        }
    }
}
