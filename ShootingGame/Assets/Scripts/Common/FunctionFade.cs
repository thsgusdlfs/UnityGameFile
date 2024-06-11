using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FunctionFade : MonoBehaviour
{
    public static FunctionFade Instance;

    Image imgFade;
    [SerializeField, Tooltip("0을 ")] float fadeTime = 1.0f;
    bool fade = false;//true가 되면 페이드 아웃, false가 되면 페이드 인
    UnityAction action = null;//어떤기능이 동작 완료후 실행

    private void Awake()
    {
        //imgFade = GetComponent<Image>();//이러면 못찾음 캔버스 내부의 이미지이기 때문이다.
        //imgFade = transform.GetChild(0).GetComponent<Image>();//자식중 첫번째 자식에게서 컴포넌트를 찾아줌
        imgFade = GetComponentInChildren<Image>();//내 위치로부터 자식중 이미지 컴포넌트가 있는 오브젝트를 찾아 등록해줌

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//루트 게임오브젝트만 동작 가능
            //DonDestroyOnLooad 신을 에디티브로 생성후 거기에 넣어둠
        }
        else 
        {
            Destroy(gameObject);//예약됨
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fade == true && imgFade.color.a < 1)//true가 되면 페이드 아웃 
        {
            Color color = imgFade.color;
            color.a += Time.deltaTime / fadeTime;
            if(color.a > 1.0f) 
            {
                color.a = 1.0f;

                if (action != null)
                { 
                    action.Invoke();
                    action = null;
                }
            }
            imgFade.color = color;
        }
        else if(fade == false && imgFade.color.a > 0)//false가 되면 페이드 인
        {
            Color color = imgFade.color;
            color.a -= Time.deltaTime / fadeTime;
            if (color.a < 0.0f)
            {
                color.a = 0.0f;
            }
            imgFade.color = color;
        }

        imgFade.raycastTarget = imgFade.color.a != 0.0f;
    }

    public void ActiveFade(bool _fade, UnityAction _action = null) 
    {
        fade = _fade;
        action = _action;
    }
}
