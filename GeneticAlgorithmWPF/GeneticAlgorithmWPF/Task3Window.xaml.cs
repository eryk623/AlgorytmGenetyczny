using System.Windows;

namespace GeneticAlgorithmWPFTask3
{
    public partial class Task3Window : Window
    {
        public Task3Window()
        {
            InitializeComponent();
            RunTask3();
        }

        private void RunTask3()
        {
            int iterationCount = 200;
            int specimenCount = 13;
            Specimen.LBnP = 4;
            Specimen.NumOfPar = 9;
            Specimen.minVal = -10;
            Specimen.maxVal = 10;
            string result = Task3Algorithm.RunAlgorithm(iterationCount, specimenCount);
            ResultsTextBox.Text = result;
        }
    }
}