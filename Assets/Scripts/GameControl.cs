using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance = null;
    
    private GameDataSource _dataSource;
    private List<GameObject>[][] _map;

    private float _time = 0.0f;
    public float timePerTurn = 0.5f;
    private int _currentTurn = -1;
    public int myAi = -1;//我的阵营编号

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

    T FindMapObject<T>(Vector2Int position) where T: MonoBehaviour
    {
        foreach (var obj in _map[position.x][position.y])
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
    }

    void changeTurn(int turn)
    {
        CurrentTurn = turn;
        _time = 0;
        var turnData = _dataSource.GetTurnData(CurrentTurn);
    }
    // Start is called before the first frame update
    void Start()
    {
        _dataSource = new GameDataSource();
        Instance = GameObject.Find("GameControl").GetComponent<GameControl>();
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
