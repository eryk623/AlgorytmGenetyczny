using System.Windows;

namespace GeneticAlgorithmWPFTask1;


public partial class Task1Window : Window
{
    public Task1Window()
    {
        InitializeComponent();
        RunTask1();
    }
    private void RunTask1()
    {
        int iterationCount = 20;
        int specimenCount = 9;
        Specimen.LBnP = 3;
        Specimen.NumOfPar = 2;
        Specimen.minVal = 0;
        Specimen.maxVal = 100;
        string result = Program.RunTask1Algorithm(iterationCount, specimenCount);
        ResultsTextBox.Text = result;
    }
}