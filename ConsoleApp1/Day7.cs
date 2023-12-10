namespace ConsoleApp1;

public class Day7 : Path
{
    
    
    public static void run()
    {
        string path = PATH + "day7.txt";
        //path = PATH + "day7example.txt";
        string[] lines = File.ReadAllLines(path);

        Task2(lines);
    }

    static List<Hand> Hands;

    static void Task1(string[] lines)
    {
        Hands = new();
        foreach (string line in lines)
		{
            Hand hand = new Hand(line);
            Insert(hand);
            Console.WriteLine($"{line} >> {hand.HandType}");
        }

        int sum = 0;

        for (int rank = 1; rank <= Hands.Count; rank++)
		{
            sum += rank * Hands[rank - 1].Bid;
            Console.WriteLine($"Rank: {rank} | HandType: {Hands[rank - 1].HandType} | HandString: {Hands[rank - 1].HandString}");
		}

        Console.WriteLine($"The sum of all cards' bids multiplied with their rank is {sum}");
    }

    static void Task2(string[] lines)
    {
        Hands = new();
        foreach (string line in lines)
		{
            Hand hand = new Hand(line);
            T2Insert(hand);
            Console.WriteLine($"{line} >> {hand.HandType}");
        }

        int sum = 0;

        for (int rank = 1; rank <= Hands.Count; rank++)
		{
            sum += rank * Hands[rank - 1].Bid;
            Console.WriteLine($"Rank: {rank} | HandType: {Hands[rank - 1].HandType} | HandString: {Hands[rank - 1].HandString}");
		}

        Console.WriteLine($"The sum of all cards' bids multiplied with their rank is {sum}");
    }

    static void Insert(Hand hand)
	{
        for (int i = 0; i < Hands.Count; i++)
		{
            Hand other = Hands[i];
            if (hand.IsHigherThan(other)) continue;
            Hands.Insert(i, hand);
            return;
		}
        // Not inserted, higher than any other.
        // Add now
        Hands.Add(hand);
	}

    static void T2Insert(Hand hand)
	{
        for (int i = 0; i < Hands.Count; i++)
		{
            Hand other = Hands[i];
            if (hand.T2IsHigherThan(other)) continue;
            Hands.Insert(i, hand);
            return;
		}
        // Not inserted, higher than any other.
        // Add now
        Hands.Add(hand);
	}

    class Hand
	{
        internal int Bid;
        internal string HandString;
        internal HandType HandType;

        CardType[] CardTypes;
        CardType Highcard = CardType.None;

        internal Hand(string value)
		{
            string[] values = value.Trim().Split(' ');

            HandString = values[0];
            Bid = int.Parse(values[1]);
            CardTypes = new CardType[HandString.Length];

            for (int i = 0; i < HandString.Length; i++)
			{
                CardTypes[i] = ValueOf(HandString[i]);
                if(Highcard < CardTypes[i]) Highcard = CardTypes[i];
			}

            //HandType = GetHandType();
            HandType = GetHandTypeTask2();

		}

        HandType GetHandType()
		{
            Dictionary<CardType, int> amounts = new Dictionary<CardType, int>();
            foreach (CardType c in CardTypes)
			{
                if (amounts.ContainsKey(c)) continue;
                amounts.Add(c, CountType(c));
			}

            // Check by descending order:
            if (amounts.ContainsValue(5)) return HandType.FiveOfAKind;
            if (amounts.ContainsValue(4)) return HandType.FourOfAKind;
            if (amounts.ContainsValue(3) && amounts.ContainsValue(2)) return HandType.FullHouse;
            if (amounts.ContainsValue(3)) return HandType.ThreeOfAKind;
            
            // For 2 times 2 there probably is a better way, but lets do it like this.
            bool contains2 = false;
            foreach (var v in amounts.Values)
            {
                if (v == 2 && !contains2) { contains2 = true; continue; }
                if (v == 2) return HandType.TwoPair;
            }

            if (amounts.ContainsValue(2)) return HandType.OnePair;
            return HandType.HighCard;
        }

