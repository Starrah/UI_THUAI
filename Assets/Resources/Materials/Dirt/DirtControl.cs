using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DirtControl : MonoBehaviour {
    private MapPlace _place = new MapPlace(new Point(0, 0));
    private MeshRenderer _meshRenderer;
    private Material _material;

    private Color _color {
        get {
            // var c = (int) (_place.Position.x + _place.Position.y) % 2 == 0 ? Color.white : new Color(.8f, .8f, .8f);
            var c=Color.white;
            // if (_place.Type == MapPlaceTypes.BOUGHT)
            //     c *= (_place.Owner == 0 ? new Color(1, .6f, .6f) : new Color(.6f, .6f, 1));
            return c;
        }
    }

    private void updateBidTurn(){
        if (_place.Type != MapPlaceTypes.EMPTY)
            GameControl.Instance.AfterNextTurnEvent -= updateBidTurn;
        else
            GetComponentInChildren<TextMeshPro>().text =
                _place.Bid != null?(_place.Bid.turn + 5 - GameControl.Instance.CurrentTurn).ToString():"";
    }

    public MapPlace Place {
        get => _place;
        set {
            _place = value;
            transform.position = new Vector3(value.Position.x, 0, value.Position.y);
            //如果出价状态非空（即至少进入拍卖环节
            if (!(value.Bid is null)){
                var tmp=GetComponentInChildren<TextMeshPro>();
                if (tmp is null)
                    Instantiate(Resources.Load<TextMeshPro>("Materials/Dirt/Num"), transform);
                var ai = value.Bid.Ai;
                tmp = GetComponentInChildren<TextMeshPro>();
                tmp.color = new Color(ai == 0 ? 1 : 0, 0, ai == 1 ? 1 : 0, .5f);
                switch (value.Type){
                    case MapPlaceTypes.CANNOT_BUY:
                        break;
                    case MapPlaceTypes.EMPTY:
                        //如果还没拍成
                        GameControl.Instance.AfterNextTurnEvent += updateBidTurn;
                        break;
                    case MapPlaceTypes.BOUGHT_FAILED:
                        tmp.text = "×";
                        tmp.color = new Color(value.Bid.Ai == 0 ? 1 : 0, 0, value.Bid.Ai == 1 ? 1 : 0, .5f);
                        break;
                    case MapPlaceTypes.BOUGHT:
                        //如果买到手了
                        tmp.text = "■";
                        tmp.color = new Color(value.Bid.Ai == 0 ? 1 : 0, 0, value.Bid.Ai == 1 ? 1 : 0, .5f);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_material) _material.color = _color;
        }
    }

    private void Start(){
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = new Material(_meshRenderer.material);
        _meshRenderer.material = _material;
        _material.mainTextureOffset = new Vector2(Random.Range(0, 1f), Random.Range(0, 1f));
        _material.color = _color;
    }
}