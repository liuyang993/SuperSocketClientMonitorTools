using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;

namespace SuperSocketClientTest
{

    public partial class Form1 : Form
    {
        //Action<string> messageTarget;
        EasyClient client;

        public void ShowWindowsMessage(string sHead, string sReply)
        {
            if (sHead == "GETCLIENTSTATE")
            {
                List<Clients> source1 = JsonConvert.DeserializeObject<List<Clients>>(sReply);

                //VideoSource s = new VideoSource();
                //s.origin = source.origin;
                //s.friendlyNameLong = source.friendlyNameLong;
                //s.friendlyNameShort = source.friendlyNameShort;
                //s.state = source.state;

                var Dsource = new BindingSource();

                Dsource.DataSource = source1;
                //dataGridView1.DataSource = Dsource;
                dataGridView1.Invoke((Action)(() => dataGridView1.DataSource = Dsource));

 
            
            }
            else if (sHead == "NORMALLOG")
            {

                //int i = 1;
                listBox1.Invoke((Action)(() => listBox1.Items.Add(sReply)));

                //listBox1.Invoke((Action)(() => listBox1.Items.Add("Total is " + listBox1.Items.Count.ToString())));


            }

            return;

        }

        //messageTarget = ShowWindowsMessage;

        public Form1()
        {
            InitializeComponent();

            //class clientState
            //{
            //    public DateTime clientConnectTime { get; set; }
            //    public int clientRecv { get; set; }
            //    public int clienthandle { get; set; }
            //    public string clientIP { get; set; }
            //}

            //List<clientState> clientList = new List<clientState>();

            //    int i = 0;
            //for(;i<5;i++)
            //{
            //    clientState cst = new clientState { clientConnectTime = DateTime.Now, clientRecv = 1, clienthandle = 2, clientIP = "127.0.0.1" };
            //    clientList.Add(cst);
            //}
            //var sReplyT = JsonConvert.SerializeObject(cst);
            //var station = new { Name = "Duren Kalibata", LastTemperature = 45, MostRecentDate = DateTime.Now };
            //        var str = JsonConvert.SerializeObject(station);
            //        var source = JsonConvert.DeserializeObject<users>(str);


            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            foreach (TabPage tab in tabControl1.TabPages)
            {
                tab.Text = "";
            }

            var result = AccessTheWebAsync();
   
        }

        private async Task<int> AccessTheWebAsync()
        {
             client = new EasyClient();     

            client.Initialize(new MyReceiveFilter(),(request) =>
            {
                // handle the received request
                ShowWindowsMessage(request.Key,request.Body);
            });

            
            
            var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2020));

            if (connected)
            {

                Thread.Sleep(3000);
                // Send data to the server

                //int iResult;
                //char request[23];

                //memcpy(request, "<cmd>GETALLCLIENT</cmd>", 23);
                //iResult = send(m_Socket, request, 23, 0);

                string sReply = "GETALLCLIENT";
                byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                byte[] bData = Encoding.ASCII.GetBytes(sReply);


                byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                var str = System.Text.Encoding.Default.GetString(rv);


 
                client.Send(rv);
//                client.Send(Encoding.ASCII.GetBytes("LOGIN kerry"));
            }
            //client.DataReceived += client_DataReceived;

