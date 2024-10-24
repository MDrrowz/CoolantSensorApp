using System.Globalization;
using System.Runtime.CompilerServices;
using MathNet.Numerics;
using Microsoft.VisualBasic;

string csvFilePath = @"C:\Users\machine shop\source\repos\CoolantSensorApp\CoolantSensorApp\BoschSensor.csv";
string[] fileLines = File.ReadAllLines(csvFilePath);
string[] numberAsString = 
	(from line in fileLines
	 from number in line.Split(',')
	 where !string.IsNullOrWhiteSpace(number)
	 select number.Replace(" ", "").Replace("\n", "")).ToArray();
//foreach(string str in numberAsString) Console.WriteLine(str.Trim() + ", ");
double[] numbers = Array.ConvertAll(numberAsString, Double.Parse);
//foreach(int num in numbers) Console.Write(num);
double[] degreesCelsius = 
	(from deg in numbers
	where Array.IndexOf(numbers, deg) % 2 == 0
	select deg).ToArray();	
double[] resistance = 
	(from deg in numbers
	where Array.IndexOf(numbers, deg) % 2 != 0
	select deg).ToArray();
// PrintArray(degreesCelsius);
// PrintArray(resistance);

int order = -1;
for (int i = 0; i < 10; i++)
{
	Polynomial poly = Polynomial.Fit(resistance, degreesCelsius,i);
	double[] c = poly.Coefficients;
	List<double> errors = new();
	
	Console.WriteLine("Order: {0}", i);
	foreach (var res in resistance)
	{
		var celsCalculated = CalcTemp(res, poly).ToArray();
		int index = Array.IndexOf(resistance, res);
		double tempCalculated = celsCalculated[index];
		var temp = degreesCelsius[index];
		errors.Add(temp != 0 ? (100 * (tempCalculated - temp) / temp) : 0);
		//Console.WriteLine("Spec: {0} - {1}, Calc.'d: {2} ({3}%)", res, temp, tempCalculated, errors.Last().ToString());
	}
	double minError = errors.Min();
	double maxError = errors.Max();
	Console.WriteLine("Error min: {0}%, max: {1}%", Math.Round(minError,3), Math.Round(maxError,3));
	if(Math.Max(Math.Abs(minError), Math.Abs(maxError)) < 10)
	{
		order = i;
		break;
	}
}
Polynomial p = Polynomial.Fit(resistance, degreesCelsius, order);

Console.WriteLine("Polynomial: " + p.ToString());

//Foreach(var val in c) Console.WriteLine(val.Round(3));

//Console.WriteLine("Celsius = {0}*R^2 + {1}*R + {2}", c[0].Round(3), c[1].Round(3), c[2].Round(3));

// PrintData();

Console.ReadKey();

IEnumerable<double> CalcTemp(double res, Polynomial poly)
{
	IEnumerable<double> enumResistance = resistance;
	return poly.Evaluate(resistance);
}
void PrintArray(int[] array)
{
	foreach (var val in array) 	
	{
		if(Array.IndexOf(array, val) != array.Length - 1) Console.Write(val + ", ");
		else Console.WriteLine(val);
	}
	Console.WriteLine();
}
void PrintData()
{
	// Parameter Check
	// foreach (var res in resistance)
	// {
	// 	var celsCalculated = CalcTemp(res, 1).ToArray();
	// 	int index = Array.IndexOf(resistance, res);
	// 	double tempCalculated = celsCalculated[index];
	// 	var temp = degreesCelsius[index];
	// 	var error = temp != 0 ? (100 * (tempCalculated - temp) / temp).ToString() : "div 0";
	// 	Console.WriteLine("Spec: {0} - {1}, Calc.'d: {2} ({3}%)", res, temp, tempCalculated, error);
	// }
	// Input: provided T and R
	// 	Calculate T using R
	// Output: R --> Calculated T, percent error	
}