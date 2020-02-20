using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameData;
using GameData.GameEvents;
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

    public StartData StartData;

    /**
     * 当前的播放状态，true为播放中、false为已暂停。
     * 播放器控制部分可直接改变此属性的值。
     */
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            _time = 0.0f;
        }
    }

    private float[] _playSpeedValidRange = new float[] {0.2f, 5.0f};
    
    /**
     * 游戏的播放速度。播放器控制部分可直接改变此属性的值。
     */
    public float PlaySpeed
    {
        get => _playSpeed;
        set
        {
            _time = 0.0f;
            _playSpeed = Mathf.Clamp(value, _playSpeedValidRange[0], _playSpeedValidRange[1]);
        }
    }

    /**
     * 我的阵营编号
     */
    public int MyAi { get; private set; } = -1;
    
    //私有变量部分
    private List<GameObject>[][] _gameMap;
    private float _time = 0.0f;
    private float _standardTimePerTurn = 1.0f;
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
        private set => _currentTurn = value;
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

    public event Action AfterNextTurnEvent;
    
    /**
     * 使游戏切换到下一回合，并正常播放动画。
     * 播放器控制部分在播放过程中其实不需要调用这个函数，
     * 本类会在播放状态为播放中的时候、根据设定的播放倍率自动的调用这个函数的。
     */
    public IEnumerator NextTurn()
    {
        CurrentTurn++;
        if (CurrentTurn >= DataSource.GetStartData().ActualRoundNum)
        {
            gameEnd();
            yield break;
        }
        var turnData = DataSource.GetTurnData(CurrentTurn);
        Debug.Log("Turn " + CurrentTurn + ", Speed " + PlaySpeed + ", Interval " + _standardTimePerTurn / PlaySpeed);
        foreach (GameEventBase eventBase in turnData.Events)
        {
            if (eventBase is NewBidEvent || eventBase is BidResultEvent)
            {
                var obj = FindMapObject<DirtControl>(eventBase.Position);
                obj.Place = eventBase.GetPlace(turnData.Map);
            }
            else if (eventBase is PutProcessorEvent evep)
            {
                var obj = Instantiate(_prefabs["Processor"]);
                obj.transform.position = new Vector3(evep.Position.x, obj.transform.position.y, evep.Position.y);
                _gameMap[evep.Position.x][evep.Position.y].Add(obj);
                var control = obj.GetComponent<ProcessorControl>();
                control.SetModelStatus(ProcessorControl.StatusEnum.NORMAL, evep.Processor, false);
                control.Blocked = Utils.CalculateBlocked(StartData.Map, evep.Processor);
                foreach (var one in evep.Result)
                {
                    var pls = FindMapObject<PollutionControl>(one.Item1.Position);
                    pls.SetModelStatus(PollutionControl.StatusEnum.PROCESSED, one.Item1, false);
                }
            }
            else if (eventBase is PutDetectorEvent eved)
            {
                var obj = Instantiate(_prefabs["Detector"]);
                obj.transform.position = new Vector3(eved.Position.x, obj.transform.position.y, eved.Position.y);
                _gameMap[eved.Position.x][eved.Position.y].Add(obj);
                var control = obj.GetComponent<DetectorControl>();
                control.SetModelStatus(DetectorControl.StatusEnum.NORMAL, eved.Detector, false);
                control.Blocked = Utils.CalculateBlocked(StartData.Map, eved.Detector);
                foreach (var one in eved.Result)
                {
                    var pls = FindMapObject<PollutionControl>(one.Position);
                    pls.SetModelStatus(PollutionControl.StatusEnum.DETECTED, one, false);
                }
            }
            else if (eventBase is TipsterEvent evet)
            {
                if(evet.Result != null)
                {
                    var pollutionSource = turnData.Map[evet.Result.x][evet.Result.y].GetElement<PollutionSource>();
                    var pls = FindMapObject<PollutionControl>(pollutionSource.Position);
                    pls.SetModelStatus(PollutionControl.StatusEnum.DETECTED, pollutionSource, false);   
                }
            }
        }
        AfterNextTurnEvent?.Invoke();
    }

    private void ChangeAPlace(MapPlace mapPlace)
    {
        List<GameObject> oldObjs = _gameMap[mapPlace.Position.x][mapPlace.Position.y];
        List<GameObject> newObjs = new List<GameObject>();
        
        var dirtControl = FindMapObject<DirtControl>(mapPlace.Position);
        oldObjs.Remove(dirtControl.gameObject);
        newObjs.Add(dirtControl.gameObject);
        dirtControl.Place = mapPlace;
        foreach (MapElementBase element in mapPlace.Elements)
        {
            GameObject obj = null;
            if (element is PollutionSource ele1)
            {
                var control = FindMapObject<PollutionControl>(mapPlace.Position);
                if (control != null)
                {
                    obj = control.gameObject;
                    oldObjs.Remove(obj);
                }
                else
                {
                    obj = Instantiate(_prefabs["Pollution"]);
                    control = obj.GetComponent<PollutionControl>();
                }
                control.SyncMapElementStatus(ele1);
            }
            else if (element is Building ele2)
            {
                var control = FindMapObject<BuildingControl>(mapPlace.Position);
                if (control != null)
                {
                    obj = control.gameObject;
                    oldObjs.Remove(obj);
                }
                else
                {
                    obj = Instantiate(_prefabs["Building"]);
                    control = obj.GetComponent<BuildingControl>();
                }
                control.SyncMapElementStatus(ele2);
            }
            else if (element is Detector ele3)
            {
                
                var control = FindMapObject<DetectorControl>(mapPlace.Position);
                if (control != null)
                {
                    obj = control.gameObject;
                    oldObjs.Remove(obj);
                }
                else
                {
                    obj = Instantiate(_prefabs["Detector"]);
                    control = obj.GetComponent<DetectorControl>();
                    control.Blocked = Utils.CalculateBlocked(StartData.Map, ele3);
                }
                control.SyncMapElementStatus(ele3);
            }
            else if (element is Processor ele4)
            {
                var control = FindMapObject<ProcessorControl>(mapPlace.Position);
                if (control != null)
                {
                    obj = control.gameObject;
                    oldObjs.Remove(obj);
                }
                else
                {
                    obj = Instantiate(_prefabs["Processor"]);
                    control = obj.GetComponent<ProcessorControl>();
                    control.Blocked = Utils.CalculateBlocked(StartData.Map, ele4);
                }
                control.SyncMapElementStatus(ele4);
            }
            else throw new Exception("非法element");
            
            newObjs.Add(obj);
        }
        
        foreach (GameObject oldObj in oldObjs)
        {
            Destroy(oldObj);
        }
        _gameMap[mapPlace.Position.x][mapPlace.Position.y] = newObjs;
    }
    
    /**
     * 直接改变游戏的回合状态。不会播放任何过渡动画。
     * 常用于玩家直接改变当前播放回合的情况，播放器控制部分直接调用本函数即可。
     */
    public void ChangeTurn(int turn)
    {
        CurrentTurn = Mathf.Clamp(turn, 0, StartData.ActualRoundNum - 1);
        _time = 0;
        var turnData = DataSource.GetTurnData(CurrentTurn);
        for (int x = 0; x < turnData.Map.Length; x++)
        {
            for (int y = 0; y < turnData.Map[x].Length; y++)
            {
                ChangeAPlace(turnData.Map[x][y]);
            }
        }
        AfterNextTurnEvent?.Invoke();
    }

    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    private bool _isPlaying = true;
    private float _playSpeed = 1.0f;

    void Awake()
    {
        Instance = GameObject.Find("GameControl").GetComponent<GameControl>();
        
        DataSource = new GameDataSource();
        DataSource.ReadFile("播放文件示例.json");

        StartData = DataSource.GetStartData();
        
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
        _prefabs["Wall"] = Resources.Load<GameObject>("Prefabs/Wall");
        
        var wallsObject = new GameObject("Walls");
        wallsObject.transform.position = Vector3.zero;
        for (int x = -1; x <= StartData.MapWidth; x++)
        {
            var ins = Instantiate(_prefabs["Wall"], wallsObject.transform);
            ins.transform.position = new Vector3(x, ins.transform.position.y, -1);
            var ins2 = Instantiate(_prefabs["Wall"], wallsObject.transform);
            ins2.transform.position = new Vector3(x, ins2.transform.position.y, StartData.MapHeight);
        }
        for (int y = 0; y < StartData.MapHeight; y++)
        {
            var ins = Instantiate(_prefabs["Wall"], wallsObject.transform);
            ins.transform.position = new Vector3(-1, ins.transform.position.y, y);
            var ins2 = Instantiate(_prefabs["Wall"], wallsObject.transform);
            ins2.transform.position = new Vector3(StartData.MapWidth, ins2.transform.position.y, y);
        }

        _gameMap = new List<GameObject>[StartData.MapWidth][];
        for (int x = 0; x < StartData.MapWidth; x++)
        {
            _gameMap[x] = new List<GameObject>[StartData.MapHeight];
            for (int y = 0; y < StartData.MapHeight; y++)
            {
                _gameMap[x][y] = new List<GameObject>();
                var dirtObj = Instantiate(_prefabs["Dirt"]);
                var dirtControl = dirtObj.GetComponent<DirtControl>();
                dirtControl.Place = StartData.Map[x][y];
                dirtObj.transform.position = new Vector3(x, dirtObj.transform.position.y, y);
                _gameMap[x][y].Add(dirtObj);
                foreach (MapElementBase element in StartData.Map[x][y].Elements)
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
                        control.Blocked = Utils.CalculateBlocked(StartData.Map, ele3);
                    }else if (element is Processor ele4)
                    {
                        obj = Instantiate(_prefabs["Processor"]);
                        var control = obj.GetComponent<ProcessorControl>();
                        control.SyncMapElementStatus(ele4);
                        control.Blocked = Utils.CalculateBlocked(StartData.Map, ele4);
                    }
                    else throw new Exception("非法element");
                    _gameMap[x][y].Add(obj);
                    obj.transform.position = new Vector3(x, obj.transform.position.y, y);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            IsPlaying = !IsPlaying;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I");
            var qwq = FindMapObject<PollutionControl>(new Point(0, 3));
            qwq.SetModelStatus(PollutionControl.StatusEnum.PROCESSED, null, false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(KeyCode.O);
            PlaySpeed *= 1.5f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(KeyCode.P);
            PlaySpeed /= 1.5f;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(KeyCode.L);
            ChangeTurn(CurrentTurn - 5 >= 0? CurrentTurn - 5 : 0);
        }

        if (IsPlaying)
        {
            _time += Time.deltaTime;
            var turns = (int) Math.Floor(_time / (_standardTimePerTurn / PlaySpeed));
            for (var i = 0; i < turns; i++)
                StartCoroutine(NextTurn());
            _time -= turns * (_standardTimePerTurn / PlaySpeed);
        }
    } 
    
    /**
     * 返回长为2的int数组，依次表示玩家0和1的当前金钱值。
     */
    public int[] GetMoneys()
    {
        if (CurrentTurn == -1) return DataSource.GetStartData().Moneys;
        else return DataSource.GetTurnData(CurrentTurn).Moneys;
    }
    
    /**
     * 返回长为2的int数组，依次表示玩家0和1的当前分数值。
     */
    public int[] GetScores()
    {
        if (CurrentTurn == -1) return DataSource.GetStartData().Scores;
        else return DataSource.GetTurnData(CurrentTurn).Scores;
    }
}
