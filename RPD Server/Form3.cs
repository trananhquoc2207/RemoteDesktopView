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

namespace RPD_Server
{
    public partial class Form3 : Form
    {
        
        private readonly int port;
        IPEndPoint IP;
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientList = new List<Socket>();
        public Form3(int Port)
        {
            port = Port;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Socket item in clientList)
            {
                Send(item);

            }
            AddMessage(textBox1.Text);
            textBox1.Clear();
        }
        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Any, port);
            

            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Bind(IP);
                        server.Listen(20);
                        Socket client = server.Accept();
                        clientList.Add(client);
                        Thread recieve = new Thread(Recieve);
                        recieve.IsBackground = true;
                        recieve.Start(client);
                        string a = " ket noi thanh cong!";
                        AddMessage(a);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
            });
            listen.IsBackground = true;
            listen.Start();
        }
        void Close()
        {
            server.Close();
        }
        void Send(Socket client)
        {
            if (client != null && textBox1.Text != string.Empty)
                client.Send(Serialize(textBox1.Text));
        }
        void Recieve(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    client.Receive(data);
                    string message = (string)Deserialize(data);

                    foreach (Socket item in clientList)
                    {
                        if (item != null && item != client)
                            item.Send(Serialize(message));
                    }
                    AddMessage(message);
                }
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
            }
        }
        void AddMessage(string s)
        {
            listView1.Items.Add(new ListViewItem() { Text = s });

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
