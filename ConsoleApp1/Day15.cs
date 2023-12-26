using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	internal class Day15 : Path
	{
		private static int DayID = 15;
		public static void run()
		{
			string path = $"{PATH}day{DayID}.txt";
			//path = $"{PATH}day{DayID}example.txt";
			//path = $"{PATH}day{DayID}example2.txt";
			string file = File.ReadAllText(path);

			//_Task1(file);
			_Task2(file);
		}

		private static void _Task1(string content)
		{
			int sum = 0;
			string[] sequence = content.Split(',');
			foreach (string sequenceItem in sequence)
			{
				HashString hash = new HashString(sequenceItem);
				sum += hash.Get256Hash();
			}

			Console.WriteLine($"Finished. Total sum of all codes: {sum}");
		}

		private static void _Task2(string content)
		{
			string[] sequence = content.Split(',');
			foreach (string sequenceItem in sequence)
			{
				LightBox.InsertValue(sequenceItem);

				//Console.WriteLine($"After \"{sequenceItem}\":");
				//LightBox.PrintAll();
				//Console.WriteLine();
			}

			Console.WriteLine($"Finished. Total sum of all Focusing Power of all LightBoxes: {LightBox.GetAllFocusingPower()}");
		}

		internal class LightBox
		{
			private static Dictionary<byte, LightBox> LightBoxes = new ();

			internal static void InsertValue(string sequenceValue)
			{
				if (sequenceValue.Contains(OPERATION_ADD_REPLACE))
				{
					string[] splitContent = sequenceValue.Split(OPERATION_ADD_REPLACE);
					byte lens = byte.Parse(splitContent[1]);
					string label = splitContent[0];
					byte Labelhash = HashString.Get256Hash(label);

					if (!LightBoxes.ContainsKey(Labelhash)) LightBoxes.Add(Labelhash, new LightBox(Labelhash));
					LightBoxes[Labelhash].AddOrReplace(label, lens);
				}
				else if (sequenceValue.Contains(OPERATION_REMOVE))
				{
					string[] splitContent = sequenceValue.Split(OPERATION_REMOVE);
					string label = splitContent[0];
					byte Labelhash = HashString.Get256Hash(label);

					if (!LightBoxes.ContainsKey(Labelhash)) LightBoxes.Add(Labelhash, new LightBox(Labelhash));
					LightBoxes[Labelhash].Remove(label);
				}
			}

			internal static int GetAllFocusingPower()
			{
				int power = 0;
				foreach (LightBox box in LightBoxes.Values)
				{
					power += box.GetFocusingPower();
				}
				return power;
			}

			internal static void PrintAll()
			{
				foreach (LightBox box in LightBoxes.Values)
				{
					if(box.IsEmpty()) continue;
					Console.WriteLine(box);
				}
			}


			private byte ID;

			private const char OPERATION_REMOVE = '-';
			private const char OPERATION_ADD_REPLACE = '=';

			List<LightBoxContent> Contents = new ();

			private LightBox(byte id)
			{
				this.ID = id;
			}

			public void AddOrReplace(string label, byte focalLength)
			{
				int index = IndexOf(label);
				if (index == -1)
				{
					Contents.Add(new LightBoxContent(label, focalLength));
					return;
				}

				Contents[index].FocalLength = focalLength;	// Insert new lens with same label
			}

			public void Remove(string label)
			{
				int index = IndexOf(label);
				if (index == -1) return;

				Contents.RemoveAt(index);
			}

			public int IndexOf(string label)
			{
				for (int i = 0; i < Contents.Count; i++)
				{
					if (Contents[i].Label == label) return i;
				}
				return -1;
			}
			public int IndexOf(string label, int focalLength)
			{
				for (int i = 0; i < Contents.Count; i++)
				{
					if (Contents[i].Label == label && Contents[i].FocalLength == focalLength) return i;
				}
				return -1;
			}

			internal int GetFocusingPower()
			{
				if (IsEmpty()) return 0;
				int power = 0;
				for (int i = 0; i < Contents.Count; i++)
				{
					power += (ID + 1) * (i + 1) * Contents[i].FocalLength;
				}
				return power;
			}

			internal bool IsEmpty() => Contents.Count == 0;

			public override string ToString()
			{
				StringBuilder builder = new StringBuilder();
				foreach (var content in Contents)
				{
					builder.Append(' ');
					builder.Append(content);
				}
				return $"Box {ID}:{builder}";
			}
		}

		internal class LightBoxContent
		{
			internal readonly string Label;
			internal byte FocalLength;

			internal LightBoxContent(string label, byte focalLength)
			{
				this.Label = label;
				this.FocalLength = focalLength;
			}

			public override string ToString()
			{
				return $"[{Label} {FocalLength}]";
			}
		}

		internal class HashString
		{
			private string Value;
			public HashString(string value)
			{
				this.Value = value;
			}

			public int Get256Hash()
			{
				byte hash = 0;  // For max value of 255, so no mod operation needed
				foreach (char c in Value)
				{
					hash += (byte) c;
					hash *= 17;
				}
				return hash;
			}

			public static byte Get256Hash(string value)
			{
				byte hash = 0;  // For max value of 255, so no mod operation needed
				foreach (char c in value)
				{
					hash += (byte) c;
					hash *= 17;
				}
				return hash;
			}
		}
	}
}
