using System.Numerics;

Console.WriteLine("Программа для счета расстояния Дамерау-Левенштейна");
Console.WriteLine("Для выхода из программы напишите 'exit' в первую строку.");

do
{
    Console.Write("Введите первую(верную) строку: ");
    string correctInput = Console.ReadLine() ?? "";
    if (correctInput == "exit") break;
    Console.Write("Введите вторую(оишбочную) строку: ");
    string wrongInput = Console.ReadLine() ?? "";

    WriteDistance(wrongInput, correctInput);


} while (true);

Console.WriteLine("GoodLuck! :)");

static int Distance(string str1Example, string str2Example)
{
    if (str1Example == null || str2Example == null) return -1;

    int str1Length = str1Example.Length;
    int str2Length = str2Example.Length;

    if ((str1Length == 0) && (str2Length == 0)) return 0;
    else if (str1Length == 0) return str2Length;
    else if (str2Length == 0) return str1Length;

    string str1 = str1Example.ToUpper();
    string str2 = str2Example.ToUpper();

    int[,] matrix = new int[str1Length + 1, str2Length + 1];

    for (int i = 1; i <= str1Length; ++i)
    {
        for(int j = 1; j <= str2Length; ++j)
        {
            matrix[0,0] = 0;
            if( (i - 1) == 0) matrix[0,j] = j;
            if( (j - 1) == 0) matrix[i,0] = i;

            int symbol = (str1[i-1] == str2[j-1] ? 0 : 1);

            int ins = matrix[i,j-1] + 1;
            int del = matrix[i-1,j] + 1;
            int rep = matrix[i-1,j-1] + symbol;

            matrix[i,j] = Math.Min(Math.Min(ins, del), rep);

            if( (i > 1) && (j > 1) && (str1[i-2] == str2[j-1]) && (str1[i-1] == str2[j - 2]))
            {
                matrix[i,j] = Math.Min(matrix[i,j], matrix[i-2,j-2] + 1);
            }
        }
    }

    return matrix[str1Length, str2Length];
}

static void WriteDistance(string str1Param, string str2Param)
{
 int dist = Distance(str1Param, str2Param);
 Console.WriteLine($"'{str2Param}', '{str1Param}' -> " + dist.ToString());
}
