using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace TCPIPCLIENT
{
    public partial class Form1 : Form
    {
        RSACryptoServiceProvider rsacrypt = new RSACryptoServiceProvider();
        int status = 0;
        string msg;
        string xmlstr;
        string signature;
        int length=1024;

        // 0 = waiting for input
        // 1 = waiting for public key
        // 2 = waiting for message
        // 3 = waiting for signed bytes


        public Form1()
        {
            InitializeComponent();
            startapp();
        }
        
        
        private void startapp()
        {
            Thread tcpservthread = new Thread(new ThreadStart(TcpServerRun));
            tcpservthread.Start();
        }
        private void TcpServerRun()
        {
            TcpListener tcplistener = new TcpListener(IPAddress.Any, 80);
            tcplistener.Start();
            updateui("listening");

            while(true)
            {
                TcpClient client = tcplistener.AcceptTcpClient();
                Thread tcphandlerThread = new Thread(new ParameterizedThreadStart(tcphandler));
                tcphandlerThread.Start(client);
            }
        }
        private void tcphandler(object client)
        {
            TcpClient mclient = (TcpClient)client;
            NetworkStream stream = mclient.GetStream();
            byte[] msg = new byte[length];
            stream.Read(msg, 0, msg.Length);
            handlebytes(msg);
            mclient.Close();
            stream.Close();
        }
        private void handlebytes(byte[] info)
        {
            string com = Encoding.ASCII.GetString(info);
            if (status == 0)
            {
                if (com.Contains("thisiskey"))
                {
                    updateui("Waiting for key");
                    com=com.Remove(0, "thisiskey".Length);
                    length = Convert.ToInt32(com);
                    status = 1;
                }
                else if (com.Contains("thisismsg"))
                {
                    updateui("Waiting for message");
                    com = com.Remove(0, "thisismsg".Length);
                    length = Convert.ToInt32(com);
                    status = 2;
                }
                else if(com.Contains("thisissignature"))
                {
                    updateui("Waiting for signature");
                    com = com.Remove(0, "thisissignature".Length);
                    length = Convert.ToInt32(com);
                    status = 3;
                }
                else
                {
                    updateui("new message: "+com);
                }
            }
            else if(status==1)
            {
                xmlstr = Encoding.UTF8.GetString(info);
                rsacrypt.FromXmlString(xmlstr);

                length = 1024;

                status = 0;
                updateui("key accepted");
            }
            else if (status == 2)
            {
                msg = Encoding.ASCII.GetString(info);

                length = 1024;

                status = 0;
                updateui("message accepted");
            }
            else if (status == 3)
            {
                updateui("Signature accepted");
                length = 1024;


                byte[] msgdata = Encoding.ASCII.GetBytes(msg);
                if((RSACLASS.verify(msgdata, xmlstr, info)))
                {
                    MessageBox.Show("success verifying");
                }
                else
                {
                    MessageBox.Show("unsuccessful verification");
                }


                status = 0;
            }
        }
        private void updateui(string s)
        {
            Func<int> del = delegate ()
            {
                textBox4.AppendText(s);
                textBox4.AppendText(Environment.NewLine);

                return 0;
            };
            Thread.Sleep(100);
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
        private void boxit(string s)
        {
            Func<int> del = delegate ()
            {
                MessageBox.Show(s);
                return 0;
            };
            Invoke(del);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            boxit(xmlstr);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            boxit(msg);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
