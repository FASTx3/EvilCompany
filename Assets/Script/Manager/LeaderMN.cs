using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using Assets.HeroEditor.Common.CharacterScripts;

public class LeaderMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._leaderMN = this;
    }

    private JsonData _jsonList;

	public IEnumerator SetLeaderMNData()
    {        
        yield return StartCoroutine(LoadLeaderIndexData());
    }

    public IEnumerator LoadLeaderIndexData()
	{
		TextAsset t = (TextAsset)Resources.Load("leader", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDatLeaderIndex(t.text));
	}

    public IEnumerator SetDatLeaderIndex(string jsonString)
	{
        GameData.Instance._leaderDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setLeader._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setLeader._hp = System.Convert.ToInt64(_jsonList[i]["hp"].ToString());
            GameData.Instance._setLeader._atk = System.Convert.ToInt64(_jsonList[i]["atk"].ToString());
            GameData.Instance._setLeader._def = System.Convert.ToInt64(_jsonList[i]["def"].ToString());
            GameData.Instance._setLeader._cri = (System.Convert.ToInt32(_jsonList[i]["cri"].ToString())*0.01f);
            GameData.Instance._setLeader._bonus1 = (System.Convert.ToInt32(_jsonList[i]["bonus1"].ToString())*0.01f);
            GameData.Instance._setLeader._bonus2 = (System.Convert.ToInt32(_jsonList[i]["bonus2"].ToString())*0.01f);
            GameData.Instance._setLeader._price = System.Convert.ToInt64(_jsonList[i]["price"].ToString());
            GameData.Instance._setLeader._name = _jsonList[i]["name"].ToString();            

			GameData.Instance._leaderDataIndex.Add(GameData.Instance._setLeader._index, GameData.Instance._setLeader); 
        }
        yield return null;  
    }

    public List<Character> _leader_prafab = new List<Character>();
    public GameData.MyLeader _set_leader;

    public int _type;

    public float _hp_percent;
    public long _hp_max;
    public long _atk;
    public long _def;

    public float _cri;
    public float _bonus1;
    public float _bonus2;

    public long _hp_addValue;
    public long _atk_addValue;
    public long _def_addValue;

    public float _cri_addValue;
    public float _bonus1_addValue;
    public float _bonus2_addValue;

    public long _price_addValue;

    public void OnResetLeader()//리더 정보 리셋
    {
        _set_leader = new GameData.MyLeader();

        _set_leader._index = 1;
        //_set_leader._lv = 1;
        if(_set_leader._lv != null) _set_leader._lv.Clear();
        _set_leader._lv = new List<long>();
        for(var i = 0; i < 6; i++) _set_leader._lv.Add(1);

        GameData.Instance._playerData._my_leader = _set_leader;

        OnHpUpdate();
        OnPrice();
    }

    public void OnLoadLeader()
    {
        _type = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._type;

        _hp_addValue = GameData.Instance.StringToLong((GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._hp * 0.15f).ToString("F0"));
        _atk_addValue = GameData.Instance.StringToLong((GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._atk * 0.2f).ToString("F0"));
        _def_addValue = GameData.Instance.StringToLong((GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._def * 0.2f).ToString("F0"));

        _cri_addValue = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._cri * 0.005f;
        _bonus1_addValue = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._bonus1 * 0.002f;
        _bonus2_addValue = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._bonus2 * 0.002f;

        _price_addValue = GameData.Instance.StringToLong((GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._price * 0.2f).ToString("F0"));

        OnLeaderStatus();
        OnPrice();
    }

    public void OnLevelUp(int code)
    {
        if(GameData.Instance._battleMN._hit_trigger) return;

        GameData.Instance._battleMN._battle[0].OnBattlePause();

        _set_leader = GameData.Instance._playerData._my_leader;
        _set_leader._lv[code] += 1;
        GameData.Instance._playerData._my_leader = _set_leader;

        //OnHpUpdate();
        //OnLeaderStatus();
        switch(code)
        {
            case 0 :
                OnHp();
                OnHpUpdate();
            break;

            case 1 :
                OnAtk();
            break;

            case 2 :
                OnDef();
            break;

            case 3 :
                OnCri();
            break;

            case 4 :
                OnBonus1();
            break;

            case 5 :
                OnBonus2();
            break;
        }

        OnPriceUpdate(code);
        _upgrade[code].OnRefresh();

        GameData.Instance._battleMN._battle[0].OnBattleRestart();
    }

    public void OnLeaderStatus()
    {
        OnHp();
        OnAtk();
        OnDef();
        OnCri();
        OnBonus1();
        OnBonus2();

    }

    public void OnHpUpdate()
    {
        GameData.Instance._playerData._leader_hp = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._hp + (_hp_addValue*(GameData.Instance._playerData._my_leader._lv[0]-1));
    }

     public void OnHp()
    {        
        _hp_max = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._hp + (_hp_addValue*(GameData.Instance._playerData._my_leader._lv[0]-1));
        _hp_percent = 1/(float)_hp_max;
    }

    public void OnAtk()
    {
        _atk = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._atk + (_atk_addValue*(GameData.Instance._playerData._my_leader._lv[1]-1));
    }

    public void OnDef()
    {
        _def = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._def + (_def_addValue*(GameData.Instance._playerData._my_leader._lv[2]-1));
    }

    public void OnCri()
    {
        _cri = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._cri + (_cri_addValue*(GameData.Instance._playerData._my_leader._lv[3]-1));
    }

    public void OnBonus1()
    {
        _bonus1 = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._bonus1 + (_bonus1_addValue*(GameData.Instance._playerData._my_leader._lv[4]-1));
    }

    public void OnBonus2()
    {
        _bonus2 = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._bonus2 + (_bonus2_addValue*(GameData.Instance._playerData._my_leader._lv[5]-1));
    }

    public void OnPrice()
    {
        //_price = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._price + (_price_addValue*(GameData.Instance._playerData._my_leader._lv-1));
        for(var i = 0; i < 6; i++)
        {
            if(!_price.Contains(i)) _price.Add(0);
            OnPriceUpdate(i);
        }
    }

    public void OnPriceUpdate(int code)
    {
        _price[code] = GameData.Instance._leaderDataIndex[GameData.Instance._playerData._my_leader._index]._price + (_price_addValue*(GameData.Instance._playerData._my_leader._lv[code]-1));
    }

    public List<Text> _text = new List<Text>();

    public Transform _leader_parent;
    public LeaderUpgrade _upgrade_ori;    
    public List<LeaderUpgrade> _upgrade = new List<LeaderUpgrade>();

    public List<long> _price = new List<long>();

    public void OpenLeader()
    {
        if(!GameData.Instance._popMN.OnMainPop(1)) return;

        _leader_parent.localPosition = Vector3.zero;    
        for(var i = 0; i < 6; i++) CreateUpgrade(i);
    }

    public void CloseLeader()
    {
        for(var i = 0; i < _upgrade.Count; i++) Destroy(_upgrade[i].gameObject);
        _upgrade.Clear();
    }

    public void CreateUpgrade(int index)
    {
        LeaderUpgrade obj = null;
        obj = Instantiate<LeaderUpgrade>(_upgrade_ori, _leader_parent);
        obj.transform.localScale = Vector3.one;
        obj.OnSet(index);

        _upgrade.Add(obj);
    }
}
