using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace waRunSQLWorkLoad
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string sFile = Application.StartupPath.ToString() + "\\AdventureWorksWorkload_DMC.sql";
            if (System.IO.File.Exists(sFile))
            {
                txtWrk.Text = sFile;
            }

            txtStrCnn.Text = @"Server=COMPASS\SQLEXPRESS;Database=AdventureWorks2014;Trusted_Connection=yes";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int iNumThreads = (int)numThreads.Value;
            Cursor.Current = Cursors.WaitCursor; 
            try
            {
                CRunWorkload[] r = new CRunWorkload[iNumThreads];
                Thread[] t = new Thread[iNumThreads];
                List<string> sComandos;
                string sBaseLog = Application.StartupPath.ToString() + "\\Log";
                string sFileLog;

                sComandos = ReadWorkloadLines(txtWrk.Text);
                for (int i = 0; i < iNumThreads; i++)
                {
                    lstSaida.Items.Add("Iniciando thread " + i.ToString());
                    
                    //arquivo de log
                    sFileLog = sBaseLog + i.ToString() + ".txt";

                    // Deleta o arquivo de log caso exista
                    System.IO.File.Delete(sFileLog);

                    //cria e starta a thread
                    r[i] = new CRunWorkload(txtStrCnn.Text, sComandos, sBaseLog + i.ToString()+".txt");                    
                    t[i] = new Thread(new ThreadStart(r[i].Run));
                    t[i].Start();                        
                }

                for (int i = 0; i < iNumThreads; i++)
                {
                    t[i].Join();
                    lstSaida.Items.Add("Thread " + i.ToString()+ " " + r[i].Resultado);
                }

            }
            catch (Exception x)
            {
                lstSaida.Items.Add("Erro:" + x.Message);
            }
            Cursor.Current = Cursors.Default ; 
        }
        private List<string> ReadWorkloadLines(string sFile)
        {
            List<string> liCmds = new List<string>();

            string s, sCMD = "";

            System.IO.StreamReader r = new System.IO.StreamReader(sFile);
            while (r.EndOfStream == false )
            {
                s = r.ReadLine();
                
                if (s.Trim().ToLower().Equals("go"))
                {
                    liCmds.Add(sCMD);
                    s = "";
                    sCMD = "";
                }
                sCMD = sCMD + "\n" + s;
            }
            //s = r.ReadToEnd();
            r.Close();
            return liCmds ;
        }

    }
}
