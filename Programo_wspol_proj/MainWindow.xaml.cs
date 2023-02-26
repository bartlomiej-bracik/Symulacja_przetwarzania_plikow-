using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Timers;
using System.Windows.Threading;

namespace Programo_wspol_proj
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class buforsData
        {
            clientData actClient;

            double neededTimeOfOpertion;
            double startTimeOfOper;
            int progress;
            public bool   isEmpty;
            public string textToShow;
            public buforsData(bool isEm)
            {
                this.isEmpty = isEm;
                this.textToShow = "Oczekiwaniw na klienta";
            }


            public void getNewClentFilles(clientData cD,double ctime)
            {
                this.actClient = cD;
                startTimeOfOper = ctime;


                float sumOfFill=0;
                for (int i = 0; i < cD.filesSize.Length; i++)
                {
                    sumOfFill = sumOfFill + cD.filesSize[i];
                }

                neededTimeOfOpertion = sumOfFill / 300;
                progress = 0;


            }

            public void findProgress(double cTime)
            {
                progress = (int) ( 100 * (cTime / (startTimeOfOper + neededTimeOfOpertion)));

                if (progress >= 100)
                    this.isEmpty = true;


            }

            public void setTextToShow()
            {
                if (isEmpty == true)
                {
                    textToShow = "Oczekiwaniw na klienta";

                }
                else
                {
                   if (actClient != null)
                    { 
                  this.textToShow = "id" + actClient.id + "[" + actClient.filesSize[0] + "," + actClient.filesSize[1] + ","
                        + actClient.filesSize[2] + "] \n Progres:" + progress + "%";
                  }

                }
            }

        }



       public class clientData
            {
            public int id;
            public int[] filesSize = new int[3];
            public int filSizeToAuction;
            public double timeOfOrder;
            public float weight;
           
            public clientData(int id, int[] filesSize, double timeOfOrder, float weight)
            {
                this.id = id;
                this.filesSize = filesSize;
                this.timeOfOrder = timeOfOrder;
               
                this.weight = weight;
            }

           

            public void findFileToAuction()
            {
                filSizeToAuction = filesSize[0];
                for(int i = 1;i <filesSize.Length;i++)
                {
                    if (filesSize[i] < filSizeToAuction)
                        filSizeToAuction = filesSize[i];
                }

            }

            public void setWeight(float w)
            {
                this.weight = w;
            }
        }


        buforsData[] bufors = new buforsData[5];
        List<clientData> clients = new List<clientData>();

        //Zmienne
        DateTime startTime;
        DateTime curentTime;
        private static Timer tm = new Timer();



        public MainWindow()
        {
            InitializeComponent();
            ShowList();
            startTime = DateTime.Now;


            //Create bufors
            for (int i = 0; i < 5; i++)
            {
                bufors[i] = new buforsData(true);
            }
            
           
       
            //  DispatcherTimer setup
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            timeText.Text = "Czas: " + ((int)getCurentTime()).ToString()+ "s";


            //Pobieranie do buforów
            getClientToBufor();

            //Operacje w buforach 

            // - przeliczenie progresu 
            //- ustawianie buforu jako pusty gdy osiągnie progres
            for (int i = 0; i < bufors.Length; i++)
            {
                bufors[i].findProgress(getCurentTime());            
            }
           //Pokaz klienta w buforze i progres
            ShowBuforState();
        }
       

     

        int inedexOfCl =0;
        private void Losuj_Button_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
             int[] filesSizeRandom = new int[3];

            filesSizeRandom[0] = rand.Next(1, 1024) ;
            filesSizeRandom[1] = rand.Next(1, 1024);
            filesSizeRandom[2] = rand.Next(1, 1024);

            int ctime = 10;
            //curentTime = DateTime.Now;
            //TimeSpan delta = curentTime.Subtract(startTime);
            double deltaInSec = getCurentTime();

            clientData exampleClient = new clientData(inedexOfCl, filesSizeRandom, deltaInSec, 0);
            clients.Add(exampleClient);
            inedexOfCl++;
            ShowList();
          //  ShowBuforState();


        }

        private void ShowList()
        { 
             int len = clients.Count;
            string tx="Lista oczekujących klientów: \nid                    Pliki                 t[s]      waga \n" ;

            for (int i = 0; i < len; i++)
            {
                string rowString = clients[i].id.ToString() + "  " + clients[i].filesSize[0].ToString() + "kB  " +
                clients[i].filesSize[1].ToString() + "kB  " + clients[i].filesSize[2].ToString() + "kB  " +
                 ((float)clients[i].timeOfOrder).ToString() + "  " + clients[i].weight.ToString() + "\n";

                tx = tx + rowString;

            }
            clientsText.Text = tx;
        }

        double getCurentTime()
        {
            curentTime = DateTime.Now;
            TimeSpan d = curentTime.Subtract(startTime);
            double dInSec = d.TotalSeconds;

            return dInSec;
        }

        void setNewWeight()
        {
            int l = clients.Count;
          //  clientData exCl = new clientData(0,[0, 1, 2], 1, 0);

            for (int i = 0; i < l; i++)
            {
                // clients[i].findFileToAuction();
                int[] fSizes = clients[i].filesSize;
               int  filSizeToAuction = fSizes[0];
               
                for (int j = 1; j < fSizes.Length; j++)
                {
                    if (fSizes[j] < filSizeToAuction)
                        filSizeToAuction = fSizes[j];
                }

                double t = getCurentTime() - clients[i].timeOfOrder;
                // Wzór
                float newWeihht = (200 * l / filSizeToAuction) + (float)(t) * l * 0.1f ;
                // float newWeihht = (100*l / filSizeToAuction) + (float) (getCurentTime()* getCurentTime()) * l * 0.05f ;
                // float newWeihht = (3 * l / filSizeToAuction) + (float)(getCurentTime() ) * l;
                // clients[i].setWeight(newWeihht);
                clients[i].weight = newWeihht;
                
            }
            
        }

        int findTheBestCl()
        {
            int l = clients.Count;
            int index_of_thBest = 0;
            float theBiggestWeight = -1;
            for (int i = 0; i < l; i++)
            {
                float w = clients[i].weight;
                if (w > theBiggestWeight)
                {
                    theBiggestWeight = w;
                    index_of_thBest = i;
                }


            }
            return index_of_thBest;
        }

        void getClientToBufor()
        {
            setNewWeight();

           
           
            // Sprwzanie buforów w petli 
            for (int z = 0; z < 5; z++)
            {
                if (bufors[z].isEmpty == true)
                {
                    int index = findTheBestCl();
                   
                    if (clients.Count > 0)
                    {
                        bufors[z].getNewClentFilles(clients[index], getCurentTime());
                        clients.RemoveAt(index);
                        bufors[z].isEmpty = false;
                        ShowList();
                    }
                    else
                    { 
                        
                    }

                }




            }
        }
        void ShowBuforState()
        {
            for (int i = 0; i < bufors.Length; i++)
            {
                bufors[i].setTextToShow();
            }  


            bufor1text.Text = bufors[0].textToShow;
            bufor2text.Text = bufors[1].textToShow;
            bufor3text.Text = bufors[2].textToShow;
            bufor4text.Text = bufors[3].textToShow;
            bufor5text.Text = bufors[4].textToShow;
        }



        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            //clients.RemoveAt(3);
            setNewWeight();
            ShowList();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getClientToBufor();
           ShowBuforState();


        }
    }
}
