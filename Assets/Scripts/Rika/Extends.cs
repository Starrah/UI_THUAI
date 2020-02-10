using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UExtensions{
    private static IEnumerator _nextTick(this MonoBehaviour self, Action func){
        yield return new WaitForEndOfFrame();
        func();
    }

    private static IEnumerator _nextTick(this MonoBehaviour self, IEnumerator func){
        yield return new WaitForEndOfFrame();
        self.StartCoroutine(func);
    }

    public static void nextTick(this MonoBehaviour self, Action func){
        self.StartCoroutine(_nextTick(self, func));
    }

    public static void nextTick(this MonoBehaviour self, IEnumerator func){
        self.StartCoroutine(_nextTick(self, func));
    }
}