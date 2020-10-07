using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderUpgrade : MonoBehaviour
{
    public Image _icon; 
    public List<Text> _text = new List<Text>();
    public int _index; 
    public long _price; 

    public void OnSet(int index)
    {
        _index = index;

        switch(index)
        {
            case 0 :
                _text[0].text = "체력";
                _text[2].text = GameData.Instance._leaderMN._hp_max.ToString();
                _text[4].text = (GameData.Instance._leaderMN._hp_max + GameData.Instance._leaderMN._hp_addValue).ToString();
            break;

            case 1 :
                _text[0].text = "공격력";
                _text[2].text = GameData.Instance._leaderMN._atk.ToString();
                _text[4].text = (GameData.Instance._leaderMN._atk + GameData.Instance._leaderMN._atk_addValue).ToString();
            break;

            case 2 :
                _text[0].text = "방어력";
                _text[2].text = GameData.Instance._leaderMN._def.ToString();
                _text[4].text = (GameData.Instance._leaderMN._def + GameData.Instance._leaderMN._def_addValue).ToString();
            break;

            case 3 :
                _text[0].text = "치명타율";
                _text[2].text = GameData.Instance._leaderMN._cri.ToString("P");
                _text[4].text = (GameData.Instance._leaderMN._cri + GameData.Instance._leaderMN._cri_addValue).ToString("P");
            break;

            case 4 :
                _text[0].text = "비자금 발견율";
                _text[2].text = GameData.Instance._leaderMN._bonus1.ToString("P");
                _text[4].text = (GameData.Instance._leaderMN._bonus1 + GameData.Instance._leaderMN._bonus1_addValue).ToString("P");
            break;

            case 5 :
                _text[0].text = "실험소재 발견율";
                _text[2].text = GameData.Instance._leaderMN._bonus2.ToString("P");
                _text[4].text = (GameData.Instance._leaderMN._bonus2 + GameData.Instance._leaderMN._bonus2_addValue).ToString("P");
            break;
        }
        
        _text[1].text = string.Format("Lv.{0}", GameData.Instance._playerData._my_leader._lv[index]);
        _text[3].text = GameData.Instance._leaderMN._price[index].ToString();   

        _price = GameData.Instance._leaderMN._price[index];             
    }

    public void OnRefresh()
    {
        switch(_index)
        {
            case 0 :
                _text[2].text = GameData.Instance._leaderMN._hp_max.ToString();
                _text[4].text = (GameData.Instance._leaderMN._hp_max + GameData.Instance._leaderMN._hp_addValue).ToString();
            break;

            case 1 :
                _text[2].text = GameData.Instance._leaderMN._atk.ToString();
                _text[4].text = (GameData.Instance._leaderMN._atk + GameData.Instance._leaderMN._atk_addValue).ToString();
            break;

            case 2 :
                _text[2].text = GameData.Instance._leaderMN._def.ToString();
                _text[4].text = (GameData.Instance._leaderMN._def + GameData.Instance._leaderMN._def_addValue).ToString();
            break;

            case 3 :
                _text[2].text = GameData.Instance._leaderMN._cri.ToString("P");
                _text[4].text = (GameData.Instance._leaderMN._cri + GameData.Instance._leaderMN._cri_addValue).ToString("P");
            break;

            case 4 :
                _text[2].text = GameData.Instance._leaderMN._bonus1.ToString("P");
                _text[4].text = (GameData.Instance._leaderMN._bonus1 + GameData.Instance._leaderMN._bonus1_addValue).ToString("P");
            break;

            case 5 :
                _text[2].text = GameData.Instance._leaderMN._bonus2.ToString("P");
                _text[4].text = (GameData.Instance._leaderMN._bonus2 + GameData.Instance._leaderMN._bonus2_addValue).ToString("P");
            break;
        }
        
        _text[1].text = string.Format("Lv.{0}", GameData.Instance._playerData._my_leader._lv[_index]);
        _text[3].text = GameData.Instance._leaderMN._price[_index].ToString();  

        _price = GameData.Instance._leaderMN._price[_index];
    }

    public void OnLevelUp()
    {
        if(!GameData.Instance._gm.UseGold(_price)) return;

        GameData.Instance._leaderMN.OnLevelUp(_index);
    }
}
