using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CSharp;
public class DataParse
{
    public static void parse(string logFile)
    {
        var rounds = JsonUtility.FromJson<dynamic>(logFile);
        Debug.Log(rounds);
    }
}
