
namespace AlgorytmGenetyczny
{
    class Specimen
    {
        public static int NumOfPar, LBnP;
        public static int chromLen => NumOfPar * LBnP;
        public static double minVal, maxVal;
        public string specimen { get; private set; }
        public string x1 { get; private set; }
        public string x2 { get; private set; }
        public double x1Val { get; private set; }
        public double x2Val { get; private set; }
        public double? rate { get; set; }
        
        public Specimen(string specimen)
        {
            this.specimen = specimen;
            x1 = specimen.Substring(0, LBnP);
            x2 = specimen.Substring(LBnP);
            x1Val = decode(x1);
            x2Val = decode(x2);
            rate = null;
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
        
        public void showSpecimen()
        {
            Console.WriteLine($"{specimen}\t{x1} | {x2}  = {x1Val,6:F2} | {x2Val,6:F2}\tocena: {rate,5:F2}");
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
                if (specimen.rate == null)
                {
                    specimen.rate = Math.Round(Math.Sin(specimen.x1Val * 0.05) + Math.Sin(specimen.x2Val * 0.05) +
                                    0.4 * Math.Sin(specimen.x1Val * 0.15) * Math.Sin(specimen.x2Val * 0.15), 2);
                }
            }
        }

        static List<Specimen> tournamentSelection(List<Specimen> population)
        {
            List<Specimen> selected = new List<Specimen>();
            Random random = new Random();
            
            for (int i = 0; i < population.Count - 1; i++)
            {
                Specimen choice1 = population[random.Next(0, population.Count)];
                Specimen choice2 = population[random.Next(0, population.Count)];
                selected.Add(choice1.rate >= choice2.rate ?  choice1 : choice2);
            }
            
            return selected;
        }

        static List<Specimen> mutator(List<Specimen> newPopulation)
        {
            List<Specimen> mutated = new List<Specimen>();
            Random random = new Random();

            foreach (var specimen in newPopulation)
            {
                int bPoint = random.Next(0, specimen.specimen.Length);
                char[] chrom = specimen.specimen.ToCharArray();
                chrom[bPoint] = chrom[bPoint] == '0' ? '1' : '0';
                mutated.Add(new Specimen(new string(chrom)));
            }
            return mutated;
        }

        static void hotDeck(List<Specimen> newPopulation, List<Specimen> population)
        {
            newPopulation.Add(population.MaxBy(s => s.rate));
        }

        static void geneticAlghoritm(int iterationCount, int specimenCount)
        {
            List<Specimen> population = initializePopulation(specimenCount, Specimen.chromLen);
            evaluatePopulation(population);
            Specimen theBest = population.MaxBy(s => s.rate);
            double populationAverage = Math.Round((double)population.Average(s => s.rate), 2);
            Console.WriteLine("Pierwsza populacja");
            showPopulation(population);
            Console.WriteLine($"Najlepszy z populacji:  {theBest.specimen} | {theBest.rate}\nŚrednia populacji: {populationAverage}\n");
            
            for (int i = 0; i < iterationCount; i++)
            {
                List<Specimen> newPopulation = tournamentSelection(population);
                newPopulation = mutator(newPopulation);
                evaluatePopulation(newPopulation);
                hotDeck(newPopulation, population);
                theBest = newPopulation.MaxBy(s => s.rate);
                populationAverage = Math.Round((double)newPopulation.Average(s => s.rate), 2);
                Console.WriteLine($"Iteracja numer: {i + 1}");
                showPopulation(newPopulation);
                Console.WriteLine($"Najlepszy z populacji:  {theBest.specimen} | {theBest.rate}\nŚrednia populacji: {populationAverage}\n");
                population = newPopulation;
            }
        }
        
        static void Main(string[] args)
        {
            int specimenCount = 9;
            int iterationCount = 20;
            Specimen.LBnP = 3;
            Specimen.NumOfPar = 2;
            Specimen.minVal = 0;
            Specimen.maxVal = 100;
            
            geneticAlghoritm(iterationCount, specimenCount);
        }
    }
}