using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GM : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._gm = this;
    }

    public float _timeRate;
    void Update()
    {
        _timeRate = Time.deltaTime;
    }

    public bool _save_reset;
    void Start()
    {
        GameData.Instance._popMN.OnPopReset();
        StartCoroutine(StartLoadData());//데이터 로드
    }

    IEnumerator StartLoadData()
    {
        yield return StartCoroutine(GameData.Instance._leaderMN.SetLeaderMNData());//리더 데이터 불러오기
        yield return StartCoroutine(GameData.Instance._memberMN.SetMemberMNData());//조직원 데이터 불러오기
        yield return StartCoroutine(GameData.Instance._enemyMN.SetEnemyMNData());//적 데이터 불러오기
        yield return StartCoroutine(GameData.Instance._questMN.SetQuestMNData());//퀘스트 데이터 불러오기
        

        if(_save_reset) PlayerPrefs.DeleteAll();

        yield return StartCoroutine(GameData.Instance.LoadData());

        if(GameData.Instance._playerData._save_data)
        {               
                   
        }
        else FirstData();//게임 최초 시작시 생성되는 데이터

        GetGold(0);//화폐 정보 로드

        GameData.Instance._playerData._quest.Add(1, 0);
        
        GameData.Instance._leaderMN.OnLoadLeader();//리더정보 불러오기
        GameData.Instance._memberMN.OnLoadMemberData();//조직원 데이터 불러오기 
        GameData.Instance._battleMN.OnLoadBattle();//원정 데이터 불러오기   
        GameData.Instance._questMN.OnLoadQuest();//퀘스트 데이터 불러오기
        GameData.Instance._questMN.OpenQuest();

        GameData.Instance._battleMN.OnBattleStart();
    }

    public void FirstData()//처음 시작시 데이터
    {        
        GameData.Instance._leaderMN.OnResetLeader();
        GameData.Instance._battleMN.OnResetStage();

        GameData.Instance._playerData._save_data = true;
    }

    public List<Text> _text = new List<Text>();

    public bool UseGold(long gold)
    {
        if(GameData.Instance._playerData._gold < gold) 
        {
            GameData.Instance._gm.OnToast("알 림", "골드가 부족합니다.");
            return false;
        }

        GameData.Instance._playerData._gold -= gold;
        _text[0].text = GameData.Instance._playerData._gold.ToString("N0");
        return true;
    }

    public void GetGold(long gold)
    {
        GameData.Instance._playerData._gold += gold;
        _text[0].text = GameData.Instance._playerData._gold.ToString("N0");
    }

    public List<Text> _popup_text = new List<Text>();

    public void OnPopup()
    {

    }

    public void ClosePopup()
    {

    }

    public Transform _toast_obj;
    public List<Text> _toast_text = new List<Text>();

    public void OnToast(string title, string text)
    {
        _toast_obj.localScale = new Vector3(1, 0, 1);
        DOTween.Kill("toast");

        _toast_text[0].text = title;
        _toast_text[1].text = text;

        _toast_obj.DOScaleY(1, 0.1f).SetEase(Ease.Linear).SetId("toast").OnComplete(() =>
        {
            _toast_obj.DOScaleY(0, 0.1f).SetEase(Ease.Linear).SetId("toast").SetDelay(1.5f);
        });
    }
}
