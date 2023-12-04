using System;
using System.Collections.Generic;

public static class RandomUtils
{
    public static string Choice(this Random rnd, IEnumerable<string> choices, IEnumerable<int> weights)
    {
        var cumulativeWeight = new List<int>();
        int last = 0;
        foreach(var curr in weights)
        {
            last += curr;
            cumulativeWeight.Add(last);
        }
        int choice = rnd.Next(last);
        int i = 0;
        foreach(var curr in choices)
        {
            if(choice < cumulativeWeight[i])
            {
                return curr;
            }
            i++;
        }
        return null;
    }

    public static List<string> Choices(this Random rnd, IEnumerable<string> choices, IEnumerable<int> weights, int maxChoices)
    {
        var result = new List<string>();
        for(int i = 0; i < maxChoices; i++)
        {
            result.Add(rnd.Choice(choices, weights));
        }
        return result;
    }
}
