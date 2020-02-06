using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquip : MonoBehaviour {
    public Scan scan { get; private set; }

    public void Settle(DeviceRangeTypes rangeType) {

        var Animation = GetComponent<Animation>();

        Animation.Play("in");
        Animation.PlayQueued("idle", QueueMode.CompleteOthers);
        scan = Instantiate(FindObjectOfType<GlobalConstants>().GetScan(rangeType));
        scan.transform.position = new Vector3(scan.transform.position.x, .01f, scan.transform.position.z);
        scan.ScanColor = new Color(.5f, 1, .5f, .8f);
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
