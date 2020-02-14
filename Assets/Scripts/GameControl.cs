using System;
using System.Collections.Generic;
using GameData;
using GameData.MapElement;
using UnityEngine;

/**
 * 控制游戏全局状态的脚本。
 * 它是单例，类内部提供静态Instance变量用于获得实例，
 */
public class GameControl : MonoBehaviour
{
    public static GameControl Instance { get; private set; } = null;

    /**
     * 游戏数据源对象。
     * 请注意其他人使用此对象时不要对里面的数据内容做任何修改。
     */
    public GameDataSource DataSource;

    /**
     * 当前的播放状态，true为播放中、false为已暂停。
     * 播放器控制部分可直接改变此属性的值。
     */
    public bool IsPlaying { get; set; } = false;

    /**
     * 游戏的播放速度。播放器控制部分可直接改变此属性的值。
     */
    public float PlaySpeed { get; set; } = 1.0f;
    
    /**
     * 我的阵营编号
     */
    public int MyAi { get; private set; } = -1;
    
    //私有变量部分
    private List<GameObject>[][] _gameMap;
    private float _time = 0.0f;
    private float _standardTimePerTurn = 0.5f;
    private int _currentTurn = -1;
    
    /**
     * getter用于获得当前的回合数。
     * setter是private的，如需设置回合请调用nextTurn或ChangeTurn接口。
     *
     * 若要获得本局游戏的总回合数，请使用DataSource.GetStartData().ActualRoundNum
     */
    public int CurrentTurn
    {
        get => _currentTurn;
        private set
        {
            _currentTurn = value;
            //TODO 通知UI刷新当前回合数的指示
        }
    }
    
    /**
     * 找到指定位置上的、带有某一种脚本的游戏对象。
     */
    public T FindMapObject<T>(Point position) where T: MonoBehaviour
    {
        foreach (var obj in _gameMap[position.x][position.y])
        {
            var component = obj.GetComponent<T>();
            if (component != null) return component;
        }
        return null;
    }
    
    private void gameEnd()
    {
        //TODO    
    }
    
    /**
     * 使游戏切换到下一回合，并正常播放动画。
     * 播放器控制部分在播放过程中其实不需要调用这个函数，
     * 本类会在播放状态为播放中的时候、根据设定的播放倍率自动的调用这个函数的。
     */
    public void NextTurn()
    {
        CurrentTurn++;
        if (CurrentTurn >= DataSource.GetStartData().ActualRoundNum)
        {
            gameEnd();
            return;
        }
        var turnData = DataSource.GetTurnData(CurrentTurn);
        //TODO 播放事件动画（可能的物体实例化和析构）
    }

    /**
     * 直接改变游戏的回合状态。不会播放任何过渡动画。
     * 常用于玩家直接改变当前播放回合的情况，播放器控制部分直接调用本函数即可。
     */
    void ChangeTurn(int turn)
    {
        CurrentTurn = turn;
        _time = 0;
        var turnData = DataSource.GetTurnData(CurrentTurn);
        //TODO 播放事件动画（可能的物体实例化和析构）
        //全图遍历、1.修改可复用对象的状况，2.删除多余对象。3.添加新加入的对象
    }
    // Start is called before the first frame update
    
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        Instance = GameObject.Find("GameControl").GetComponent<GameControl>();
        
        DataSource = new TestGameDataSource();
        DataSource.ReadFile("");
        
        //TODO MyAi问题
        MyAi = 0;   
    }

    void Start()
    {
        _prefabs["Dirt"] = Resources.Load<GameObject>("Prefabs/Dirt");
        _prefabs["Pollution"] = Resources.Load<GameObject>("Prefabs/Pollution");
        _prefabs["Building"] = Resources.Load<GameObject>("Prefabs/Building");
        _prefabs["Processor"] = Resources.Load<GameObject>("Prefabs/Processor");
        _prefabs["Detector"] = Resources.Load<GameObject>("Prefabs/Detector");

        var startData = DataSource.GetStartData();
        for (int x = 0; x < startData.MapWidth; x++)
        {
            for (int y = 0; y < startData.MapHeight; y++)
            {
                var dirtObj = Instantiate(_prefabs["Dirt"]);
                var dirtControl = dirtObj.GetComponent<DirtControl>();
                dirtControl.Place = startData.Map[x][y];
                dirtObj.transform.position = new Vector3(x, dirtObj.transform.position.y, y);
                // continue;
                foreach (MapElementBase element in startData.Map[x][y].Elements)
                {
                    GameObject obj = null;
                    if (element is PollutionSource ele1)
                    {
                        obj = Instantiate(_prefabs["Pollution"]);
                        var control = obj.GetComponent<PollutionControl>();
                        control.SyncMapElementStatus(ele1);
                    }else if (element is Building ele2)
                    {
                        obj = Instantiate(_prefabs["Building"]);
                        var control = obj.GetComponent<BuildingControl>();
                        control.SyncMapElementStatus(ele2);
                    }else if (element is Detector ele3)
                    {
                        obj = Instantiate(_prefabs["Detector"]);
                        var control = obj.GetComponent<DetectorControl>();
                        control.SyncMapElementStatus(ele3);
                    }else if (element is Processor ele4)
                    {
                        obj = Instantiate(_prefabs["Processor"]);
                        var control = obj.GetComponent<ProcessorControl>();
                        control.SyncMapElementStatus(ele4);
                    }
                    else throw new Exception("非法element");
                    obj.transform.position = new Vector3(x, obj.transform.position.y, y);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        var turns = (int)Math.Floor(_time / _standardTimePerTurn);
        for (var i = 0; i < turns; i++)
            NextTurn();
        _time -= turns * _standardTimePerTurn;
    } 
    
    /**
     * 返回长为2的int数组，依次表示玩家0和1的当前金钱值。
     */
    public int[] GetMoneys()
    {
        //TODO
        return new int[] {0, 0};
    }
    
    /**
     * 返回长为2的int数组，依次表示玩家0和1的当前分数值。
     */
    public int[] GetScores()
    {
        //TODO
        return new int[] {0, 0};
    }
}
