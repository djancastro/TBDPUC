using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics ;

namespace waRunSQLWorkLoad
{
    class CRunWorkload
    {
        private SqlConnection cnn;
        private List<string> sWorkLoadsCmds;
        private string sResultado;
        private string sLogFile;
        private Stopwatch swTimeElapsed;
        //stopWatch.Start();
        //Thread.Sleep(10000);
        public CRunWorkload(string sConnection, List<string> commands, string sParLogFile)
        {
            swTimeElapsed = new Stopwatch();
            swTimeElapsed.Start();
            
            cnn = new SqlConnection(sConnection);
            cnn.Open();
            sWorkLoadsCmds = commands;
            sLogFile = sParLogFile;

            swTimeElapsed.Stop();
        }

        private void LogTxt(string s)
        {   
            System.IO.StreamWriter swLog = new System.IO.StreamWriter(sLogFile, true);
            swLog.WriteLine(s);
            swLog.Close();            
        }

        public void Run()
        {
            swTimeElapsed.Start();
            int iNumeroFalhas = 0, iNumeroSucessos = 0;
            try
            {                
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                foreach (string s in sWorkLoadsCmds)
                {
                    cmd.CommandText = s;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        iNumeroSucessos++;
                    }
                    catch (Exception sqlErr)
                    {
                        iNumeroFalhas++;//loga    
                        LogTxt(sqlErr.Message + "\n" + s + new String('.', 60) + new String('.', 60) + "\n\n");
                    }
                }
                cnn.Close();
            }
            catch (Exception x) 
            {
                sResultado = x.ToString();
            }
            swTimeElapsed.Stop();
            sResultado = "Comandos completados com sucesso:" + iNumeroSucessos.ToString() 
                + " número de falhas: " + iNumeroFalhas.ToString()
                + " Tempo Decorrido (Segs):" + String.Format("{0:N2}", swTimeElapsed.Elapsed.TotalSeconds);
        }

        public string Resultado { get { return sResultado; } }
    }
}
