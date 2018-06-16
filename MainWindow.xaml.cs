using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PUBGAuto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Process.GetProcessesByName("PUBGAuto").Length > 1)
            {
                MessageBox.Show("Application is exiting...... Please try again after 60 second!!");
                Environment.Exit(1);
            }
        }

        string gamePath = @".\Steam.exe";
        String ipAddrServer = "118.200.75.51";
        //String ipAddrServer = "127.0.0.1";
        //string gamePath = @"d:\Games\Steam\Steam.exe";


        string username = "";
        Process startproc = new Process();

        public void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.

                Int32 port = 13001;
                TcpClient client = new TcpClient(server, port);


                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);


                //Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                if (responseData != "NoID" && responseData != "")
                {
                    username = responseData.Split(' ')[0];
                    steamStart(responseData);

                }
                else if (responseData == "NoID")
                {
                    MessageBox.Show("No more free ID, please contact our staff!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Unknow error!");
                    this.Close();
                }

                stream.Close();
                client.Close();

                //}
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(e.ToString());
                this.Close();
                Environment.Exit(1);
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString());
                this.Close();
                Environment.Exit(1);
            }

        }

        public void steamStart(string responseData)
        {
            string arg = "-language en -login " + responseData;

            startproc.StartInfo.FileName = gamePath;
            startproc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            startproc.StartInfo.Arguments = arg;
            startproc.Start();

        }

        public void Beating(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                //Int32 port = 13000;
                Int32 port = 13001;
                TcpClient client = new TcpClient(server, port);



                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);


                //Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                // Close everything.

                stream.Close();
                client.Close();

            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(e.ToString());
                this.Close();
                Environment.Exit(1);
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString());
                this.Close();
                Environment.Exit(1);
            }

        }



        private async void btnCafe_Click(object sender, RoutedEventArgs e)
        {
            if (Process.GetProcessesByName("Steam").Length > 0)
            {
                this.Close();
                MessageBox.Show("Steam is already running, Please closed Steam and try again!!");
            }
            else
            {

                this.Hide();
                string pcname = System.Environment.MachineName;
                string externalip;
                try
                {
                    externalip = new WebClient().DownloadString("http://icanhazip.com");
                }
                catch (Exception err)
                {
                    externalip = new WebClient().DownloadString("https://api.ipify.org");
                }
                externalip = externalip.Trim('\n');
                if (externalip != null)
                    Connect(ipAddrServer, "PUBG" + " " + pcname + "," + externalip);
                else
                    MessageBox.Show("Gettingh public IP Address error");

                while (username != "")
                {

                    String beatingmsg = "Beating " + "PUBG" + "," + username;
                    Beating(ipAddrServer, beatingmsg);
                    if (Process.GetProcessesByName("Steam").Length <= 0)
                    {
                        break;
                    }
                    await Task.Delay(100000);
                    Process[] process = Process.GetProcessesByName("Steam");
                    process[0].EnableRaisingEvents = true;
                    process[0].Exited += Startproc_Exited;
                    //await Task.Delay(3000);
                    await Task.Delay(1000 * 60 * 15);

                }
                Environment.Exit(1);

            }
        }

        private void Startproc_Exited(object sender, EventArgs e)
        {
            Environment.Exit(1);
            throw new NotImplementedException();
        }

        private void btnPersonal_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                startproc.StartInfo.FileName = gamePath;
                startproc.Start();
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(Convert.ToString(err));
            }
        }
    }
}
