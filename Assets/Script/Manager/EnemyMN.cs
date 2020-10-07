using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Assets.HeroEditor.Common.CharacterScripts;

public class EnemyMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._enemyMN = this;
    }

    private JsonData _jsonList;
    public Dictionary<int, List<int>> _map_enemy = new Dictionary<int, List<int>>();
    public Dictionary<int, int> _map_boss = new Dictionary<int, int>();

	public IEnumerator SetEnemyMNData()
    {        
        yield return StartCoroutine(LoadEnemyIndexData());
    }

    public IEnumerator LoadEnemyIndexData()
	{
		TextAsset t = (TextAsset)Resources.Load("enemy", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDatEnemyIndex(t.text));
	}

    public IEnumerator SetDatEnemyIndex(string jsonString)
	{
        GameData.Instance._enemyDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setEnemy._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setEnemy._hp = System.Convert.ToInt64(_jsonList[i]["hp"].ToString());
            GameData.Instance._setEnemy._atk = System.Convert.ToInt64(_jsonList[i]["atk"].ToString());
            GameData.Instance._setEnemy._def = (System.Convert.ToInt32(_jsonList[i]["def"].ToString())*0.1f);
            GameData.Instance._setEnemy._reward = System.Convert.ToInt64(_jsonList[i]["reward"].ToString());
            GameData.Instance._setEnemy._type = System.Convert.ToInt32(_jsonList[i]["type"].ToString());
            GameData.Instance._setEnemy._map = System.Convert.ToInt32(_jsonList[i]["map"].ToString());
            GameData.Instance._setEnemy._class = System.Convert.ToInt32(_jsonList[i]["class"].ToString());
            GameData.Instance._setEnemy._rate = (System.Convert.ToInt32(_jsonList[i]["rate"].ToString())*0.01f);
            GameData.Instance._setEnemy._name = _jsonList[i]["name"].ToString();

            GameData.Instance._setEnemy._hp_addValue = System.Convert.ToInt32(_jsonList[i]["plus1"].ToString());
            GameData.Instance._setEnemy._atk_addValue = System.Convert.ToInt32(_jsonList[i]["plus2"].ToString());

            if(GameData.Instance._setEnemy._class == 2) _map_boss.Add(GameData.Instance._setEnemy._map, GameData.Instance._setEnemy._index);
            
            if(!_map_enemy.ContainsKey(GameData.Instance._setEnemy._map)) _map_enemy.Add(GameData.Instance._setEnemy._map, new List<int>());
            _map_enemy[GameData.Instance._setEnemy._map].Add(GameData.Instance._setEnemy._index);

			GameData.Instance._enemyDataIndex.Add(GameData.Instance._setEnemy._index, GameData.Instance._setEnemy); 
        }
        yield return null;  
    }


    float _enemy_total = 0;
    List<float> _enemy_rate = new List<float>();
    List<int> _enemy = new List<int>();
    public void SetEnemyRate(int map)//불러올 몬스터 확률 정리
    {
        _enemy_total = 0;
        _enemy_rate.Clear();
        _enemy.Clear();

        for(var i = 0; i < _map_enemy[map].Count; i++)
        {            
            if(GameData.Instance._enemyDataIndex[_map_enemy[map][i]]._rate <= 0 ) continue;

            _enemy_total += GameData.Instance._enemyDataIndex[_map_enemy[map][i]]._rate;
            _enemy_rate.Add(_enemy_total);
            _enemy.Add(_map_enemy[map][i]);
        }
    }

    public long _hp;
    public long _atk;
    public long _def;
    public long _reward;
    public int _type;

    public long _hp_max;
    public float _hp_percent;

    public void OnEnemy()
    {
        
        int code = 1;
        float _rate_enemy = Random.Range(0f, _enemy_total);

        if(GameData.Instance._playerData._round < 5)
        {
            for(var i = 0; i < _enemy_rate.Count; i++)
            {
                if(_rate_enemy < _enemy_rate[i])
                {                    
                    code = _enemy[i];
                    break;
                }
            }
        }
        else
            code = _map_boss[GameData.Instance._playerData._map];

        GameData.Instance._battleMN._enemy_code = code;        

        _hp = GameData.Instance._enemyDataIndex[code]._hp + (GameData.Instance._enemyDataIndex[code]._hp_addValue * (GameData.Instance._playerData._stage-1));
        _atk = GameData.Instance._enemyDataIndex[code]._atk + (GameData.Instance._enemyDataIndex[code]._atk_addValue * (GameData.Instance._playerData._stage-1));
        _def = GameData.Instance.StringToLong((GameData.Instance._enemyDataIndex[code]._def * GameData.Instance._playerData._stage).ToString("F0"));
        _reward = GameData.Instance._enemyDataIndex[code]._reward * GameData.Instance._playerData._stage;

        _type = GameData.Instance._enemyDataIndex[code]._type; 
        
        _hp_max = _hp;
        _hp_percent = 1/(float)_hp;   
    }

    public List<Character> _enemy_prafab = new List<Character>();
}
