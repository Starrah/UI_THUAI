﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test : MonoBehaviour {
    public Animation Animation;
    // Start is called before the first frame update
    void Start() {
        Animation.Play("in");
        Animation.PlayQueued("idle");


    }


    // Update is called once per frame
    void Update() {

    }
}
