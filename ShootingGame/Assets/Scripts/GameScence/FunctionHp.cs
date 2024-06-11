using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 HP의 변동이 생기면 HP게이지를 즉시 변경하고 Effect가 해당 게이지로 초당 변동하게 만들어 줍니다.
/// </summary>
public class FunctionHp : MonoBehaviour
{
    [SerializeField] Image imgHp;
    [SerializeField] Image imgEffect;

    [SerializeField, Range(0.1f, 10f)] float effectTime = 1;

    Transform trsPlayer;
    GameManager gameManager;

    bool isEnded = false;

    private void Awake()
    {
        initHp();
    }
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void initHp()
    {
        imgHp.fillAmount = 1;
        imgEffect.fillAmount = 1;
    }

    void Update()
    {
        checkHpAmount();
        chasePlayer();
        checkPlayerDestroy();
    }

    private void chasePlayer()
    {
        Vector3 pos = new Vector3();
        if (gameManager.GetPlyerPosition(out pos) == true)
        {
            transform.position = pos - new Vector3(-0.5f, 0.8f, 0);
        }

        //if (trsPlayer == null)
        //{
        //    trsPlayer = GameManager.Instance._Player.transform;
        //    if (trsPlayer == null)
        //    {
        //        return;
        //    }
        //}
    }

    private void checkHpAmount()
    {
        if (imgHp.fillAmount == imgEffect.fillAmount)
        {
            return;
        }

        if (imgHp.fillAmount < imgEffect.fillAmount)
        {
            imgEffect.fillAmount -= (Time.deltaTime / effectTime);
            if (imgHp.fillAmount > imgEffect.fillAmount)
            {
                imgEffect.fillAmount = imgHp.fillAmount;
            }
        }
        else if (imgHp.fillAmount > imgEffect.fillAmount)
        {
            imgEffect.fillAmount = imgHp.fillAmount;
        }
    }

    public void SetHp(float _maxHp, float _curHp)
    {
        imgHp.fillAmount = _curHp / _maxHp;
    }

    private void checkPlayerDestroy()
    {
        //if (isEnded == false && gameManager._Player == null)
        //{ 
        //    isEnded = true;
        //    Destroy(gameObject, 0.5f);
        //}

        if (imgEffect.fillAmount == 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
