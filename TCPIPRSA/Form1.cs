using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace TCPIPRSA
{
    public partial class Form1 : Form
    {
        TcpClient client;
        RSACryptoServiceProvider rsacrypt = new RSACryptoServiceProvider();
        string xmlstr;
        string msg;

        public Form1()
        {
            InitializeComponent();
        }


        private void sendkeys()
        {
            byte[] xmlbytes = Encoding.UTF8.GetBytes(rsacrypt.ToXmlString(false));
            xmlstr = rsacrypt.ToXmlString(false);

            byte[] thisiskey = Encoding.ASCII.GetBytes("thisiskey"+ xmlstr.Length.ToString());
            updateui("Sending preparation for key");

            Thread th1 = new Thread(new ParameterizedThreadStart(sendmsg));
            th1.Start(thisiskey);

            Thread.Sleep(500);

            updateui("Sending keys");
            Thread th2 = new Thread(new ParameterizedThreadStart(sendmsg));
            th2.Start(xmlbytes);
        }
        private void sendtext()
        {
            msg = textBox2.Text;

            byte[] thisiskey = Encoding.ASCII.GetBytes("thisismsg"+msg.Length.ToString());
            updateui("Sending preparation for text");

            Thread th1 = new Thread(new ParameterizedThreadStart(sendmsg));
            th1.Start(thisiskey);

            Thread.Sleep(500);

            updateui("Sending message: " + (msg));

            Thread th2 = new Thread(new ParameterizedThreadStart(sendmsg));
            th2.Start(Encoding.ASCII.GetBytes(msg));
        }
        private void sendsign()
        {

            byte[] tobesigned = Encoding.ASCII.GetBytes(textBox2.Text);
            byte[] signed = RSACLASS.Sign(tobesigned, rsacrypt.ToXmlString(true));
            string signedstr = Encoding.UTF8.GetString(signed);

            updateui("Sending preparation for signature");

            Thread th1 = new Thread(new ParameterizedThreadStart(sendmsg));
            th1.Start(Encoding.ASCII.GetBytes("thisissignature"+signed.Length));

            Thread.Sleep(500);

            Thread th2 = new Thread(new ParameterizedThreadStart(sendmsg));
            th2.Start(signed);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Thread th1 = new Thread(new ThreadStart(sendtext));
            th1.Start();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Thread th1 = new Thread(new ThreadStart(sendsign));
            th1.Start();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Thread th1 = new Thread(new ThreadStart(sendkeys));
            th1.Start();

        }
        private void boxit(string s)
        {
            Func<int> del = delegate ()
            {
                MessageBox.Show(s);
                return 0;
            };
            Invoke(del);
        }
        private void boxit2(string s, string ss)
        {
            Func<int> del = delegate ()
            {
                MessageBox.Show(s,ss);
                return 0;
            };
            Invoke(del);
        }
        private void button1_Click(object sender, EventArgs e)

        {
            rsacrypt.ImportParameters(RSACLASS.GenerateKeys());
            textBox3.AppendText("Keys generated" + Environment.NewLine);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string message = textBox2.Text;
            byte[] messagebytes = Encoding.ASCII.GetBytes(message);
            Thread th2 = new Thread(new ParameterizedThreadStart(sendmsg));
            th2.Start(messagebytes);
        }
        private void updateui(string s)
        {
            Func<int> del = delegate ()
            {
                textBox3.AppendText(s + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }
        private void sendmsg(object obj)
        {
            byte[] messagebytes = ((byte[])obj);

            client = new TcpClient();
            client.Connect(IPAddress.Parse(textBox1.Text), 80);
            NetworkStream stream = client.GetStream();
            stream.Write(messagebytes, 0, messagebytes.Length);
            stream.Close();
            client.Close();
        }
    }
}
