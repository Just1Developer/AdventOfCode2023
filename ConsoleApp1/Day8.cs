namespace ConsoleApp1
{
	internal class Day8 : Path
	{
        public static void run()
        {
            string path = PATH + "day8.txt";
            //path = PATH + "day8example.txt";
            //path = PATH + "day8example2.txt";
            //path = PATH + "day8example3.txt";
            string[] lines = File.ReadAllLines(path);

            Task2_2(lines);
        }

        // Correct answer task 2: 13.334.102.464.297    (~13.3 Trillion)

        static void Task1(string[] lines)
		{
            string instructions = "";
            Node? firstNode = null;
            Node? lastNode = null;

            Dictionary<string, (bool, bool)> encounters = new Dictionary<string, (bool, bool)>();

            foreach (string line in lines)
			{
                if (string.IsNullOrEmpty(instructions))
				{
                    instructions = line;
                    continue;
                }
                if (string.IsNullOrEmpty(line)) continue;
                Node n = new Node(line);
                if (n.ID == "AAA") firstNode = n;
                if (n.ID == "ZZZ") lastNode = n;
                encounters.Add(n.ID, (false, false));
            }

            // Pathfinder: First to Last
            int i = 0;
            Node currentNode = firstNode;
            int steps = 0;
            while (currentNode.ID != lastNode.ID)
			{
                // Go to next node
                if (i >= instructions.Length) i %= instructions.Length;
                char instruction = instructions[i++];

                string oldID = currentNode.ID;

                /*
                // Loop protection code
                if (instruction == 'L')
                {
                    // We've been here before
                    if (encounters[oldID].Item1 == true)
                    {
                        Console.WriteLine($"Encountered an Infinite Loop at ({oldID} -> {currentNode.Left}, L), terminating..");
                        return;
                    }
                    encounters[oldID] = (true, encounters[oldID].Item2);
                }
                else
                {
                    // We've been here before
                    if (encounters[oldID].Item2 == true)
                    {
                        Console.WriteLine($"Encountered an Infinite Loop at ({oldID} -> {currentNode.Right}, R), terminating..");
                        return;
                    }
                    encounters[oldID] = (encounters[oldID].Item1, true);
                }
                */


                currentNode = Node.GetNext(currentNode, instruction);
                Console.WriteLine($"Going from ({oldID}) to ({currentNode.ID}) via '{instruction}'");

                steps++;
            }

            Console.WriteLine($"Steps: {steps}");
		}
        
        static void Task2(string[] lines)
		{
            instructions = "";
            List<Node> currentNodes = new List<Node>();

            Dictionary<string, (bool, bool)> encounters = new Dictionary<string, (bool, bool)>();

            foreach (string line in lines)
			{
                if (string.IsNullOrEmpty(instructions))
				{
                    instructions = line;
                    continue;
                }
                if (string.IsNullOrEmpty(line)) continue;
                Node n = new Node(line);
                if (n.ID[2] == 'A') currentNodes.Add(n);
                encounters.Add(n.ID, (false, false));
            }

            // Pathfinder: First to Last

            for (int i = 0; i < currentNodes.Count; i++)
			{
                NodeMetZ.Add(new List<long>());
            }

            /*
            int i = 0;
            int steps = 0;
            bool allNodesEndWithZ = false;
            while (!allNodesEndWithZ)
			{
                allNodesEndWithZ = true;

                // Go to next node
                if (i >= instructions.Length) i %= instructions.Length;
                char instruction = instructions[i++];

                // Go step
                for (int n = 0; n < currentNodes.Count; n++)
                {
                    //string oldID = currentNodes[n].ID;

                    currentNodes[n] = Node.GetNext(currentNodes[n], instruction);
                    //Console.WriteLine($"Going from ({oldID}) to ({currentNodes[n].ID}) via '{instruction}'");
                    if (currentNodes[n].ID[2] != 'Z') allNodesEndWithZ = false;
                }

                steps++;
            }
            */

            // Multithread:

            Console.WriteLine("Time: " + DateTime.Now);
            Console.WriteLine($"Starting with {currentNodes.Count} Nodes.");

            finished = false;
            List<Thread> threads = new();
            for (int i2 = 0; i2 < currentNodes.Count; i2++)
			{
                Thread t = new Thread(() => { T2_run(currentNodes[t_count], t_count); });
                threads.Add(t);
                t.Start();
                // Wait until it started. We can somehow calc this, but lets actually just wait a second
                Thread.Sleep(1000);
                t_count++;
            }

            long steps = -1;

            while (!finished)
			{
                for (int id = 0; id < NodeMetZ[0].Count; id++)
                {
                    bool containsAll = true;
                    for (int i3 = 1; i3 < NodeMetZ.Count; i3++)
					{
                        if(!NodeMetZ[i3].Contains(id)) containsAll = false;
					}
                    if (!containsAll) continue;
                    steps = id; // At this point, all met
                    finished = true;
                    break;
				}
			}

            Console.WriteLine("Time: " + DateTime.Now);
            Console.WriteLine($"Steps: {steps}");
		}
        
        static void Task2_2(string[] lines)
		{
            instructions = "";
            List<Node> currentNodes = new List<Node>();

            foreach (string line in lines)
			{
                if (string.IsNullOrEmpty(instructions))
				{
                    instructions = line;
                    continue;
                }
                if (string.IsNullOrEmpty(line)) continue;
                Node n = new Node(line);
                if (n.ID[2] == 'A') currentNodes.Add(n);
            }

            // Pathfinder: First to Last

            Console.WriteLine("Time: " + DateTime.Now);
            Console.WriteLine($"Starting with {currentNodes.Count} Nodes.");

            Console.WriteLine("Determining all loops...");

            List<ulong>[] startnodeEncounteredZIndicesTillLoopFound = new List<ulong>[currentNodes.Count];

            int nodeID = 0;
            foreach (Node n in currentNodes)
			{
                // Follow path until repetition
                // Saves node, encountered at instruction index i
                Dictionary<string, (List<int>, List<ulong>)> encounters = new ();
                startnodeEncounteredZIndicesTillLoopFound[nodeID] = new List<ulong>();

                // There HAS to be a loop.
                int i = 0;
                Node currentNode = n;
                ulong steps = 0;

                while (true)
                {
                    if (i >= instructions.Length) i %= instructions.Length;
                    char instruction = instructions[i++];

                    // Loop protection code
                    if (encounters.ContainsKey(currentNode.ID))
                    {
                        // We've been here before
                        if (encounters[currentNode.ID].Item1.Contains(i))
                        {
                            int index = encounters[currentNode.ID].Item1.IndexOf(i);
                            ulong startSteps = encounters[currentNode.ID].Item2[index];
                            Console.WriteLine($"Encountered an Infinite Loop at ({currentNode.ID} -> {Node.GetNext(currentNode, instruction).ID}, Instruction index: {i}), Number of steps: {startSteps} -> {steps} (loop is {steps - startSteps} steps ulong), encountered {startnodeEncounteredZIndicesTillLoopFound[nodeID].Count} many Z-node possibilities ({startnodeEncounteredZIndicesTillLoopFound[nodeID][0]})");
                            break;
                        }
                        encounters[currentNode.ID].Item1.Add(i);
                        encounters[currentNode.ID].Item2.Add(steps);
                    }
                    else
                    {
                        // I've never met this node in my life
                        List<int> l1 = new();
                        List<ulong> l2 = new();
                        l1.Add(i);
                        l2.Add(steps);
                        encounters.Add(currentNode.ID, (l1, l2));
                    }

                    if(currentNode.ID[2] == 'Z')
					{
                        // Z-node, remember
                        startnodeEncounteredZIndicesTillLoopFound[nodeID].Add(steps);
                    }

                    currentNode = Node.GetNext(currentNode, instruction);
                    steps++;
                }
                nodeID++;
            }

            // This would be more complicated, but each list actually (in this case) only contains 1 value.
            // So, we can just compile a list of all these values and determine the smallest common divisor.

            ulong[] numList = new ulong[currentNodes.Count];

            for (int i = 0; i < currentNodes.Count; i++)
			{
                numList[i] = startnodeEncounteredZIndicesTillLoopFound[i][0];
			}

            ulong leastCommonDivider = 1;

            foreach (ulong j in numList)
            {
                leastCommonDivider = LCM(leastCommonDivider, j);
                Console.WriteLine("Calculated new temp: " + leastCommonDivider);
            }

            Console.WriteLine("Time: " + DateTime.Now);
            Console.WriteLine($"Steps: {leastCommonDivider}");
		}

        // GPT-4 suggestion: Formular / techniques
        static ulong GCD(ulong a, ulong b)
        {
            while (b != 0)
            {
                ulong temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static ulong LCM(ulong a, ulong b)
        {
            return (ulong) Math.Abs((double) (a * b)) / GCD(a, b);
        }

        static int t_count = 0;
        static string instructions;
        static bool finished = true;
        static List<List<long>> NodeMetZ = new List<List<long>>();

        static void T2_run(Node node, int ThreadID)
		{
            Console.WriteLine($"Started Thread {ThreadID}");
            Node currentNode = node;
            int i = 0;
            long steps = 0;
            while (!finished)
            {
                //Console.WriteLine($"Accessing Index {i} of instructions with length {instructions.Length}");
                // Go to next node
                if (i >= instructions.Length) i %= instructions.Length;
                char instruction = instructions[i++];

                //Console.WriteLine($"Accessing Index {ThreadID} of NodeMetZ with Size {NodeMetZ.Count}");

                currentNode = Node.GetNext(currentNode, instruction);
                if (currentNode.ID[2] == 'Z')
                {
                    NodeMetZ[ThreadID].Add(steps);    // Keep track of when we met node with z
                    //Console.WriteLine($"Thread {ThreadID} encountered Z-node at step {steps}");
                }

                steps++;
            }
        }

        class Node
		{
            internal static Dictionary<string, Node> Nodes = new ();

            internal string ID;
            internal string Left, Right;

            // AAA = (BBB, CCC)
            // 0123456789ABCDEF
            internal Node(string value)
			{
                this.ID = value.Substring(0, 3);
                this.Left = value.Substring(7, 3);
                this.Right = value.Substring(12, 3);
                Nodes.Add(ID, this);
			}

            internal static Node GetNext(Node current, char instruction)
			{
                string nextID = instruction == 'L' ? current.Left : current.Right;
                if(Nodes.ContainsKey(nextID)) return Nodes[nextID];
                throw new NullReferenceException(nextID + " doesnt exist lol");
			}
		}
    }
}
