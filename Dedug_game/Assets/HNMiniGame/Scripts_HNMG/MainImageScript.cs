using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MainImageScript : MonoBehaviour
{
    [SerializeField] private GameObject card_Back;
    [SerializeField] private GameObject image_show;
    [SerializeField] private GameObject image_frame;
    [SerializeField] private GameObject real_Image;

    [SerializeField] private GameControllerScript gameController;


    public void Start()
    {
        card_Back.SetActive(false);
        Invoke("DisableShowImage", 3);
    }
    public void DisableShowImage()
    {
        image_show.SetActive(false);
        card_Back.SetActive(true);
        image_frame.SetActive(false);
    }
    // 이미지를 클릭했을 때 호출되는 함수
    public void OnClick()
    {
        

        // 이미지가 활성화되어 있고, 게임 컨트롤러가 이미지를 열 수 있는 상태일 때 실행
        if (card_Back.activeSelf && gameController.canOpen)
        {
            //vector3 는 x, y, z로 구성된 3차원 좌표값. (Vector2는 2d)
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = new Vector3(0f, originalScale.y, originalScale.z);


            transform.DOScale(targetScale, 0.1f).OnComplete(() => // 람다식

            {
                card_Back.SetActive(false);
                image_frame.SetActive(true);


                gameController.imageOpened(this);
                transform.DOScale(originalScale, 0.1f);

            }
            
            );
            // 이미지를 비활성화하고, 게임 컨트롤러의 imageOpened 함수 호출
          
        }
    }

    // 이미지의 식별 ID를 저장하는 변수
    private int _spriteId;

    // 이미지의 식별 ID를 가져오는 속성
    public int spriteId
    {
        get { return _spriteId; }
    }

    // 이미지의 스프라이트를 변경하는 함수
    public void ChangeSprite(int id, Sprite image)
    {
        _spriteId = id;
        real_Image.GetComponent<Image>().sprite = image; // 스프라이트를 변경하기 위해 Image 컴포넌트를 가져와 사용
        image_show.GetComponent<Image>().sprite = image;

    }

    // 이미지를 닫는 함수
    public void Close()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(0f, originalScale.y, originalScale.z);
       
        transform.DOShakePosition(0.3f, 50).OnComplete(() =>


            { transform.DOScale(targetScale, 0.2f).OnComplete(() => // 람다식

            {
                card_Back.SetActive(true);
                image_frame.SetActive(false);

                transform.DOScale(originalScale, 0.2f);

            }

            );
        });
        
    }
  
}
