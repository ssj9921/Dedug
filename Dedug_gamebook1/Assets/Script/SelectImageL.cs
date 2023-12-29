using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Unity.VisualScripting;

public class SelectImageL : MonoBehaviour
{

    //기본 투명 이미지
    public Sprite chrictorNomal;

    // 표정 리스트
    public Sprite nomal11;
    public Sprite fun12;
    public Sprite sad13;
    public Sprite surprise14;
    public Sprite shy15;
    public Sprite angry16;

    public Sprite nomal21;
    public Sprite fun22;
    public Sprite sad23;
    public Sprite surprise24;
    public Sprite shy25;
    public Sprite angry26;

    public Sprite nomal31;
    public Sprite fun32;
    public Sprite sad33;
    public Sprite surprise34;
    public Sprite shy35;
    public Sprite angry36;

    public Sprite nomal41;
    public Sprite fun42;
    public Sprite sad43;
    public Sprite surprise44;
    public Sprite shy45;
    public Sprite angry46;

    public Sprite nomal51;

    public Sprite nomal61;

    // 기본 투명 이미지
    public Image tagetCharictor;

    // 애니메이션
    public Text text;
    public Vector3 targetScale = new Vector3(2, 2, 2);
    public Ease ease = Ease.OutQuad;
    public Color targetColor = Color.red;
    public float targetFadeValue = 0;

    void Start()
    {
        tagetCharictor = GetComponent<Image>();
    }

    public void SelectImageChangeL()
    {

        int clickNum = MainController.clickNum;
        List<Dictionary<string, object>> data_Dialog = CSVReader.Read("DedugScript");

        if (data_Dialog[clickNum]["selectText1"].ToString() == "따라간다.")
        {
            tagetCharictor.sprite = fun32;
        }

        if (data_Dialog[clickNum]["selectText2"].ToString() == "믿고 따라가도 되는지 물어본다.")
        {
            tagetCharictor.sprite = surprise34;
        }

        if (data_Dialog[clickNum]["selectText3"].ToString() == "못 믿겠다고 말한다.")
        {
            tagetCharictor.sprite = sad33;
        }
    }
}