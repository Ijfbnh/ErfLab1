using System;
using System.Globalization;

namespace ErfLab1;

internal static class Program
{
    private const double X = 3.0;
    private const double TwoOverSqrtPi = 1.12837916709551257389615890312154517;

    private static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА №1. ВЫЧИСЛЕНИЯ С ПЛАВАЮЩЕЙ ТОЧКОЙ");
        Console.WriteLine(new string('=', 72));
        PrintFloatingPointLimits();
        PrintFirstThreeTerms();

        Console.WriteLine("\n1) Факториал целого типа (Int32)");
        Console.WriteLine(CalculateWithIntFactorial());

        Console.WriteLine("\n2) Факториал вещественного типа (Double)");
        var d = CalculateWithDoubleFactorial();
        Console.WriteLine($"erf(3) = {d.Value:G17}");
        Console.WriteLine($"Число учтённых членов: {d.Terms}");
        Console.WriteLine($"Последний ненулевой член: {d.LastTerm:G17}");

        Console.WriteLine("\n3) Рекуррентное вычисление очередного члена через предыдущий");
        var r = CalculateByRecurrence();
        Console.WriteLine($"erf(3) = {r.Value:G17}");
        Console.WriteLine($"Число учтённых членов: {r.Terms}");
        Console.WriteLine($"Последний ненулевой член: {r.LastTerm:G17}");

        const double reference = 0.9999779095030014;
        Console.WriteLine("\nКонтрольное значение и ошибки");
        Console.WriteLine($"Контрольное значение = {reference:G17}");
        Console.WriteLine($"Погрешность метода 2 = {Math.Abs(d.Value - reference):G17}");
        Console.WriteLine($"Погрешность метода 3 = {Math.Abs(r.Value - reference):G17}");
        Console.WriteLine("\nНажмите любую клавишу для завершения.");
        Console.ReadKey();
    }

    private static void PrintFloatingPointLimits()
    {
        Console.WriteLine("\nГРАНИЦЫ ПРЕДСТАВЛЕНИЯ ВЕЩЕСТВЕННЫХ ЧИСЕЛ");
        Console.WriteLine("Тип       Машинный эпсилон       Min normal          Min subnormal       Max finite");
        Console.WriteLine($"Single    {1.1920928955078125E-7:G9}   {1.17549435E-38:G9}   {BitConverter.Int32BitsToSingle(1):G9}   {float.MaxValue:G9}");
        Console.WriteLine($"Double    {2.2204460492503131E-16:G9}   {2.2250738585072014E-308:G9}   {BitConverter.Int64BitsToDouble(1):G9}   {double.MaxValue:G9}");
        Console.WriteLine("Машинный ноль: значения меньше половины минимального субнормального числа округляются к 0.");
        Console.WriteLine("Машинная бесконечность: при выходе за верхнюю границу конечного диапазона получается +Infinity.");
    }

    private static void PrintFirstThreeTerms()
    {
        Console.WriteLine("\nПЕРВЫЕ ТРИ ЧЛЕНА РЯДА (до множителя 2/sqrt(pi))");
        double factorial = 1.0, sum = 0.0;
        for (int n = 0; n < 3; n++)
        {
            if (n > 0) factorial *= n;
            double term = Math.Pow(-1.0, n) * Math.Pow(X, 2 * n + 1)
                          / (factorial * (2 * n + 1));
            sum += term;
            Console.WriteLine($"n={n}: a_n={term:G17}; частичная сумма={sum:G17}");
        }
        Console.WriteLine($"Сумма трёх членов с множителем 2/sqrt(pi): {sum * TwoOverSqrtPi:G17}");
    }

    private static string CalculateWithIntFactorial()
    {
        int factorial = 1;
        double sum = 0.0;
        for (int n = 0; n < 100; n++)
        {
            if (n > 0)
            {
                if (factorial > int.MaxValue / n)
                    return $"ОШИБКА: переполнение Int32 при вычислении {n}!.\n" +
                           $"Максимальный корректный факториал Int32: 12! = 479001600.\n" +
                           $"Частичная сумма до переполнения (не является ответом): {sum * TwoOverSqrtPi:G17}";
                factorial *= n;
            }
            double term = Math.Pow(-1.0, n) * Math.Pow(X, 2 * n + 1)
                          / ((double)factorial * (2 * n + 1));
            if (term == 0.0) break;
            sum += term;
        }
        return $"erf(3) = {sum * TwoOverSqrtPi:G17}";
    }

    private static (double Value, int Terms, double LastTerm) CalculateWithDoubleFactorial()
    {
        double factorial = 1.0, sum = 0.0, lastTerm = 0.0;
        int terms = 0;
        for (int n = 0; n < 1000; n++)
        {
            if (n > 0) factorial *= n;
            double term = Math.Pow(-1.0, n) * Math.Pow(X, 2 * n + 1)
                          / (factorial * (2 * n + 1));
            if (term == 0.0 || double.IsInfinity(term) || double.IsNaN(term)) break;
            sum += term; lastTerm = term; terms++;
        }
        return (sum * TwoOverSqrtPi, terms, lastTerm);
    }

    private static (double Value, int Terms, double LastTerm) CalculateByRecurrence()
    {
        // a_0=x; a_(n+1)=a_n*(-x^2)*(2n+1)/((n+1)*(2n+3)).
        double term = X, sum = 0.0, lastTerm = 0.0;
        int terms = 0;
        for (int n = 0; n < 1000; n++)
        {
            if (term == 0.0 || !double.IsFinite(term)) break;
            sum += term; lastTerm = term; terms++;
            term *= -X * X * (2.0 * n + 1.0) / ((n + 1.0) * (2.0 * n + 3.0));
        }
        return (sum * TwoOverSqrtPi, terms, lastTerm);
    }
}
