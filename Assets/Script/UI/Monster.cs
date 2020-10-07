using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.HeroEditor.Common.CharacterScripts;
public class Monster : MonoBehaviour
{
    public Slider _hp;
    public Transform _parent;
    public Character _enemy;

    public void OnSet(int index)
    {
        if(_enemy != null) Destroy(_enemy.gameObject);
        Character _obj = Instantiate<Character>(GameData.Instance._enemyMN._enemy_prafab[index-1], _parent);
        _obj.transform.localPosition = Vector3.zero;

        if(GameData.Instance._enemyDataIndex[index]._class == 0) _obj.transform.localScale = new Vector3(-50, 50, 1);
        else if(GameData.Instance._enemyDataIndex[index]._class == 1) _obj.transform.localScale = new Vector3(50, 50, 1);
        else if(GameData.Instance._enemyDataIndex[index]._class == 2) _obj.transform.localScale = new Vector3(-70, 70, 1);

        _enemy = _obj;

        transform.localPosition = new Vector3(500, 0, 0);                 
        gameObject.SetActive(true);  

        _enemy.Animator.SetBool("Run", true);
    }
    /*
    void Update()
    {
        if(_enemy != null)
        {
            
        }
    }
    */
}
