using System.Windows;

namespace GeneticAlgorithmWPFTask2
{
    public partial class Task2Window : Window
    {
        public Task2Window()
        {
            InitializeComponent();
            RunTask2();
        }
        
        private void RunTask2()
        {
            int iterationCount = 100;
            int specimenCount = 13;
            Specimen.LBnP = 4;
            Specimen.NumOfPar = 3;
            Specimen.minVal = 0;
            Specimen.maxVal = 3;
            string result = GeneticAlgorithm.RunTask2Algorithm(iterationCount, specimenCount);
            ResultsTextBox.Text = result;
        }
    }
}