using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UExtensions{
    public static IEnumerator nextTick(this MonoBehaviour self, Action func){
        yield return new WaitForEndOfFrame();
        func();
    }
}