using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._battleMN = this;
        OffStage();
    }

    public Transform _battle_parent;
    public Battle _battle_ori;
    public List<Battle> _battle = new List<Battle>(); 
    
    public void OnLoadBattle()
    {
        Battle obj = null;
        obj = Instantiate<Battle>(_battle_ori, _battle_parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet();

        _battle.Add(obj);

        GameData.Instance._enemyMN.SetEnemyRate(GameData.Instance._playerData._map);
    }

    public Material _map_ori;
    public List<Text> _text = new List<Text>();

    public int _enemy_code;

    public void OnResetStage()
    {
        GameData.Instance._playerData._map = 1;
        GameData.Instance._playerData._stage = 1;
        GameData.Instance._playerData._round = 1;
    }

    List<string> _map_name = new List<string>{"초원 지대 ({0}/5)", "암석 지대 ({0}/5)", "용암 지대 ({0}/5)", "방산 지대 ({0}/5)", "해안 지대 ({0}/5)"};
    public void OnStage()
    {
        _text[0].text = string.Format(_map_name[GameData.Instance._playerData._map-1], GameData.Instance._playerData._round);
    }

    public void OffStage()
    {
        //_team_hp.gameObject.SetActive(false);
        _enemy_hp.gameObject.SetActive(false);
        _text[0].text = "";
    }

    public void OnEnemySet()
    {
        GameData.Instance._enemyMN.OnEnemy();
        _battle[0].OnEnemySet(_enemy_code);    
    }

    public void OnBattleStart()
    {
        OnStage();
        OnEnemySet();
        _battle[0].OnBattle();

        OnTeamHp();
    }

    public void OnBattleStop()
    {
        
    }

    public void OnNextStage()
    {        
        bool next_stage = false;
      
        GameData.Instance._playerData._round += 1;

        if(GameData.Instance._playerData._round > 5)
        {
            GameData.Instance._playerData._round = 1;
            GameData.Instance._playerData._stage += 1;                                         

            // if(GameData.Instance._playerData._stage % 10 <= 0) 
            // {                    
                GameData.Instance._playerData._map = Random.Range(1, GameData.Instance._enemyMN._map_boss.Count+1);   
                GameData.Instance._enemyMN.SetEnemyRate(GameData.Instance._playerData._map);                
            // }
        
            next_stage = true;
        }

        if(next_stage) _battle[0].OnWinNext(); //다음 맵으로 이동
        else OnBattleStart();
    }
    public bool _hit_trigger;
    public void OnHit()
    {
        _hit_trigger = true;
        /*
        if(GameData.Instance._leaderMN._def < GameData.Instance._enemyMN._atk)
            GameData.Instance._playerData._leader_hp -= (GameData.Instance._enemyMN._atk - GameData.Instance._leaderMN._def);
            //GameData.Instance._leaderMN._hp -= (GameData.Instance._enemyMN._atk - GameData.Instance._leaderMN._def);
        */
        long _atk = 0;
        float _cri_rate = Random.Range(0f, 1f);

        if(_cri_rate <= GameData.Instance._leaderMN._cri) _atk = GameData.Instance.StringToLong((GameData.Instance._leaderMN._atk * 1.2f).ToString("F0"));
        else _atk = GameData.Instance._leaderMN._atk;

        if(GameData.Instance._enemyMN._type == GameData.Instance._leaderMN._type) _atk -= GameData.Instance._enemyMN._def;

        GameData.Instance._enemyMN._hp -= _atk;
        OnEnemyHp();
        OnTeamHp();
        _hit_trigger = false;
    }

    public void OnGameReset()
    {
        OnResetStage();
        GameData.Instance._leaderMN.OnResetLeader();
        GameData.Instance._leaderMN.OnLeaderStatus();
    }
    
    public Slider _team_hp;
    public Slider _enemy_hp;

    public void OnEnemyHp()
    {
        if(GameData.Instance._enemyMN._hp <= 0)
        {
            _enemy_hp.gameObject.SetActive(false);
            return;
        }

        if(!_enemy_hp.gameObject.activeSelf) _enemy_hp.gameObject.SetActive(true);

        _enemy_hp.value = GameData.Instance._enemyMN._hp * GameData.Instance._enemyMN._hp_percent;
        _text[2].text = string.Format("{0} / {1}", GameData.Instance._enemyMN._hp, GameData.Instance._enemyMN._hp_max);
    }

    public void OnTeamHp()
    {
        if(!_team_hp.gameObject.activeSelf) _team_hp.gameObject.SetActive(true);

        _team_hp.value = GameData.Instance._playerData._leader_hp * GameData.Instance._leaderMN._hp_percent;
        _text[1].text = string.Format("{0} / {1}", GameData.Instance._playerData._leader_hp, GameData.Instance._leaderMN._hp_max);
    }
}


