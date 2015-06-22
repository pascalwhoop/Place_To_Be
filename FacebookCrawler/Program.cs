using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Service;

namespace FacebookCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            Program p = new Program();
            while (true) {
                p.facebookCrawlerInit();
                Thread.Sleep(1000*60*60*4); //wait 4 hours
            }

        }

       
        protected void runFacebookCrawler()
        {
            BackgroundWorker bw = new BackgroundWorker();

            // this allows our worker to report progress during work
            bw.WorkerReportsProgress = true;

            // what to do in the background thread
            bw.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                // run our crawler
                facebookCrawlerInit();

            });

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate(object o, RunWorkerCompletedEventArgs args)
            {
                Thread.Sleep(1000 * 3600 * 24);
                runFacebookCrawler();
            });

            bw.RunWorkerAsync();
        }

        protected void facebookCrawlerInit()
        {
            FbCrawler fbCrawler = new FbCrawler();

            City berlin = new City();
            berlin.name = "Berlin, Germany";
            berlin.polygon = new double[5, 2]{
                {52.6754542,13.0891553}, {52.6754542,13.7611176}, {52.339629599,13.7611176}, {52.339629599, 13.0891553}, {52.6754542,13.0891553}
            };
            fbCrawler.FindPagesForCities(berlin);

            /*City munich = new City();
            munich.name = "Munich, Germany";
            munich.polygon = new double[5,2]{
                {48.2482197,11.360796}, {48.2482197,11.7228755}, {48.0616018,11.7228755}, {48.0616018, 11.360796}, {48.2482197,11.360796}
            };
            fbCrawler.FindPagesForCities(berlin);

            City cologne = new City();
            cologne.name = "Cologne, Germany";
            cologne.polygon = new double[5, 2] {
                {51.08496299999999, 6.7725819}, {51.08496299999999, 7.1620628}, {50.8295269, 7.1620628},
                {50.8295269, 6.7725819}, {51.08496299999999, 6.7725819}
            };
            fbCrawler.FindPagesForCities(cologne);
            */

        }
    }
}
