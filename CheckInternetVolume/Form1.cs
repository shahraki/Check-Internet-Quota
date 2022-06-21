using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Reflection;

namespace CheckInternetVolume
{
    public partial class Form1 : Form
    {
        /* private static HttpClientHandler handler = new HttpClientHandler();
         private static readonly HttpClient client = new HttpClient(handler);*/
        private string ScreenName = "";
        private static readonly int LimitValume = 40;
        private static readonly int TimePeriod = 5; //in mituntes
        private static readonly string[] quotaEqualZeroMessages = new string[2] { "Something Went Wrong or Reached Quota Limit", "The return value is " };
        private static readonly string[] quotaOutOfLimitMessages = new string[2] { "Internet is Out of Limit", "The Quota is less than " };
        private static readonly string[] quotaInformationMessages = new string[2] { "Internet Quota Information", "The remaining Quota is " };
        private static readonly string[] InternetRegistrationSuccessMessages = new string[2] { "Internet Registration Information", "You have been registerd successfully" };
        private static readonly string[] InternetRegistrationUnsuccessMessages = new string[2] { "Internet Registration Information", "You have not been registerd successfully or already registered" };
        private static readonly string[] InternetUnRegistrationSuccessMessages = new string[2] { "Internet Unregistration Information", "You have been Unregisterd successfully" };
        private static readonly string[] InternetUnegistrationUnsuccessMessages = new string[2] { "Internet Unregistration Information", "You have not been Unregisterd successfully" };
        //private static string[] Users = { "noBody", "name=00a3a6f5d8a842114809baffbeff80ed", "word=003a224148da6adb551201e7cf71031e 00a667d0545699cb0ca7f5db1e601d9e" };
        private static string[] Users = { };
        private static string UsersName = "";
        //private static string ResigtrationURL = "https://192.168.26.1:442/tr_auth_cnf.html?command=Register&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e 00a667d0545699cb0ca7f5db1e601d9e";
        //private static string ResigtrationURL = "https://192.168.26.1:442/kta.cgi?command=Register&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e%2000a667d0545699cb0ca7f5db1e601d9e";
        private static string ResigtrationURL = "https://192.168.26.1:442/api/v3/auth/command?command=Register&name=shahraki&word=shahraki=401984";
        //private static string InformationURL = "https://192.168.26.1:442/tr_auth_cnf.html?command=View Account&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e 00a667d0545699cb0ca7f5db1e601d9e";
        //private static string InformationURL = "https://192.168.26.1:442/kta.cgi?command=View%20Account&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e%2000a667d0545699cb0ca7f5db1e601d9e";
        private static string InformationURL = "https://192.168.26.1:442/kta.cgi?command=View%20Account&name=shahraki&word=shahraki=401984";
        //private static string UnresigtrationURL = "https://192.168.26.1:442/tr_auth_cnf.html?command=Unregister&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e 00a667d0545699cb0ca7f5db1e601d9e";
        //private static string UnresigtrationURL = "https://192.168.26.1:442/kta.cgi?command=Unregister&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e%2000a667d0545699cb0ca7f5db1e601d9e";
        private static string UnresigtrationURL = "https://192.168.26.1:442/api/v3/auth/command?command=Unregister&name=shahraki&word=shahraki=401984";

        //public static string[] QuotaInformationMessages => quotaInformationMessages;

        public Form1()
        {
            InitializeComponent();
            DoWorkAsyncInfiniteLoop();
        }

