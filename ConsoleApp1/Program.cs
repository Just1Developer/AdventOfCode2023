
using ConsoleApp1;

Day10.run();

/*
using System.Text;

int sum = 0;

string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\myFile.txt";
string[] file = File.ReadAllLines(path);

foreach (string s2 in file)
{
	int first = -1, last = -1;

	StringBuilder newString = new StringBuilder();

	int len = s2.Length;
	for (int i = 0; i < len; i++)
	{
		if (i <= len - 3 && s2.Substring(i, 3) == "one")
			newString.Append('1');
		else if (i <= len - 3 && s2.Substring(i, 3) == "two")
			newString.Append('2');
		else if (i <= len - 5 && s2.Substring(i, 5) == "three")
			newString.Append('3');
		else if (i <= len - 4 && s2.Substring(i, 4) == "four")
			newString.Append('4');
		else if (i <= len - 4 && s2.Substring(i, 4) == "five")
			newString.Append('5');
		else if (i <= len - 3 && s2.Substring(i, 3) == "six")
			newString.Append('6');
		else if (i <= len - 5 && s2.Substring(i, 5) == "seven")
			newString.Append('7');
		else if (i <= len - 5 && s2.Substring(i, 5) == "eight")
			newString.Append('8');
		else if (i <= len - 4 && s2.Substring(i, 4) == "nine")
			newString.Append('9');
		else newString.Append(s2[i]);
	}

	string s = newString.ToString();

	for (int i = 0; i < s.Length; i++)
	{
		int reverse = s.Length - i - 1;
		if(first == -1)
			if (s[i] >= '0' && s[i] <= '9')
				first = int.Parse(s[i] + "");
		if(last == -1)
			if (s[reverse] >= '0' && s[reverse] <= '9')
				last = int.Parse(s[reverse] + "");

		if (first > -1 && last > -1) break;
	}

	if (first == -1 || last == -1) throw new AbandonedMutexException();

	int localSum = first * 10 + last;
	sum += localSum;
	Console.WriteLine("Adding " + localSum + " | " + s2 + " >> " + s);
}

Console.WriteLine("Sum: " + sum);
*/