using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.HeroEditor.Common.CharacterScripts;

public class Battle : MonoBehaviour
{
    public Slider _team_hp_guage;
    public Transform _team;
    public Dictionary<int, Character> _obj_team_member = new Dictionary<int, Character>();
    public List<Transform> _team_pos = new List<Transform>();
    public Character _leader_model;
    public Transform _leader_pos;    

    public Transform _parent_monster;
    public Transform _now_monster;
    public Transform _death_monster;
    public List<Monster> _monster = new List<Monster>();
    public long _monster_hp;
    public long _monster_hp_max;
    public float _monster_hp_percent;
    public long _monster_atk;

    public bool _rest = false;
    public bool _map_move = false;
    public string _tweenId;

    public Image _map;
    public Image _fade;
    private Material thisMaterial;
    Vector2 newOffset;
    
    void Start()
    {
        _map.material = Instantiate<Material>(GameData.Instance._battleMN._map_ori);
        thisMaterial = _map.material;
        newOffset = thisMaterial.mainTextureOffset;
    }
        
    public int _monster_obj_code = 0;

    public void OnSet()//파티 세팅
    {
        if(_leader_model != null) Destroy(_leader_model.gameObject);

        _leader_model = Instantiate<Character>(GameData.Instance._leaderMN._leader_prafab[GameData.Instance._playerData._my_leader._index], _leader_pos);
        _leader_model.transform.localPosition = Vector3.zero;
        _leader_model.transform.localScale = new Vector3(80, 80, 1);
    }

    public void OnEnemySet(int _enemy_code)//적 호출
    {
        if(_monster_obj_code >= _monster.Count) _monster_obj_code = 0;
                
        if(_monster.Count < 1)
        {
            for(var i = 0; i < _parent_monster.childCount; i++)
            {
                _monster.Add(_parent_monster.GetChild(i).GetComponent<Monster>());
            }
        }

        _now_monster = _monster[_monster_obj_code].transform;
        _monster[_monster_obj_code].OnSet(_enemy_code);

        _monster_obj_code++;
    }