            return 0;

        }

        //private void client_DataReceived(object sender, DataEventArgs e)
        //{
        //    string msg = Encoding.Default.GetString(e.Data);
        //    Console.WriteLine(msg);
        //}



        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("aa");
            tabControl1.SelectedIndex = 0;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0)
                return;


            if (listBox1.Items[e.Index].ToString().Contains("error"))
            {
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(),
                e.Font, Brushes.Red, e.Bounds);
            }
            else if (listBox1.Items[e.Index].ToString().Contains("warn"))
            {
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(),
                    e.Font, Brushes.Red, e.Bounds);

            }
            else
            {
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                        e.Font, Brushes.Black, e.Bounds);
            }



        }

        private void closeThisSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iRow = dataGridView1.CurrentRow.Index;
            if (iRow < 0)
                return;

            string sID = dataGridView1.Rows[iRow].Cells["sessionID"].Value.ToString();
            //MessageBox.Show(sID);


            if (client.IsConnected)
            {

                string sReply = @"CLOSESESSION:" + sID;
                byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                byte[] bData = Encoding.ASCII.GetBytes(sReply);


                byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                var str = System.Text.Encoding.Default.GetString(rv);

                client.Send(rv);

                listBox1.Items.Add("send 'CLOSESESSION' to server");
                dataGridView1.Rows.Remove(dataGridView1.Rows[iRow]);

                return;
            }
            else
            {
                var connected = client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2020));
                if (client.IsConnected)
                {
                    string sReply = @"CLOSESESSION:" + sID;
                    byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                    byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                    byte[] bData = Encoding.ASCII.GetBytes(sReply);


                    byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                    System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                    System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                    System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                    var str = System.Text.Encoding.Default.GetString(rv);

                    client.Send(rv);
                    listBox1.Items.Add("send 'CLOSESESSION' to server");
                    dataGridView1.Rows.Remove(dataGridView1.Rows[iRow]);

                    return;
                }
            }

            //listBox1.BackColor = Color.Red;

            listBox1.Items.Add("error : send 'CLOSESESSION' to server fail , not connected");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //托盘显示图标等于托盘图标对象 
                //注意notifyIcon1是控件的名字而不是对象的名字 
               //notifyIcon1.Icon = ico;
                //隐藏任务栏区图标 
                this.ShowInTaskbar = false;
                
                //图标显示在托盘区 
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            //this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                string sTrySend = textBox1.Text;
                byte[] bData = Encoding.ASCII.GetBytes(sTrySend);

                client.Send(bData);
                listBox1.Items.Add("send " + sTrySend + " to server");
            }
            else
            {
                listBox1.Items.Add("error : send test cmd to server fail , not connected");
            }

        }

        private void viewClientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {

                string sReply = "GETALLCLIENT";
                byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                byte[] bData = Encoding.ASCII.GetBytes(sReply);


                byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                var str = System.Text.Encoding.Default.GetString(rv);

                client.Send(rv);

                listBox1.Items.Add("send 'GETALLCLIENT' to server");
                return;
            }
            else
            {
                var connected = client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2020));
                if (client.IsConnected)
                {
                    string sReply = "GETALLCLIENT";
                    byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                    byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                    byte[] bData = Encoding.ASCII.GetBytes(sReply);


                    byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                    System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                    System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                    System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                    var str = System.Text.Encoding.Default.GetString(rv);

                    client.Send(rv);
                    listBox1.Items.Add("send 'GETALLCLIENT' to server");
                    return;
                }
            }

            //listBox1.BackColor = Color.Red;


            listBox1.Items.Add("error : send 'GETALLCLIENT' to server fail , not connected");

            //var result = AccessTheWebAsync();


            // Initialize the client with the receive filter and request handler
            //client.Initialize(new MyReceiveFilter(), (request) => {
            //    // handle the received request
            //    Console.WriteLine(request.Key);
            //});
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                string sReply = @"SETDEBUGLEVEL:0,1";
                byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                byte[] bData = Encoding.ASCII.GetBytes(sReply);


                byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                var str = System.Text.Encoding.Default.GetString(rv);



                client.Send(rv);
                listBox1.Items.Add("send 'SETDEBUGLEVEL' to server");
            }
            else
            {
                listBox1.Items.Add("error : send 'SETDEBUGLEVEL' to server fail , not connected");
            }
        }

        private void onlyErrorAndWarningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                string sReply = @"SETDEBUGLEVEL:3,1";
                byte[] bHead = Encoding.ASCII.GetBytes(@"<cmd>");
                byte[] bTail = Encoding.ASCII.GetBytes(@"</cmd>");
                byte[] bData = Encoding.ASCII.GetBytes(sReply);


                byte[] rv = new byte[bHead.Length + bTail.Length + bData.Length];
                System.Buffer.BlockCopy(bHead, 0, rv, 0, bHead.Length);
                System.Buffer.BlockCopy(bData, 0, rv, bHead.Length, bData.Length);
                System.Buffer.BlockCopy(bTail, 0, rv, bHead.Length + bData.Length, bTail.Length);

                var str = System.Text.Encoding.Default.GetString(rv);



                client.Send(rv);
                listBox1.Items.Add("send 'SETDEBUGLEVEL' to server");
            }
            else
            {
                listBox1.Items.Add("error : send 'SETDEBUGLEVEL' to server fail , not connected");
            }
        }
    }

    public class Clients
    {
        /// <summary>
        /// A User's username. eg: "sergiotapia, mrkibbles, matumbo"
        /// </summary>
        [JsonProperty("clientConnectTime")]
        public DateTime clientConnectTime { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("clientRecv")]
        public int clientRecv { get; set; }

        /// <summary>
        /// A User's location. eh: "Bolivia, USA, France, Italy"
        /// </summary>
        [JsonProperty("clienthandle")]
        public int clienthandle { get; set; }

        [JsonProperty("clientIP")]
        public string clientIP { get; set; } //Todo.

        [JsonProperty("sessionID")]
        public string sessionID { get; set; } //Todo.

    }


}
