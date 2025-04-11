
using System.Globalization;

namespace AlgorytmGenetyczny
{
    class Specimen
    {
        public static int NumOfPar, LBnP;
        public static int chromLen => NumOfPar * LBnP;
        public static double minVal, maxVal;
        public string specimen { get; private set; }
        public List<string> xChrom { get; private set; }
        public List<double> xVal { get; private set; }
        public double? rate { get; set; }
        
        public Specimen(string specimen)
        {
            this.specimen = specimen;
            cut();
            decode(xChrom);
            rate = null;
        }

        private void cut()
        {
            List<string> xChrom = new List<string>();
            for (int i = 0; i < NumOfPar; i++)
            {
                xChrom.Add(specimen.Substring(i * LBnP, LBnP));
            }
            this.xChrom = xChrom;
        }

        private void decode(List<string> xChrom)
        {
            double ZD = maxVal - minVal;
            List<double> xVal = new List<double>();
            for (int i = 0; i < NumOfPar; i++)
            {
                if (xChrom[i].All(chrom => chrom == '0')) {xVal.Add(minVal);}
                else if (xChrom[i].All(chrom => chrom == '1')) {xVal.Add(maxVal);}
                else
                {
                    double ctmp = 0;
                    string xRev = new string(xChrom[i].Reverse().ToArray());
                    for (int j = 0; j < LBnP; j++)
                    {
                        if (xRev[j] != '0')
                        {
                            ctmp += Math.Pow(2, j);
                        }
                    }
                    xVal.Add(Math.Round(minVal + ctmp/(Math.Pow(2, LBnP) - 1) * ZD, 2));
                }
            }
            this.xVal = xVal;
        }
        
        public void showSpecimen()
        {
            Console.Write($"{specimen}  |  ");
            foreach (var val in xChrom)
            {
                Console.Write($"{val} ");
            }

            Console.Write(" | ");

            foreach (var val in xVal)
            {
                Console.Write($"{val,5:F2} ");
            }
            
            Console.Write($"  rate: {rate:F2} ");
            Console.WriteLine();
            
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

        static void evaluatePopulation(List<Specimen> population, List<(double x, double y)> probes)
        {
            foreach (var spec in population)
            {
                double rate = 0;
                foreach (var val in probes)
                {
                    rate += Math.Pow(val.y - (spec.xVal[0] * Math.Sin(spec.xVal[1] * val.x + spec.xVal[2])), 2);
                }
                spec.rate = rate;
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
                Specimen choice3 = population[random.Next(0, population.Count)];
                List<Specimen> choices = [choice1, choice2, choice3];
                selected.Add(choices.MinBy(choice => choice.rate));
            }
            return selected;
        }

        static List<Specimen> mutatorr(List<Specimen> newPopulation)
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

        static void mutator(List<Specimen> newPopulation)
        {
            Random random = new Random();

            for (int i = 4; i < newPopulation.Count - 1; i++)
            {
                int bPoint = random.Next(0, Specimen.chromLen);
                char[] chrom = newPopulation[i].specimen.ToCharArray();
                chrom[bPoint] = chrom[bPoint] == '0' ? '1' : '0';
                newPopulation[i] =  new Specimen(new string(chrom));
            }
        }

        static void hotDeck(List<Specimen> newPopulation, List<Specimen> population)
        {
            newPopulation.Add(population.MinBy(s => s.rate));
        }

        static void hybridization(List<Specimen> newPopulation)
        {
            Random random = new Random();
            for (int i = 0; i < newPopulation.Count - 1; i += 2)
            {
                if (i > 3 && i <= 7) continue;
                int cut = random.Next(0, Specimen.chromLen - 2);
                (string, string) p1 = (newPopulation[i].specimen.Substring(0, cut + 1), newPopulation[i].specimen.Substring(cut + 1));
                (string, string) p2 = (newPopulation[i + 1].specimen.Substring(0, cut + 1), newPopulation[i + 1].specimen.Substring(cut + 1));
                newPopulation[i] = new Specimen(p1.Item1 + p2.Item2);
                newPopulation[i + 1] = new Specimen(p2.Item1 + p1.Item2);
            }
        }

        static List<(double, double)> getProbes()
        {
            string path = "probes.txt";
            List<(double, double)> probes = new List<(double, double)>();
            
            if (File.Exists(path))
            {
                using StreamReader sr = new StreamReader(path);
                string line;
                while ((line = sr.ReadLine()) != null)
                    
                {
                    line = line.TrimStart();
                    string[] separated = line.Split("  ");
                    double probe1 = double.Parse(separated[0], CultureInfo.InvariantCulture);
                    double probe2 = double.Parse(separated[1], CultureInfo.InvariantCulture);
                    probes.Add((probe1, probe2));
                }
            }
            else
            {
                Console.WriteLine($"Plik {path} nie istnieje.");
            }
            return probes;
        }

        static void geneticAlghoritm(int iterationCount, int specimenCount)
        {
            List<Specimen> population = initializePopulation(specimenCount, Specimen.chromLen);
            List <(double, double)> probes = getProbes();
            evaluatePopulation(population, probes);
            Specimen theBest = population.MinBy(s => s.rate);
            double populationAverage = Math.Round((double)population.Average(s => s.rate), 2);
            Console.WriteLine("Pierwsza populacja");
            showPopulation(population);
            Console.WriteLine($"Najlepszy z populacji:  {theBest.specimen} | {theBest.rate:F2}\nŚrednia populacji: {populationAverage}\n");
            
            for (int i = 0; i < iterationCount; i++)
            {
                List<Specimen> newPopulation = tournamentSelection(population);
                hybridization(newPopulation);
                mutator(newPopulation);
                evaluatePopulation(newPopulation, probes);
                hotDeck(newPopulation, population);
                theBest = newPopulation.MinBy(s => s.rate);
                populationAverage = Math.Round((double)newPopulation.Average(s => s.rate), 2);
                Console.WriteLine($"Iteracja numer: {i + 1}");
                showPopulation(newPopulation);
                Console.WriteLine($"Najlepszy z populacji:  {theBest.specimen} | {theBest.rate:F2}\nŚrednia populacji: {populationAverage}\n");
                population = newPopulation;
            }
        }
        
        static void Main(string[] args)
        {
            int specimenCount = 13;
            int iterationCount = 200;
            Specimen.LBnP = 6;
            Specimen.NumOfPar = 3;
            Specimen.minVal = 0;
            Specimen.maxVal = 3;
            geneticAlghoritm(iterationCount, specimenCount);
        }
    }
}