    public void OnBattle()
    {
        if(!_map_move) _map_move = true;
        OnUnitAnimation();

        _tweenId = "0MoveEnemy";                              
        _now_monster.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.Linear).SetId(_tweenId).OnComplete(() =>
        {
            OnReward();
        });
    }

    public void OnReward()
    {      
        if(_now_monster == null) return;

        _map_move = false;

        GameData.Instance._battleMN.OnHit();

        if(GameData.Instance._playerData._leader_hp <= 0)//원정대 패배   
        {
            OnLose();
        }
        else
        {
            if(GameData.Instance._enemyMN._hp <= 0)
            {
                //몬스터 격파
                OnWin();
            }
            else
            {
                //누구도 죽지않는 상황                
                OnDraw();
            }
        }
    }

    public void OnWin()//전투 승리
    {
        if(_now_monster == null) return;

        _death_monster = _now_monster;
        _death_monster.DOLocalJump(new Vector3(500, 0, 0), 350f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _death_monster.gameObject.SetActive(false);                      
        });
        
        GameData.Instance._battleMN.OnNextStage();
        GameData.Instance._gm.GetGold(GameData.Instance._enemyMN._reward);
    }

    void OnLose()//전투 패배
    {
        if(_now_monster == null) return;

        GameData.Instance._battleMN.OffStage();

        _leader_model.Animator.SetBool("Run", false);
        //몬스터 위치 초기화
        _now_monster.localPosition = new Vector3(500, 0, 0);
        _now_monster.gameObject.SetActive(false);
        
        _leader_model.Animator.SetBool("DieBack", true);

        _fade.DOFade(1, 1f).SetEase(Ease.Linear).SetId("fade").OnComplete(() =>
        {
            OnSet();
            GameData.Instance._battleMN.OnGameReset();
            OnMapSet();            

            _fade.DOFade(0, 0.5f).SetDelay(0.1f).SetEase(Ease.Linear).SetId("fade").OnComplete(() =>
            {
                GameData.Instance._battleMN.OnBattleStart();
            });
        }); 
        
    }

    void OnDraw()//교착
    {
        _map_move = false;
        //파티 넉백
        _team.DOLocalJump(new Vector3(-100, 0, 0), 10f, 1, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _team.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.Linear);
        });
        //몬스터 넉백
        if(_now_monster == null) return;

        _tweenId = "1MoveEnemy";  
        _now_monster.DOLocalJump(new Vector3(100, 0, 0), 20f, 1, 0.15f).SetEase(Ease.Linear).SetId(_tweenId).OnComplete(() =>
        {
            _tweenId = "2MoveEnemy";  
            _now_monster.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.Linear).SetId(_tweenId).OnComplete(() =>
            {
                OnReward();
            });
        });
    }

    public void OnWinNext()//다음 스테이지
    {
        if(_now_monster == null) return;

        GameData.Instance._battleMN.OffStage();

        _team.DOLocalMove(new Vector3(600, 0, 0), 0.95f).SetEase(Ease.Linear);

        _death_monster = _now_monster;
        _death_monster.DOLocalJump(new Vector3(500, 0, 0), 350f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _death_monster.gameObject.SetActive(false);                      
        });

        _fade.DOFade(1, 1f).SetEase(Ease.Linear).SetId("fade").OnComplete(() =>
        {
            OnMapSet();

            _fade.DOFade(0, 0.5f).SetDelay(0.1f).SetEase(Ease.Linear).SetId("fade").OnComplete(() =>
            {
                GameData.Instance._battleMN.OnBattleStart();
            });
        });         
    }

    public void OnBattleStop()
    {
        _map_move = false;

        if(_now_monster == null) return;

        DOTween.Kill(_tweenId);
        
        _now_monster.localPosition = new Vector3(500, 0, 0);
        _now_monster.gameObject.SetActive(false);     
    }

    public void OnBattlePause()
    {
        _map_move = false;

        if(_now_monster == null) return;

        DOTween.Pause(_tweenId);     
    }

    public void OnBattleRestart()
    {
        if(_tweenId == "0MoveEnemy") _map_move = true;
        DOTween.Play(_tweenId);
    }

    public float _time = 0;
    void Update()
    {
        if(_rest)
        {
            _time += GameData.Instance._gm._timeRate;
            if(_time >= 1)
            {
                _time = 0;
            }
        }

        if(_map_move) OnMoveMap();
    }

    float scrollSpeed = 0.75f;//맵이동 속도
    public void OnMoveMap()//맵 이동 연출
    {
        newOffset = thisMaterial.mainTextureOffset;
        newOffset.Set(newOffset.x + (scrollSpeed * GameData.Instance._gm._timeRate), 0);
        thisMaterial.mainTextureOffset = newOffset;
    }

    public void OnMapSet()
    {    
        _leader_model.Animator.SetBool("Run", false);
        
        for(var i = 1; i < _obj_team_member.Count+1; i++)
        {
            if(_obj_team_member[i] != null) _obj_team_member[i].Animator.SetBool("Run", false);
        }

        newOffset = thisMaterial.mainTextureOffset;
        newOffset.Set(0, 0);
        thisMaterial.mainTextureOffset = newOffset;

        _team.localPosition = Vector3.zero;
    }

    public void OnUnitAnimation()
    {
        _leader_model.Animator.SetBool("Run", true);
        
        for(var i = 1; i < _obj_team_member.Count+1; i++)
        {
            if(_obj_team_member[i] != null) _obj_team_member[i].Animator.SetBool("Run", true);
        }        
    }

    public void OnMemberSet(int index)
    {
        if(!_obj_team_member.ContainsKey(index)) _obj_team_member.Add(index, null);
        if(_obj_team_member[index] != null) Destroy(_obj_team_member[index].gameObject);

        _obj_team_member[index] = Instantiate<Character>(GameData.Instance._memberMN._member_obj[index-1], _team_pos[index-1]);
        _obj_team_member[index].transform.localPosition = Vector3.zero;
        _obj_team_member[index].transform.localScale = new Vector3(30, 30, 1);

        //_obj_team_member[index].Animator.SetBool("Run", true);
    }

    public void OnMemberDel()
    {
        for(var i = 1; i < _obj_team_member.Count+1; i++)
            Destroy(_obj_team_member[i].gameObject);

        _obj_team_member.Clear();
    }
}
