using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static IEnumerator nectTick(this MonoBehaviour self, Action func) {
        yield return new WaitForEndOfFrame();
        func();
    }
}