        private async Task<double> ReceiveInternetQuota()
        {
            var handler = new HttpClientHandler();
            handler.UseProxy = false;
            HttpClient client = new HttpClient(handler);
            //HttpClient client = new HttpClient();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                string findQuotaString = "var TrafficQuota = new Array([\"Days\",\"Total\",\"1\",\"1024 mbytes\",\"";
                int lengthOfFindQuotaString = findQuotaString.Length;
                char[] quotac = new char[6];
                var response = await client.GetStringAsync(InformationURL);
                //int Tindex = response.IndexOf("var TrafficQuota = new Array([\"Days\",\"Total\",\"1\",\"300 mbytes\",\"");
                //int Tindex = response.IndexOf("var TrafficQuota = new Array([\"Days");
                int Tindex = response.IndexOf(findQuotaString) + lengthOfFindQuotaString;
                response.CopyTo(Tindex , quotac, 0, 6);
                string quotas = new string(quotac);
                double quota = new double();
                //quota = Convert.ToDouble(quotas);
                quota = Convert.ToDouble(System.Text.RegularExpressions.Regex.Match(quotas, @"[-+]?([0-9]*\.[0-9]+|[0-9]+)").Value);
                quotaLabel.Text = quota.ToString(); 
                label_username.Text = " of " + UsersName;
                return quota;
                /*if (quota < 500)
                {
                    notifyIcon1.ShowBalloonTip(2000, "Out of Quota", "The Quota is less than " + quotas, ToolTipIcon.Info);
                }*/
            }
            catch (Exception ee)
            {
                //MessageBox.Show(ee.ToString());
                return 0;
            }
            /*finally
            {
                await client.DeleteAsync("https://192.168.26.1:442/tr_auth_cnf.html?command=View Account&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e 00a667d0545699cb0ca7f5db1e601d9e");
                handler.UseProxy = true;
            }*/

        }

        static async Task<string> RegisterInternet()
        {
            var handler = new HttpClientHandler();
            handler.UseProxy = false;
            HttpClient client = new HttpClient(handler);

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            //HttpClient client = new HttpClient();
            try
            {
                //handler.UseProxy = false;
                var response = await client.GetStringAsync(ResigtrationURL);
                if (response.Contains("Register/Unregister was accomplished successfully"))
                    return "yes";
                else
                    return "no";
            }
            catch (Exception)
            {
                //MessageBox.Show(ee.ToString());
                return "no";
            }

        }

        static async Task<string> UnRegisterInternet()
        {
            var handler = new HttpClientHandler();
            handler.UseProxy = false;
            HttpClient client = new HttpClient(handler);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            //HttpClient client = new HttpClient();
            try
            {
                //handler.UseProxy = false;
                var response = await client.GetStringAsync(UnresigtrationURL);
                if (response.Contains("Register/Unregister was accomplished successfully"))
                    return "yes";
                else
                    return "no";
            }
            catch (Exception)
            {
                //MessageBox.Show(ee.ToString());
                return "no";
            }

        }

        private async Task DoWorkAsyncInfiniteLoop()
        {
            while (true)
            {
                // do the work in the loop
                double quota = await ReceiveInternetQuota();
                //quotaLabel.Text = quota.ToString();
                // update the UI
                if (quota == 0)
                    notifyIcon1.ShowBalloonTip(2000, quotaEqualZeroMessages[0], quotaEqualZeroMessages[1] + quota.ToString(), ToolTipIcon.Error);
                else if (quota < LimitValume)
                    notifyIcon1.ShowBalloonTip(2000, quotaInformationMessages[0], quotaInformationMessages[1] + quota.ToString(), ToolTipIcon.Warning);
                // don't run again for at least TimePeriod * 60 * 1000 milliseconds
                await Task.Delay(TimePeriod * 60 * 1000);
            }
        }

