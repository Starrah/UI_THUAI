using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataParse.parse(File.ReadAllText(@"D:\学校\大学\软院\UI_THUAI\Test.json"));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
