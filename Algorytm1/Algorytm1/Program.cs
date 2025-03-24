
namespace AlgorytmGenetyczny
{
    class Specimen
    {
        public static int paramNum, LBnP, chromLen;
        public static double minVal, maxVal;
        public string specimen { get; set; }
        public string x1 { get; set; }
        public string x2 { get; set; }
        public double x1Val { get; set; }
        public double x2Val { get; set; }
        public double? ocena { get; set; }
        
        public Specimen(string specimen)
        {
            this.specimen = specimen;
            x1 = specimen.Substring(0, LBnP);
            x2 = specimen.Substring(LBnP);
            x1Val = decode(x1);
            x2Val = decode(x2);
            ocena = null;
        }
        
        private double decode(string x)
        {
            double ZD = maxVal - minVal;
            double ctmp = 0;
            string xRev = new string(x.Reverse().ToArray());

            if (x.All(c => c == '0')) {return minVal;}
            if (x.All(c => c == '1')) {return maxVal;}
            
            for (int i = 0; i < x.Length; i++)
            {
                if (xRev[i] != '0')
                {
                    ctmp += Math.Pow(2, i);
                }
            }
            return Math.Round(minVal + ctmp/(Math.Pow(2, x.Length) - 1) * ZD, 2);
        }

        public void showStatics()
        {
            Console.WriteLine($"{minVal}, {maxVal}, {chromLen}, {paramNum}, {LBnP}");
        }
        
        public void showSpecimen()
        {
            Console.WriteLine($"{specimen}\t{x1} | {x2}  = {x1Val,6:F2} | {x2Val,6:F2}\tocena: {ocena,5:F2}");
        }
    }
    
    
    class Program
    {
        static List<Specimen> initializePopulation(int specimensCount, int specimenLen)
        {
            List<Specimen> population = new List<Specimen>();
            Random random = new Random();

            for (int i = 0; i < specimensCount; i++)
            {
                string newSpecimen = "";
                for (int j = 0; j < specimenLen; j++)
                {
                    newSpecimen += Convert.ToString(random.Next(0, 2));
                }
                population.Add(new Specimen(newSpecimen));
            }
            return population;
        }
        
        static void showPopulation(List<Specimen> population)
        {
            foreach (var specimen in population)
            {
                specimen.showSpecimen();
            }
        }
        
        static void evaluatePopulation(List<Specimen> population)
        {
            foreach (var specimen in population)
            {
                if (specimen.ocena == null)
                {
                    specimen.ocena = Math.Sin(specimen.x1Val * 0.05) + Math.Sin(specimen.x2Val * 0.05) +
                                    0.4 * Math.Sin(specimen.x1Val * 0.15) * Math.Sin(specimen.x2Val * 0.15);
                }
            }
        }
        
        
        static void Main(string[] args)
        {
            int LBnP = 3;
            int paramNum = 2;
            int specimenLen = LBnP * paramNum;
            int specimenCount = 9;
            int iterationCount = 20;
            double xMin = 0, xMax = 100;
            
            Specimen.LBnP = LBnP;
            Specimen.paramNum = paramNum;
            Specimen.chromLen = LBnP * paramNum;
            Specimen.minVal = xMin;
            Specimen.maxVal = xMax;
            
            List<Specimen> population = initializePopulation(specimenCount, specimenLen);
            population.Add(new Specimen("010010"));
            evaluatePopulation(population);
            showPopulation(population);
            population[3].showStatics();
        }
    }
}