using System;
using System.Collections.Generic;
using GameData;
using GameData.MapElement;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance = null;
    
    private GameDataSource _dataSource;
    private List<GameObject>[][] _gameMap;

    private float _time = 0.0f;
    public float timePerTurn = 0.5f;
    private int _currentTurn = -1;
    public int myAi = -1;//我的阵营编号

    private GameObject _objPollution;
    private GameObject _objBuilding;
    private GameObject _objDetector;
    private GameObject _objProcessor;

    public int CurrentTurn
    {
        get => _currentTurn;
        set
        {
            _currentTurn = value;
            //TODO 通知UI刷新当前回合数的指示
            GameObject.Find("Text").GetComponent<Text>().text = _currentTurn.ToString();
        }
    }

    T FindMapObject<T>(Point position) where T: MonoBehaviour
    {
        foreach (var obj in _gameMap[position.x][position.y])
        {
            var component = obj.GetComponent<T>();
            if (component != null) return component;
        }
        return null;
    }

    void gameEnd()
    {
        
    }
    
    void nextTurn()
    {
        CurrentTurn++;
        if (CurrentTurn >= _dataSource.GetStartData().ActualRoundNum)
        {
            gameEnd();
            return;
        }
        var turnData = _dataSource.GetTurnData(CurrentTurn);
        //TODO 播放事件动画（可能的物体实例化和析构）
    }

    void changeTurn(int turn)
    {
        CurrentTurn = turn;
        _time = 0;
        var turnData = _dataSource.GetTurnData(CurrentTurn);
        //TODO 播放事件动画（可能的物体实例化和析构）
        //全图遍历、1.修改可复用对象的状况，2.删除多余对象。3.添加新加入的对象
    }
    // Start is called before the first frame update
    void Start()
    {
        _dataSource = new GameDataSource();
        Instance = GameObject.Find("GameControl").GetComponent<GameControl>();

        var startData = _dataSource.GetStartData();
        for (int x = 0; x < startData.MapWidth; x++)
        {
            for (int y = 0; y < startData.MapHeight; y++)
            {
                foreach (MapElementBase element in startData.Map[x][y].Elements)
                {
                    GameObject obj = null;
                    if (element is PollutionSource ele1)
                    {
                        var control = Resources.Load<PollutionControl>("Prefabs/Pollution");
                        obj = control.gameObject;
                        control.SyncMapElementStatus(ele1);
                    }else if (element is Building ele2)
                    {
                        var control = Resources.Load<BuildingControl>("Prefabs/Building");
                        obj = control.gameObject;
                        control.SyncMapElementStatus(ele2);
                    }else if (element is Detector ele3)
                    {
                        var control = Resources.Load<DetectorControl>("Prefabs/Detector");
                        obj = control.gameObject;
                        control.SyncMapElementStatus(ele3);
                    }else if (element is Processor ele4)
                    {
                        var control = Resources.Load<ProcessorControl>("Prefabs/Processor");
                        obj = control.gameObject;
                        control.SyncMapElementStatus(ele4);
                    }

                    var pos = obj.transform.position;
                    pos.x = x;
                    pos.z = y;
                    obj.transform.position = pos;
                    
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        var turns = (int)Math.Floor(_time / timePerTurn);
        for (var i = 0; i < turns; i++)
            nextTurn();
        _time -= turns * timePerTurn;
    }
}
