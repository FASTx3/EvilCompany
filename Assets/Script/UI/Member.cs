using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Member : MonoBehaviour
{
    public MemberData _data;
    public Image _icon; 
    public List<Text> _text = new List<Text>();

    public List<GameObject> _obj = new List<GameObject>();

    public int _member_code;

    public void OnSet(MemberData data)
    {
        _data = data;
        _index = data._data_index;

        _text[0].text = GameData.Instance._memberDataIndex[_index]._name;
        _text[4].text = _data._atk_addValue.ToString();
        _text[5].text = _data._price.ToString();
        
        OnBtn();
    }

    public void OnValue()
    {
        _text[1].text = string.Format("Lv. {0}", _data._lev);
        _text[2].text = _data._atk.ToString();
        _text[3].text = _data._price.ToString();
    }

    public int _index;

    public void OnLevelUp()
    {   
        if(!GameData.Instance._gm.UseGold(_data._price)) return;
        
        _data.OnLevelUp();
    }

    public void OnEmploy()
    {
        if(!GameData.Instance._gm.UseGold(_data._price)) return;

        _data.OnEmploy();        
    }

    public void OnBtn()
    {
        if(GameData.Instance._playerData._member.ContainsKey(_index))
        {
            _obj[0].SetActive(true);
            _obj[1].SetActive(false);
        }
        else
        {
            _obj[0].SetActive(false);
            _obj[1].SetActive(true);
        }

        OnValue();
    }
}
