using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;

public class MemberMN : MonoBehaviour
{
    //데이터 불러오는 영역
    private JsonData _jsonList;

	public IEnumerator SetMemberMNData()
    {        
        yield return StartCoroutine(LoadMemberIndexData());
    }

    public IEnumerator LoadMemberIndexData()
	{
		TextAsset t = (TextAsset)Resources.Load("member", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDatMemberIndex(t.text));
	}

    public IEnumerator SetDatMemberIndex(string jsonString)
	{
        GameData.Instance._memberDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setMember._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setMember._class = System.Convert.ToInt32(_jsonList[i]["class"].ToString());
            GameData.Instance._setMember._power = System.Convert.ToInt64(_jsonList[i]["power"].ToString());
            GameData.Instance._setMember._name = _jsonList[i]["name"].ToString();
            GameData.Instance._setMember._intro = _jsonList[i]["intro"].ToString();
            GameData.Instance._setMember._price = System.Convert.ToInt64(_jsonList[i]["price"].ToString());

			GameData.Instance._memberDataIndex.Add(GameData.Instance._setMember._index, GameData.Instance._setMember); 

        }
        yield return null;  
    }

    void Awake()
    {
        GameData.Instance._memberMN = this;
    }

    public long _member_power; //원정대 무기 공격력
    public long _member_price; //원정대 무기 강화 가격

    public MemberData _member_data_ori;
    public Dictionary<int, MemberData> _member_data = new Dictionary<int, MemberData>();

    public Transform _member_parent;
    public Member _member_ori;
    public Dictionary<int, Member> _member_list = new Dictionary<int, Member>();

    public void OnLoadMemberData()
    {
        for(var i = 1; i < GameData.Instance._memberDataIndex.Count+1; i++)
        {
            CreateMemberData(i);
        }
    }

    public void CreateMemberData(int index)
    {
        MemberData obj = null;
        obj = Instantiate<MemberData>(_member_data_ori, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(index);

        _member_data.Add(index, obj);
    }
    
    public void OpenMember()
    {
        if(!GameData.Instance._popMN.OnMainPop(2)) return;

        for(var i = 1; i < GameData.Instance._memberDataIndex.Count+1; i++)
        {
            CreateMemberObject(i, _member_data[i]);
        }

        //_member_parent.localPosition = new Vector3(0, 145*(GameData.Instance._playerData._member.Count-1),0);
    }

    public void CreateMemberObject(int index, MemberData data)
    {
        Member obj = null;
        obj = Instantiate<Member>(_member_ori, _member_parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
       
        obj.OnSet(data);
        
        _member_list.Add(index, obj);
    }

    public void CloseMember()
    {
        for(var i = 1; i < _member_list.Count+1; i++)
        {
            if(_member_list.ContainsKey(i)) Destroy(_member_list[i].gameObject);
        }       

        _member_list.Clear();
    }

    public List<Character> _member_obj = new List<Character>();
}
