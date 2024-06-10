using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


// Asks a cost to test the pay function
while (true) // keeps prompting the user for tests;
{
    Console.WriteLine("Input cost:");
    string cost = Console.ReadLine();
    Console.WriteLine("Input payment:");
    string payment = Console.ReadLine();

    HandlePayment(cost, payment);
}


    static void HandlePayment(string cost, string payment)
    {
        // Test if cost is valid
        if (!IsValidCost(cost, payment)) { return; }

        Console.WriteLine(PayCost(cost, payment)); //BUG: hybrid costs and payment sets may be invalid due to order (haven't found a reapeatable bug yet)
    }


    static string PayCost(string cost, string payment)
    {
        var (gCost, cCost, hCost) = SplitCost(cost);
        var (gPay, cPay, hPay) = SplitCost(payment);

        foreach (var key in cCost.Keys){if (cCost[key] > 0) {
                              (cCost, cPay, hPay) = ProcessColorPayment(cCost, cPay, hPay); break;}}        // Pay for color cost
        if (hCost.Count > 0) {(hCost, cPay, hPay) = ProcessHybridPayment(hCost, cPay, hPay);}               // Pay for hybrid cost
        if (gCost > 0) {(gCost, gPay, cPay, hPay) = ProcessGenericPayment(gCost, gPay, cPay, hPay);}        // pay for generic cost

        /* Debugging code
        Console.WriteLine($"Color cost left: {string.Join(", ", cCost)}  Color payment left: {string.Join(", ", cPay)}");
        Console.WriteLine($"Generic cost left: {gCost} Generic payment left: {gPay}");
        Console.WriteLine($"Hybrid cost left: {string.Join(", ", hCost)} Hybrid payment left: {string.Join(", ", hPay)}");*/

        //returns a string with the leftovers after the cost has been paid
        return CombineCost(gPay, cPay, hPay);
    }

    static (Dictionary<char, int>, Dictionary<char, int>, List<string>) ProcessColorPayment (Dictionary<char, int> colorCost, Dictionary<char, int> colorPayment, List<string> hybridPayment)
    {
        var tempToRemove = new List<string>();

        foreach (var key in colorCost.Keys) // pay color cost
        {
            if (colorPayment.ContainsKey(key)) // use colors to pay if we have a match
            {
                while (colorCost[key] > 0 && colorPayment[key] > 0)
                {
                    colorCost[key]--;
                    colorPayment[key]--;
                }
            }

            if (colorCost[key] > 0 && hybridPayment.Count > 0) // use hybrid color payments if we have to
            {
                foreach (var set in hybridPayment)
                {
                    foreach (var c in set) // each hybrid colors are counted as a set. If one of the character matches, mark the set for removal
                    {
                        if (c == key && colorCost[key] > 0) // second check if key > 0 necessary to avoid removing a key count more than once if there are multiple hybrid sets
                        {
                            colorCost[key]--;
                            tempToRemove.Add(set);
                            break;
                        }
                    }
                }

                if (tempToRemove.Count > 0)
                {
                    foreach (var temp in tempToRemove)
                    {
                        hybridPayment.Remove(temp);
                    }
                    tempToRemove.Clear();
                }
            }
        }

        return (colorCost, colorPayment, hybridPayment);
    }

    static (List<string>, Dictionary<char, int>, List<string>) ProcessHybridPayment (List<string> hybridCost, Dictionary<char, int> colorPayment, List<string> hybridPayment)
    {
        var tempToRemove = new List<string>();

        foreach (var set1 in hybridCost) // use colors first
        {
            foreach (var key1 in colorPayment.Keys)
            {
                if (set1.Contains(key1) && colorPayment[key1] > 0)
                {
                    colorPayment[key1]--;
                    tempToRemove.Add(set1);
                    break;
                }
            }
        }

        if (tempToRemove.Count > 0)
        {
            foreach (var temp1 in tempToRemove)
            {
                hybridCost.Remove(temp1);
            }
            tempToRemove.Clear();
        }
        

        var hPayTempToRemove = new List<string>();
        if (hybridCost.Count > 0 && hybridPayment.Count > 0) // use hybrid colors
        {
            foreach (var hybridCostSet in hybridCost)
            {
                foreach (var hybridPaySet in hybridPayment)
                {
                    foreach (var c in hybridPaySet)
                    {
                        if (hybridCostSet.Contains(c))
                        {
                            tempToRemove.Add(hybridCostSet);
                            hPayTempToRemove.Add(hybridPaySet);
                            break;
                        }
                    }
                }
            }

            if (tempToRemove.Count > 0)
            {
                foreach (var temp2 in tempToRemove)
                {
                    hybridCost.Remove(temp2);
                }

                foreach (var temp3 in hPayTempToRemove)
                {
                    hybridPayment.Remove(temp3);
                }
            }
        }

        return (hybridCost, colorPayment, hybridPayment);
    }

    static (int, int, Dictionary<char, int>, List<string>) ProcessGenericPayment (int genericCost, int genericPayment, Dictionary<char, int> colorPayment, List<string> hybridPayment)
    {
        var tempToRemove = new List<string>();

        Console.WriteLine("initial generic cost:" + genericCost.ToString() + " initial generic payment" + genericPayment.ToString());
        
        while (genericCost > 0 && genericPayment > 0) // pay generic cost using generic payment
        {
            genericCost--;
            genericPayment--;
        }

        foreach (var k in colorPayment.Keys) // use colors to pay for generic
        {
            while (genericCost > 0 && colorPayment[k] > 0)
            {
                genericCost--;
                colorPayment[k]--;
            }
        }

        foreach (var set in hybridPayment) // use hybrid to pay generic
        {
            if (genericCost > 0)
            {
                genericCost--;
                tempToRemove.Add(set);
            }
        }

        foreach (var temp2 in tempToRemove)
        {
            hybridPayment.Remove(temp2);
        }

        Console.WriteLine("generic cost:" + genericCost.ToString() + " generic payment" + genericPayment.ToString());
        return (genericCost, genericPayment, colorPayment, hybridPayment);
    }

    // Split the cost into 3 types to be able to pay it properly (returns upper characters)
    static (int, Dictionary<char, int>, List<string>) SplitCost(string cost)
    {
        var colorCost = new Dictionary<char, int>() { { 'W', 0 }, { 'U', 0 }, { 'B', 0 }, { 'R', 0 }, { 'G', 0 } };
        int genericCost = 0;
        string numberString = "";
        var hybridCost = new List<string>();
        bool inHybrid = false;
        string tempHColors = "";

        foreach (char c in cost)
        {
            if (c == '/')
            {
                inHybrid = true;
            }
            else if (c == '\\')
            {
                inHybrid = false;
                hybridCost.Add(tempHColors.ToUpper());
                tempHColors = "";
            }
            else if (inHybrid)
            {
                tempHColors += c;
            }
            else
            {
                if (char.IsDigit(c))
                {
                    numberString += c;
                }
                else
                {
                    colorCost[char.ToUpper(c)]++;
                }
            }
        }
        genericCost = int.Parse(numberString);
        return (genericCost, colorCost, hybridCost);
    }
    
    // Takes the split types (from SplitCost) and recombines them into a string
    static string CombineCost(int generic, Dictionary<char, int> color, List<string> hybrid)
    {
        string s_generic = "";
        string s_color = "";
        string s_hybrid = "";

        if (generic > 0)
        {
            s_generic = generic.ToString();
        }
        
        if (color != null)
        {
            foreach (var key in color)
            {
                if (key.Value > 0)
                {
                    s_color += key.Key;
                }
            }
        }

        if (hybrid != null)
        {
            foreach(var c in hybrid)
            {
                s_hybrid += "/" + c + "\\";
            }
        }

    return s_generic + s_color + s_hybrid;

    }


    static bool IsValidCost(params string[] args)
    {
        foreach (var arg in args)
        {
            foreach (char c in arg)
            {
                if (!IsAcceptedColor(c))
                {
                    Console.WriteLine($"Unrecognized energy color {c}!");
                    return false;
                }
                if (c == ('/'))
                {
                    if (!IsValidHybridColor(arg))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    static bool IsAcceptedColor(char sColor)
    {
        string acceptedColors = "WUBRG0123456789/\\";
        return acceptedColors.Contains(char.ToUpper(sColor));
    }

    static bool IsValidHybridColor(string sCost)
    {
        bool start = false;
        bool end = false;
        var hybridPart = new List<string>();
        string tempChar = "";
        foreach (char c in sCost)
        {
            if (c == '/')
            {
                start = true;
            }
            else if (c == '\\')
            {
                end = true;
                hybridPart.Add(tempChar);
                tempChar = "";
            }
            else if (start)
            {
                tempChar += c;
            }
        }

        foreach (var part in hybridPart)
        {
            if (part.Distinct().Count() != part.Length)
            {
                Console.WriteLine("Hybrid character set has duplicates!");
                return false;
            }
        }

        if ((start && !end) || (end && !start))
        {
            Console.WriteLine("Incomplete hybrid character set!");
            return false;
        }

        return true;
    }
