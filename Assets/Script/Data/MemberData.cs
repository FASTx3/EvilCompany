using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberData : MonoBehaviour
{
    public int _data_index;

    public long _base_atk;
    public long _base_price;

    public long _lev;

    public long _atk;
    public long _price;

    public long _atk_addValue;
    public long _price_addValue;
    

    public void OnSet(int index)
    {
        _data_index = index;

        _base_atk = GameData.Instance._memberDataIndex[_data_index]._power;
        _base_price = GameData.Instance._memberDataIndex[_data_index]._price;

        _atk_addValue = GameData.Instance.StringToLong((_base_atk * 0.2f).ToString("F0"));
        _price_addValue = GameData.Instance.StringToLong((_base_price * 0.5f).ToString("F0"));

        OnValue();

        if(GameData.Instance._playerData._member.ContainsKey(index))
            GameData.Instance._battleMN._battle[0].OnMemberSet(index);
    }

    public void OnValue()
    {
        if(GameData.Instance._playerData._member.ContainsKey(_data_index))
        {
            _lev = GameData.Instance._playerData._member[_data_index];
            _atk = _base_atk + (_atk_addValue * (GameData.Instance._playerData._member[_data_index]-1));
            _price = _base_price + (_price_addValue * GameData.Instance._playerData._member[_data_index]);
        }            
        else
        {
            _lev = 0;
            _atk = _base_atk;
            _price = _base_price;  
        }
            
    }

    public void OnLevelUp()
    {
        GameData.Instance._playerData._member[_data_index] += 1;
        OnValue();

        if(GameData.Instance._memberMN._member_list.ContainsKey(_data_index)) GameData.Instance._memberMN._member_list[_data_index].OnValue();      
    }

    public void OnEmploy()
    {
        GameData.Instance._playerData._member.Add(_data_index, 1);
        OnValue();
        GameData.Instance._battleMN._battle[0].OnMemberSet(_data_index);

        if(GameData.Instance._memberMN._member_list.ContainsKey(_data_index)) GameData.Instance._memberMN._member_list[_data_index].OnBtn(); 
    }
}
