using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPD
{
    public partial class chatclient : Form
    {
        private readonly int port;
        private readonly string diachiIp;
        IPEndPoint IP; 
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public chatclient(string DiachiIp,int Port)
        {
            port = Port;
            diachiIp = DiachiIp;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Send();
            AddMessage(textBox1.Text);
        }

        private void chatclient_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
        void Connect()
        {

            IP = new IPEndPoint(IPAddress.Parse(diachiIp), port);
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("khong the ke noi", "loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Thread listen = new Thread(Recieve);
            listen.IsBackground = true;
            listen.Start();
        }
        void Close()
        {
            client.Close();
        }
        void Send()
        {
            if (textBox1.Text != string.Empty)
                client.Send(Serialize(textBox1.Text));
        }
        void Recieve()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    client.Receive(data);
                    string message = (string)Deserialize(data);
                    AddMessage(message);
                }
            }
            catch
            {
                Close();
            }
        }
        void AddMessage(string s)
        {
            listView1.Items.Add(new ListViewItem() { Text = s });
            textBox1.Clear();
        }
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }
}
