using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InventoryModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationCases = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        ///////////// INPUTS /////////////

        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int NumberOfDays { get; set; }
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        ///////////// OUTPUTS /////////////

        public List<SimulationCase> SimulationCases { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }
        static int stat = 0;
        public void Fun(StreamReader sr, List<Distribution> DemandDistribution)
        {
            int Tcom = 0;
            while (sr.Peek() != -1)
            {
                string l = sr.ReadLine();
                if (l == "")
                {
                    break;
                }
                //read the dimand distribution and Lead Days Distribution 
                //with comulative prop

                string[] dist = l.Split(',');
                Distribution row = new Distribution();
                row.Value = int.Parse(dist[0]);
                int pro = 0;
                if (stat % 2 == 0)
                    pro = Convert.ToInt32(float.Parse(dist[1]) * 100);
                else
                    pro = Convert.ToInt32(float.Parse(dist[1]) * 100);
                row.Probability = pro;
                row.CummProbability = Tcom + pro;
                row.MinRange = Tcom + 1;
                Tcom += pro;
                row.MaxRange = Tcom;
                DemandDistribution.Add(row);
            }
            stat++;

        }
        public void readfile(string str)
        {
            FileStream fs = new FileStream(str, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek() != -1)
            {
                string l = sr.ReadLine();
                if (l == "")
                {
                    continue;
                }
                else if (l.Equals("OrderUpTo"))
                {
                    OrderUpTo = int.Parse(sr.ReadLine());
                }
                else if (l.Equals("ReviewPeriod"))
                {
                    ReviewPeriod = int.Parse(sr.ReadLine());
                }
                else if (l.Equals("StartInventoryQuantity"))
                {
                    StartInventoryQuantity = int.Parse(sr.ReadLine());
                }
                else if (l.Equals("StartLeadDays"))
                {
                    StartLeadDays = int.Parse(sr.ReadLine());
                }
                else if (l.Equals("StartOrderQuantity"))
                {
                    StartOrderQuantity = int.Parse(sr.ReadLine());
                }
                else if (l.Equals("NumberOfDays"))
                {
                    NumberOfDays = int.Parse(sr.ReadLine());
                }
                else if (l.Equals("DemandDistribution"))
                {

                    Fun(sr, DemandDistribution);
                }
                else if (l.Equals("LeadDaysDistribution"))
                {
                    Fun(sr, LeadDaysDistribution);
                }
            }
            fs.Close();

        }
        public void sim()
        {
            int ncycle = NumberOfDays / ReviewPeriod;
            Random rnd = new Random();
            int EndingInventoryAverage = 0;
            int ShortageQuantityAverage = 0;
            int shortage = 0;
            for (int i=0; i<NumberOfDays; i++)
            {
                SimulationCase row = new SimulationCase();
                row.Day = i+1;
                row.Cycle = i / ReviewPeriod+1; //calculate the cycle
                row.DayWithinCycle = i % ReviewPeriod+1;
                row.BeginningInventory = StartInventoryQuantity;
                row.RandomDemand = rnd.Next(1, 101);
                row.Demand = get_dist(row.RandomDemand, DemandDistribution);

                row.EndingInventory = row.BeginningInventory - row.Demand - shortage;
                shortage = 0;
                if (row.EndingInventory < 0)
                {
                    row.ShortageQuantity =- row.EndingInventory;
                    row.EndingInventory = 0;
                    shortage = row.ShortageQuantity;

                }
                StartInventoryQuantity = row.EndingInventory;

                if(row.DayWithinCycle==StartLeadDays)
                {
                    StartInventoryQuantity += StartOrderQuantity;
                }
                if (row.DayWithinCycle == ReviewPeriod)
                {
                    row.OrderQuantity = OrderUpTo - row.EndingInventory + row.ShortageQuantity;
                    StartOrderQuantity = row.OrderQuantity;
                    row.RandomLeadDays = rnd.Next(1, 101);
                    row.LeadDays = get_dist(row.RandomLeadDays, LeadDaysDistribution);
                    StartLeadDays = row.LeadDays;
                }
                SimulationCases.Add(row);


                EndingInventoryAverage += row.EndingInventory;
                ShortageQuantityAverage += row.ShortageQuantity;

            }
            
            PerformanceMeasures.EndingInventoryAverage = (EndingInventoryAverage / (decimal)NumberOfDays);
            PerformanceMeasures.ShortageQuantityAverage = (ShortageQuantityAverage / (decimal)NumberOfDays);
        }
        public int get_dist(int rnd, List<Distribution> dis)
        {
            for (int i = 0; i < dis.Count; i++)
                if (rnd <= dis[i].MaxRange)
                    return dis[i].Value;
            return 0;
        }
    }
}
