using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMConnect;



namespace BPMConnectTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //String PostString = "<TEst Name = 'Users'>Body</TEst>";
            //String PostString = @"<DataRequest><Table Name=""Users"" Destination=""Users""><Row><Field Name=""ID"" Destination=""ID"" Type=""STANDARD""/><Field Name=""FirstName"" Destination=""FirstName"" Type=""STANDARD""/><Field Name=""LastName"" Destination=""LastName"" Type=""STANDARD""/></Row></Table></DataRequest>";
            String PostString = @"<DataRequest><Table Name='Users'><Row><Field Name='ID'/><Field Name='FirstName'/></Row></Table></DataRequest>";
            //String PostString = "";
            BPMRequest r = new BPMConnect.BPMRequest();
            //System.Console.WriteLine(r.SendRequest(PostString));
            System.Console.WriteLine(r.SendRequest(PostString, "UsersList", null , "Supervisor", "Supervisor"));
            System.Console.ReadLine();

        }
    }
}
