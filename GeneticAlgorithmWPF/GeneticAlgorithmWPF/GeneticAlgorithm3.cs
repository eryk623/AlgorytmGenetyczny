using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneticAlgorithmWPFTask3
{
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
            Cut();
            Decode(xChrom);
            rate = null;
        }

        private void Cut()
        {
            List<string> xChrom = new List<string>();
            for (int i = 0; i < NumOfPar; i++)
            {
                xChrom.Add(specimen.Substring(i * LBnP, LBnP));
            }
            this.xChrom = xChrom;
        }

        private void Decode(List<string> xChrom)
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
    
    public class Task3Algorithm
    {
        public static string RunAlgorithm(int iterationCount, int specimenCount)
        {
            StringBuilder sb = new StringBuilder();
            List<(int we1, int we2, int d)> probes = new List<(int, int, int)> {(0, 0, 0), (0, 1, 1), (1, 0, 1), (1, 1, 0) };
            
            List<Specimen> population = InitializePopulation(specimenCount, Specimen.chromLen);
            EvaluatePopulation(population, probes);
            Specimen theBest = population.MinBy(s => s.rate);
            double populationAverage = Math.Round(population.Average(s => s.rate.Value), 2);

            sb.AppendLine("Pierwsza populacja:");
            sb.AppendLine($"Najlepszy z populacji: {theBest.specimen}   ocena: {theBest.rate:F2}");
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
                sb.AppendLine($"Najlepszy z populacji: {theBest.specimen}   ocena: {theBest.rate:F2}");
                sb.AppendLine($"Średnia populacji: {populationAverage:F2}");
                sb.AppendLine();

                population = newPopulation;
            }
            return sb.ToString();
        }
        
        public static List<Specimen> InitializePopulation(int specimensCount, int specimenLen)
        {
            List<Specimen> population = new List<Specimen>();
            Random random = new Random();
            for (int i = 0; i < specimensCount; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < specimenLen; j++)
                {
                    sb.Append(random.Next(0, 2).ToString());
                }
                population.Add(new Specimen(sb.ToString()));
            }
            return population;
        }
        
        public static void EvaluatePopulation(List<Specimen> population, List<(int we1, int we2, int d)> probes)
        {
            foreach (var s in population)
            {
                double rate = 0;
                foreach (var p in probes)
                {
                    double neuron1 = 1 / (1 + Math.Exp(-(p.we1 * s.xVal[0] + p.we2 * s.xVal[1] + s.xVal[2])));
                    double neuron2 = 1 / (1 + Math.Exp(-(p.we1 * s.xVal[3] + p.we2 * s.xVal[4] + s.xVal[5])));
                    double output = 1 / (1 + Math.Exp(-(neuron1 * s.xVal[6] + neuron2 * s.xVal[7] + s.xVal[8])));
                    rate += Math.Pow(p.d - output, 2);
                }
                s.rate = rate;
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
                chrom[bPoint] = (chrom[bPoint] == '0') ? '1' : '0';
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
    }
}