        HandType GetHandTypeTask2()
		{
            Dictionary<CardType, int> amounts = new Dictionary<CardType, int>();
            foreach (CardType c in CardTypes)
			{
                if (amounts.ContainsKey(c)) continue;
                amounts.Add(c, CountType(c));
			}

            int JokerCount = amounts.ContainsKey(CardType.J) ? amounts[CardType.J] : 0;
            if (JokerCount == 5) return HandType.FiveOfAKind;   // In this case the loop won't yield anything
            if (JokerCount == 0) return GetHandType();  // Without this line, something is evaluated incorrectly below.

            bool containsPair = false, containsTwoPair = false, containsTriple = false;
            // First, Determine existing pairs
            foreach (var entry in amounts)
            {
                if (entry.Key == CardType.J) continue;

                var v = amounts[entry.Key];
                if (v + JokerCount == 5) return HandType.FiveOfAKind;
                if (v + JokerCount == 4) return HandType.FourOfAKind;

                if (v == 3) containsTriple = true;
                if (v == 2)
                {
                    if (containsPair) containsTwoPair = true;
                    else containsPair = true;
                }
            }

            // Triple + Pair or Triple + Card + Joker = Full House
            if (containsTriple && (containsPair || JokerCount > 0)) return HandType.FullHouse;
            if (containsTwoPair && JokerCount > 0) return HandType.FullHouse;
            if (containsPair && JokerCount > 0) return HandType.ThreeOfAKind;
            if (JokerCount >= 2) return HandType.ThreeOfAKind;
            if (containsTriple) return HandType.ThreeOfAKind;
            if (containsTwoPair) return HandType.TwoPair;
            if (containsPair || JokerCount > 0) return HandType.OnePair;
            return HandType.HighCard;
        }

        internal bool IsHigherThan(Hand other)
		{
            if (HandType < other.HandType) return false;
            if (HandType > other.HandType) return true;

            for (int i = 0; i < Math.Min(CardTypes.Length, other.CardTypes.Length); i++)
			{
                if (CardTypes[i] == other.CardTypes[i]) continue;
                if (CardTypes[i] > other.CardTypes[i]) return true;
                if (CardTypes[i] < other.CardTypes[i]) return false;
			}

            return false;
		}

        internal bool T2IsHigherThan(Hand other)
		{
            if (HandType < other.HandType) return false;
            if (HandType > other.HandType) return true;

            for (int i = 0; i < Math.Min(CardTypes.Length, other.CardTypes.Length); i++)
			{
                if (T2NumValueOf(CardTypes[i]) == T2NumValueOf(other.CardTypes[i])) continue;
                if (T2NumValueOf(CardTypes[i]) > T2NumValueOf(other.CardTypes[i])) return true;
                if (T2NumValueOf(CardTypes[i]) < T2NumValueOf(other.CardTypes[i])) return false;
			}

            return false;
		}

        int CountType(CardType type)
		{
            int count = 0;
            foreach (var t in CardTypes)
			{
                if (t == type) count++;
			}
            return count;
		}
    }

    static CardType ValueOf(char s)
    {
        switch(s)
		{
            case '1':
                return CardType._1;
            case '2':
                return CardType._2;
            case '3':
                return CardType._3;
            case '4':
                return CardType._4;
            case '5':
                return CardType._5;
            case '6':
                return CardType._6;
            case '7':
                return CardType._7;
            case '8':
                return CardType._8;
            case '9':
                return CardType._9;
            case 'T':
                return CardType.T;
            case 'J':
                return CardType.J;
            case 'Q':
                return CardType.Q;
            case 'K':
                return CardType.K;
            case 'A':
                return CardType.A;
            default:
                return CardType.None;
		}
    }

    static int T2NumValueOf(CardType c)
    {
        if (c == CardType.J) return 1;
        if (c == CardType.None) return 0;
        return ((int)c) + 1;
    }

    enum CardType
	{
        _1 = 1,
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        T = 10,
        J = 11,
        Q = 12,
        K = 13,
        A = 14,
        None = 0
    }

    enum HandType
	{
        HighCard = 1,
        OnePair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        FullHouse = 5,
        FourOfAKind = 6,
        FiveOfAKind = 7,
	}
}