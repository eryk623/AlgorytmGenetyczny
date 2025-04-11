using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneticAlgorithmWPFTask2;

    public class Specimen
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
                if (xChrom[i].All(chrom => chrom == '0'))
                    xVal.Add(minVal);
                else if (xChrom[i].All(chrom => chrom == '1'))
                    xVal.Add(maxVal);
                else
                {
                    double ctmp = 0;
                    string xRev = new string(xChrom[i].Reverse().ToArray());
                    for (int j = 0; j < LBnP; j++)
                    {
                        if (xRev[j] != '0')
                            ctmp += Math.Pow(2, j);
                    }
                    xVal.Add(Math.Round(minVal + ctmp / (Math.Pow(2, LBnP) - 1) * ZD, 2));
                }
            }
            this.xVal = xVal;
        }
    }

    public class GeneticAlgorithm
    {
        public static List<Specimen> InitializePopulation(int specimensCount, int specimenLen)
        {
            List<Specimen> population = new List<Specimen>();
            Random random = new Random();
            for (int i = 0; i < specimensCount; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < specimenLen; j++)
                    sb.Append(random.Next(0, 2).ToString());
                population.Add(new Specimen(sb.ToString()));
            }
            return population;
        }
        
        public static void EvaluatePopulation(List<Specimen> population, List<(double x, double y)> probes)
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
        
        public static List<Specimen> TournamentSelection(List<Specimen> population)
        {
            List<Specimen> selected = new List<Specimen>();
            Random random = new Random();
            for (int i = 0; i < population.Count - 1; i++)
            {
                Specimen choice1 = population[random.Next(0, population.Count)];
                Specimen choice2 = population[random.Next(0, population.Count)];
                Specimen choice3 = population[random.Next(0, population.Count)];
                List<Specimen> choices = new List<Specimen> { choice1, choice2, choice3 };
                selected.Add(choices.MinBy(choice => choice.rate));
            }
            return selected;
        }
        
        public static void Mutator(List<Specimen> newPopulation)
        {
            Random random = new Random();
            for (int i = 4; i < newPopulation.Count - 1; i++)
            {
                int bPoint = random.Next(0, Specimen.chromLen);
                char[] chrom = newPopulation[i].specimen.ToCharArray();
                chrom[bPoint] = chrom[bPoint] == '0' ? '1' : '0';
                newPopulation[i] = new Specimen(new string(chrom));
            }
        }
        
        public static void HotDeck(List<Specimen> newPopulation, List<Specimen> population)
        {
            newPopulation.Add(population.MinBy(s => s.rate));
        }
        
        public static void Hybridization(List<Specimen> newPopulation)
        {
            Random random = new Random();
            for (int i = 0; i < newPopulation.Count - 1; i += 2)
            {
                if (i > 3 && i <= 7)
                    continue;
                int cut = random.Next(1, Specimen.chromLen - 1);
                (string, string) p1 = (newPopulation[i].specimen.Substring(0, cut),
                                        newPopulation[i].specimen.Substring(cut));
                (string, string) p2 = (newPopulation[i + 1].specimen.Substring(0, cut),
                                        newPopulation[i + 1].specimen.Substring(cut));
                newPopulation[i] = new Specimen(p1.Item1 + p2.Item2);
                newPopulation[i + 1] = new Specimen(p2.Item1 + p1.Item2);
            }
        }
        
        public static List<(double, double)> GetProbes()
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
                    string[] separated = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    double probe1 = double.Parse(separated[0], CultureInfo.InvariantCulture);
                    double probe2 = double.Parse(separated[1], CultureInfo.InvariantCulture);
                    probes.Add((probe1, probe2));
                }
            }
            return probes;
        }
        
        public static string RunTask2Algorithm(int iterationCount, int specimenCount)
        {
            StringBuilder sb = new StringBuilder();
            List<Specimen> population = InitializePopulation(specimenCount, Specimen.chromLen);
            List<(double, double)> probes = GetProbes();
            EvaluatePopulation(population, probes);
            Specimen theBest = population.MinBy(s => s.rate);
            double populationAverage = Math.Round(population.Average(s => s.rate.Value), 2);
            sb.AppendLine("Pierwsza populacja:");
            sb.AppendLine($"Najlepszy z populacji: {theBest.specimen}  ocena: {theBest.rate:F2}");
            sb.AppendLine($"Średnia populacji: {populationAverage:F2}");
            sb.AppendLine();

            for (int i = 0; i < iterationCount; i++)
            {
                List<Specimen> newPopulation = TournamentSelection(population);
                Hybridization(newPopulation);
                Mutator(newPopulation);
                EvaluatePopulation(newPopulation, probes);
                HotDeck(newPopulation, population);
                theBest = newPopulation.MinBy(s => s.rate);
                populationAverage = Math.Round(newPopulation.Average(s => s.rate.Value), 2);
                sb.AppendLine($"Iteracja numer: {i + 1}");
                sb.AppendLine($"Najlepszy z populacji: {theBest.specimen}  ocena: {theBest.rate:F2}");
                sb.AppendLine($"Średnia populacji: {populationAverage:F2}");
                sb.AppendLine();
                population = newPopulation;
            }
            return sb.ToString();
        }
    }