        private async void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double quota = await ReceiveInternetQuota();
            if (quota == 0)
                notifyIcon1.ShowBalloonTip(2000, quotaEqualZeroMessages[0], quotaEqualZeroMessages[1] + quota.ToString(), ToolTipIcon.Error);
            else if (quota < LimitValume)
                notifyIcon1.ShowBalloonTip(2000, quotaOutOfLimitMessages[0], quotaOutOfLimitMessages[1] + quota.ToString(), ToolTipIcon.Warning);
            else
                notifyIcon1.ShowBalloonTip(2000, quotaInformationMessages[0], quotaInformationMessages[1] + quota.ToString(), ToolTipIcon.Info);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void RegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //double quota = await registerInternet();
            //string isRegisterd = await registerInternet();
            await ReceiveInternetQuota();
            if (await RegisterInternet() == "yes")
                notifyIcon1.ShowBalloonTip(2000, InternetRegistrationSuccessMessages[0], InternetRegistrationSuccessMessages[1], ToolTipIcon.Info);
            else
                notifyIcon1.ShowBalloonTip(2000, InternetRegistrationUnsuccessMessages[0], InternetRegistrationUnsuccessMessages[1], ToolTipIcon.Error);
        }

        private async void UnregisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ReceiveInternetQuota();
            if (await UnRegisterInternet() == "yes")
                notifyIcon1.ShowBalloonTip(2000, InternetUnRegistrationSuccessMessages[0], InternetUnRegistrationSuccessMessages[1], ToolTipIcon.Info);
            else
                notifyIcon1.ShowBalloonTip(2000, InternetUnegistrationUnsuccessMessages[0], InternetUnegistrationUnsuccessMessages[1], ToolTipIcon.Error);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ///locating right screen to show
            int x = Screen.PrimaryScreen.Bounds.Right;
            //var screen = Screen.AllScreens;
            foreach (var screen in Screen.AllScreens)
            {
                (contextMenuStrip1.Items[4] as ToolStripMenuItem).DropDownItems.Add(screen.DeviceName);
            }
            (contextMenuStrip1.Items[4] as ToolStripMenuItem).DropDownItemClicked += Screen_DropDownItemClicked;
            //x = screen[1].Bounds.Left;
            this.Location = new Point(x, 0);

            ///locating right user to use to
            //string Dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string filenamelocation = Environment.CurrentDirectory + @"\credentials.txt";
            //string filenamelocation = Path.Combine(Dir, "commitMessageTemplate.txt");
            if (File.Exists(filenamelocation))
            {
                Users = File.ReadAllLines(filenamelocation, Encoding.UTF8);
                UsersName = Users[0];
                for (int i = 0; i < Users.Length; i = i + 3)
                {
                    string User = Users[i];
                    (contextMenuStrip1.Items[3] as ToolStripMenuItem).DropDownItems.Add(User);
                }
                (contextMenuStrip1.Items[3] as ToolStripMenuItem).DropDownItemClicked += Users_DropDownItemClicked;
            }
            else
            {
                (contextMenuStrip1.Items[3] as ToolStripMenuItem).DropDownItems.Add("Default User");
            }
             
        }

        private void Users_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string User = e.ClickedItem.ToString();
            UsersName = User;
            //ResigtrationURL = "https://192.168.26.1:442/tr_auth_cnf.html?command=Register&name=00a3a6f5d8a842114809baffbeff80ed&word=003a224148da6adb551201e7cf71031e 00a667d0545699cb0ca7f5db1e601d9e";
            ResigtrationURL = string.Concat("https://192.168.26.1:442/api/v3/auth/command?command=Register&", Users[Array.IndexOf(Users, User) + 1], "&", Users[Array.IndexOf(Users, User) + 2]);
            UnresigtrationURL = string.Concat("https://192.168.26.1:442/api/v3/auth/command?command=Unregister&", Users[Array.IndexOf(Users, User) + 1], "&", Users[Array.IndexOf(Users, User) + 2]);
            InformationURL = string.Concat("https://192.168.26.1:442/kta.cgi?command=View%20Account&", Users[Array.IndexOf(Users, User) + 1], "&", Users[Array.IndexOf(Users, User) + 2]);
        }

        private void Screen_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ScreenName = e.ClickedItem.ToString();
            //var screen = Screen.AllScreens;
            int x = new int();
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.DeviceName == e.ClickedItem.ToString())
                {
                    x = screen.Bounds.Left;
                    this.Location = new Point(x, 0);
                    this.Focus();
                    break;
                }
            }

        }

    }
}
