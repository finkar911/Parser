using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Http_Parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //List<string> code = new List<string>();
        int divonstr = 12;
        string code;
        //public string[] newpage;
        public int sinh = 0;
        public int turn = 1;
        //public List<string> filter = new List<string>();
        string[] filter=File.ReadAllLines(@"filter.txt");
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < filter.Length; i++) {
                listBox2.Items.Add(filter[i]);
            }
            //int progbar = 0;
            // filter = 
            turn = 1;
            button1.Enabled = false;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
            timer1.Enabled = true;
            //newpage = new string[Convert.ToInt32(numericUpDown3.Value)];
            progressBar1.Value = 0;
            progressBar1.Maximum = divonstr * Convert.ToInt32(numericUpDown3.Value)+(Convert.ToInt32(numericUpDown3.Value));
            label3.Text = "Нужно проанализировать "+ progressBar1.Maximum.ToString() + " пунктов";
            code = "<html><body>";
            sinh = Convert.ToInt32(numericUpDown1.Value);
            //parser();

            for (int ii = Convert.ToInt32(numericUpDown1.Value); ii < Convert.ToInt32(numericUpDown1.Value)+Convert.ToInt32(numericUpDown3.Value); ii++)
            {
                Thread myThread = new Thread(new ParameterizedThreadStart(trhreadParser));
                myThread.Start(ii);
            }

            //code += "</body></html>";
            //string Date = DateTime.Now.ToString();
            //Date= Date.Replace(":", " ");
            //File.WriteAllText(Date+".html", code, Encoding.Default);

        }

            void trhreadParser(object ia) {
            string htmtoout="";
            int ii = (int)ia;
            var webClient = new System.Net.WebClient();
            string HTML = webClient.DownloadString("https://www.indiegala.com/giveaways/" + ii + "/expiry/asc/level/0");
            HTML = HTML.Substring(HTML.IndexOf("row tickets-row"), HTML.IndexOf("</section>", HTML.IndexOf("row tickets-row")) - HTML.IndexOf("row tickets-row"));

            //this.BeginInvoke(new Action(delegate ()
            //{
            //    listBox1.Items.Add("Поток "+ii+" закончил загрузку страницы");
               
            //}));
           

            int fi = 0;
            int li = 0;
            int fip = 0;
            int lip = 0;
            for (int i = 0; i < divonstr; i++)
            {
                //this.BeginInvoke(new Action(delegate ()
                //{
                //    progressBar1.Value = progressBar1.Value + 1;

                //}));
                //progressBar1.Invoke(progressBar1.Value++);
                int est = 0;
               

                fi = HTML.IndexOf("<h2>", fi + 5);
                li = HTML.IndexOf("</h2>", li + 5)+5 ;//+5
                fip = HTML.IndexOf("<strong>", fip + 5);
                lip = HTML.IndexOf("</strong>", lip + 5) + 5;
                string add = HTML.Substring(fi, li - fi);
                for (int iai = 0; iai < filter.Length; iai++)
                {
                    if (add.Contains(filter[iai]))
                    {
                        est = 1;
                    }
                }
                if (est==0){
                    string firspart = add.Substring(4, add.IndexOf("<a href=") + 5);
                    string lastpart = "https://www.indiegala.com" + add.Substring(add.IndexOf("<a href=") + 9,add.IndexOf("</h2>")- (add.IndexOf("<a href=")+9));//+9

                    //fip = HTML.IndexOf("<strong>", fip + 5);
                    //lip = HTML.IndexOf("</strong>", lip + 5) + 5;
                    string ip = HTML.Substring(fip + 8, lip - (fip + 8)) + "<br><br>";

                    

                    htmtoout += firspart + lastpart + " IP ="+ip;
                   // break;
                }
            }
            this.BeginInvoke(new Action(delegate ()
            {
                progressBar1.Value = progressBar1.Value + 6;
                listBox1.Items.Add("Поток " + ii + " закончил анализ страницы и ждет своей очереди для записи");

            }));
            for (int i = 0; i < 50; i--)
            {
                if (ii == sinh)
                {
                    this.BeginInvoke(new Action(delegate ()
                    {
                        //listBox1.Items.Add(htmtoout);
                        code += htmtoout;
                    }));
                    int change = ii + 1;
                    int ch = turn + 1;
                    Interlocked.Exchange(ref sinh, change);
                    Interlocked.Exchange(ref turn, ch);
                    this.BeginInvoke(new Action(delegate ()
                    {
                        listBox1.Items.Add("Поток " + ii + " закончил работу.");

                        progressBar1.Value = progressBar1.Value + 7;
                        
                    }));
                    i = 55;
                    break;
                    
                }
                else {
                    Thread.Sleep(500);
                }
            }
            

        }
        //static void threadout(out List<string> newpage) {

        //    //newpage = "11";
        //}

        public void parser(){
            var webClient = new System.Net.WebClient();
            for (int ii = Convert.ToInt32(numericUpDown1.Value); ii <= Convert.ToInt32(numericUpDown3.Value); ii++)
            {
                string HTML = webClient.DownloadString("https://www.indiegala.com/giveaways/" + ii + "/expiry/asc/level/" + numericUpDown2.Value.ToString());
                HTML = HTML.Substring(HTML.IndexOf("row tickets-row"), HTML.IndexOf("</section>", HTML.IndexOf("row tickets-row")) - HTML.IndexOf("row tickets-row"));
                int fi = 0;
                int li = 0;
                for (int i = 0; i < 12; i++)
                {
                    progressBar1.Value++;
                    fi = HTML.IndexOf("<h2>", fi + 5);
                    li = HTML.IndexOf("</h2>", li + 5) + 5;
                    string add = HTML.Substring(fi, li - fi);
                    if (!add.Contains("Savage Resurrection"))
                    {
                        string firspart = add.Substring(0, add.IndexOf("<a href=") + 9);
                        string lastpart = "https://www.indiegala.com" + add.Substring(add.IndexOf("<a href=") + 9);
                        code += firspart + lastpart;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //progressBar1.Value = (turn - 1) * divonstr;
            label3.Text = "Обработанно "+(turn - 1).ToString()+" страниц.";
            if (turn > Convert.ToInt32(numericUpDown3.Value)) {
                code += "</body></html>";
                string Date = DateTime.Now.ToString();
                Date = Date.Replace(":", " ");
                File.WriteAllText(Date + ".html", code, Encoding.Default);
                timer1.Enabled = false;
                listBox1.Items.Add("Обработка завершена.");
                button1.Enabled = true;
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
            }
        }
    }
}
