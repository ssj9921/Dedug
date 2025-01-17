using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YJMiniGameManager : MonoBehaviour
{
    // 미니게임 시작 및 끝 부모 객체
    public GameObject endBg;
    public GameObject startBg;

    // 메시지 출력 이미지 부모 객체
    public GameObject messageBg;
    public GameObject messageBg02;

    // 메시지 타임 출력 텍스트 및 이펙트
    public Image loveEffect;

    // 응원봉 타이밍 판정 이미지
    public Image success;
    public Image fail;

    // 게임 대기시간 카운트
    public Text beforeCount;
    public Image beforeImg;

    // 미니게임 탑 바 관련 변수 (점수, 제한시간)
    public Text costText;
    public Text countDown;

    // 마법소녀 캐릭터 및 관객(주인공) 애니메이션 이미지
    public Image badaChar;
    public Image meChar;
    public Image meChar01;
    public Image meChar02;
    public Image meChar03;

    // 게임 버튼
    public Button message01;
    public Button message02;
    public Button bong01;
    public Button bong02;
    public Button bong03;

    // 응원봉 타임 안내 이펙트
    public Image colorEffect01;
    public Image colorEffect02;
    public Image colorEffect03;

    // 점수 및 제한시간, 대기시간 변수
    public static int score;
    int gameTime;
    int beforeGameTime;

    // 응원봉 타임!
    private bool isGameRunning = false;
    private float bongTime = 2.0f;
    private bool isBongTimeActive = false;

    // 하드 응원봉 타임!
    public Image hardStart;
    private float hardBongTimeNext = 3.0f;
    private bool isHardBongTimeActive = false;

    // 하드 응원봉 타임 순서대로 응원봉 출력 및 버튼 대응 저장
    private Queue<Image> activeColorEffects = new Queue<Image>();
    private Queue<Button> expectedBongButtons = new Queue<Button>();

    // 하드 응원봉 타임 동안 클릭 여부를 추적하는 변수
    private bool isHardBongTimeButtonClick = false;

    // 중복 컬러 방지를 위한 리스트 선언
    private List<Image> availableColorEffects = new List<Image>();

    // 게임 일시정지 관련 변수
    public Image stopBg;
    public Button stop;
    public Button keepGoing;
    public Button goTitle;

    public Image realStopBg;
    public Button stopOk;
    public Button stopNo;

    // 게임 일시정지 상태를 나타내는 변수
    private bool isGamePaused = false;

    // 관중 흔들림
    private float shakeRange = 0.2f;
    private float shakeSpeed = 2f;

    public AudioSource gameAudioSource;  // 게임 중 재생될 사운드

    public AudioSource badamessage01_SFX;
    public AudioSource badamessage02_SFX;

    public AudioSource badasucces_SFX;
    public AudioSource badafail_SFX;

    public AudioSource badacount_SFX;

    public AudioSource bongtime01;
    public AudioSource bongtime02;
    public AudioSource bongtime03;

    // 게임 종료 시
    public GameObject ResultCanvas;
    public static bool badaResult;


    private void Start()
    {
        // 게임 시작 시 호출되는 함수
        StartGame();

        gameAudioSource.loop = true;  // 반복 재생

    }

private void StartGame()
    {
        // 게임 대기시간 초기화 및 대기시간 UI 활성화
        beforeGameTime = 3;
        beforeCount.gameObject.SetActive(true);
        beforeImg.gameObject.SetActive(true);
        // 1초마다 CountDownBeforeGame 메소드 호출
        InvokeRepeating("CountDownBeforeGame", 1.0f, 1.0f);
    }

    // 게임 대기시간 관련
    private void CountDownBeforeGame()
    {
        // 게임 대기시간 카운트 다운
        beforeGameTime--;

        if (beforeGameTime == 0)
        {
            // 게임 대기시간 종료 후 숨김
            beforeCount.gameObject.SetActive(false);
            beforeImg.gameObject.SetActive(false);
            // CountDownBeforeGame 호출 중단
            CancelInvoke("CountDownBeforeGame");

            // 실제 게임 시작
            StartRealTimeGame();
        }
        else
        {
            // 대기시간 텍스트 갱신
            badacount_SFX.Play();
            beforeCount.text = beforeGameTime.ToString();

        }
    }

    // 게임 시작
    private void StartRealTimeGame()
    {
        // 실제 게임 시작
        isGameRunning = true;

        // 초기 제한시간 설정
        gameTime = 3;
        countDown.text = gameTime.ToString();

        // 1초마다 UpdateGame 메소드 호출
        InvokeRepeating("UpdateGame", 1.0f, 1.0f);

        gameAudioSource.Play();


    }


    private void UpdateGame()
    {
        // 일시정지 상태에서는 게임 업데이트를 건너뛰기
        if (isGamePaused)
            return;

        // 0. 제한시간이 종료된 경우
        if (gameTime <= 0)
        {
            // 게임 종료 처리
            EndGame();
            return;
        }

        // 1. 실시간 카운트다운 갱신 및 BGM 재생
        countDown.text = gameTime.ToString();
        gameTime--;

        // 2. bongTime 동안 랜덤한 colorEffect 활성화
        if (gameTime >= 75 && gameTime % 5 == 0)
        {
            ActivateRandomColorEffect();
            isBongTimeActive = true;

            // bongTime 초 후에 아무 버튼도 클릭하지 않았을 때 처리를 위해 Invoke 호출
            Invoke("HandleButtonClick", bongTime);
        }

        // (5) countDown < 60일 경우
        if (gameTime > 42 && gameTime < 75 && gameTime % 7 == 0)
        {
            // 랜덤한 colorEffect를 2가지 이미지를 3초 동안 활성화 후 비활성화
            StartCoroutine(ActivateRandomColorEffects());
        }

        if (gameTime <= 42 && gameTime % 10 == 0)
        {
            StartCoroutine(TooHardRandomColorEffects());
        }

        // 3. 게임 버튼 클릭 처리
        if (Input.GetMouseButtonDown(0))
        {
            HandleButtonClick();
        }

        // 4. 스코어가 0 미만으로 내려가지 않도록 확인
        if (score < 0)
        {
            score = 0;
            costText.text = score.ToString();
        }

        // meChar를 Y 축을 기준으로 왕복하도록 만드는 코드
        if (isGameRunning)
        {
            float yOffset = Mathf.PingPong(Time.time * shakeSpeed, shakeRange * 2) - shakeRange;
            Vector3 newPos = meChar.transform.position;
            newPos.y = yOffset;
            meChar.transform.position = newPos;
        }

        // meChar를 Y 축을 기준으로 왕복하도록 만드는 코드
        if (isGameRunning)
        {
            float yOffset = Mathf.PingPong(Time.time * shakeSpeed, shakeRange * 2) - shakeRange;
            Vector3 newPos = meChar01.transform.position;
            newPos.y = yOffset;
            meChar01.transform.position = newPos;
        }

        // meChar를 Y 축을 기준으로 왕복하도록 만드는 코드
        if (isGameRunning)
        {
            float yOffset = Mathf.PingPong(Time.time * shakeSpeed, shakeRange * 2) - shakeRange;
            Vector3 newPos = meChar02.transform.position;
            newPos.y = yOffset;
            meChar02.transform.position = newPos;
        }

        // meChar를 Y 축을 기준으로 왕복하도록 만드는 코드
        if (isGameRunning)
        {
            float yOffset = Mathf.PingPong(Time.time * shakeSpeed, shakeRange * 2) - shakeRange;
            Vector3 newPos = meChar03.transform.position;
            newPos.y = yOffset;
            meChar03.transform.position = newPos;
        }

        // 바다쨩 좌우로 흔들리게 만드는 코드
        if (isGameRunning)
        {
            ShakeObject(badaChar, shakeRange, shakeSpeed);
        }
    }

    // 바다쨩 흔들 함수
    private void ShakeObject(Image obj, float range, float speed)
    {
        float xOffset = Mathf.PingPong(Time.time * speed, range * 2) - range;
        Vector3 newPos = obj.transform.position;
        newPos.x = xOffset;
        obj.transform.position = newPos;
    }

    // 투 하드 봉타임 진행

    private IEnumerator TooHardRandomColorEffects()
    {
        expectedBongButtons.Clear(); // 기존에 저장된 버튼 초기화

        for (int i = 0; i < 3; i++)
        {
            // 랜덤한 colorEffect를 가져와서 큐에 추가
            Image randomColorEffect = GetRandomColorEffect();
            activeColorEffects.Enqueue(randomColorEffect);

            // 1.5초 동안 활성화
            randomColorEffect.gameObject.SetActive(true);
            
            // 효과음 재생
            PlayColorEffectSound(randomColorEffect);

            yield return new WaitForSeconds(1f);

            // 대응하는 bong 버튼을 tooExpectedBongButtons에 저장
            expectedBongButtons.Enqueue(GetMatchingBongButton(randomColorEffect));

            // 비활성화
            randomColorEffect.gameObject.SetActive(false);
        }

        // (5)번에 설명한 대로 3초 동안만 hardBongTimeNext이 진행됨
        isHardBongTimeActive = true;
        isHardBongTimeButtonClick = true;
        hardStart.gameObject.SetActive(true); // hardStart를 활성화
        Invoke("DeactivateHardBongTime", hardBongTimeNext);

        yield return new WaitForSeconds(hardBongTimeNext);

        // hardBongTimeNext가 끝날 때 isHardBongTimeActive = false로 하고 동시에 hardStart를 비활성화
        isHardBongTimeActive = false;
        hardStart.gameObject.SetActive(false);

        // hardBongTime이 종료되면 다시 activeColorEffects를 비움
        activeColorEffects.Clear();
    }

    // 하드 봉타임 진행
    private IEnumerator ActivateRandomColorEffects()
    {
        expectedBongButtons.Clear(); // 기존에 저장된 버튼 초기화

        for (int i = 0; i < 2; i++)
        {
            // 랜덤한 colorEffect를 가져와서 큐에 추가
            Image randomColorEffect = GetRandomColorEffect();
            activeColorEffects.Enqueue(randomColorEffect);

            // 1.5초 동안 활성화
            randomColorEffect.gameObject.SetActive(true);

            // 효과음 재생
            PlayColorEffectSound(randomColorEffect);

            yield return new WaitForSeconds(1.5f);

            // 대응하는 bong 버튼을 expectedBongButtons에 저장
            expectedBongButtons.Enqueue(GetMatchingBongButton(randomColorEffect));

            // 비활성화
            randomColorEffect.gameObject.SetActive(false);
        }

        // (5)번에 설명한 대로 3초 동안만 hardBongTimeNext이 진행됨
        isHardBongTimeActive = true;
        isHardBongTimeButtonClick = true;
        hardStart.gameObject.SetActive(true); // hardStart를 활성화
        Invoke("DeactivateHardBongTime", hardBongTimeNext);

        yield return new WaitForSeconds(hardBongTimeNext);

        // hardBongTimeNext가 끝날 때 isHardBongTimeActive = false로 하고 동시에 hardStart를 비활성화
        isHardBongTimeActive = false;
        hardStart.gameObject.SetActive(false);

        // hardBongTime이 종료되면 다시 activeColorEffects를 비움
        activeColorEffects.Clear();
    }

    // 효과음 재생 메소드
    private void PlayColorEffectSound(Image colorEffect)
    {
        if (colorEffect == colorEffect01)
        {
            bongtime01.Play();
        }

        else if (colorEffect == colorEffect02)
        {
            bongtime02.Play();
        }

        else if (colorEffect == colorEffect03)
        {
            bongtime03.Play();
        }
    }

    // 하드 봉 타임 이후 처리
    private void DeactivateHardBongTime()
    {
        isHardBongTimeActive = false;
        // 하드 봉 타임 동안 버튼이 클릭되지 않았을 경우 score를 -1 감소
        if (isHardBongTimeButtonClick == true)
        {
            score--;
            costText.text = score.ToString();

            // 오답 이미지 활성화
            fail.gameObject.SetActive(true);
            badafail_SFX.Play();
            Invoke("DeactivateFailImage", 2.0f);
        }
        // hardBongTime이 종료되면 다시 activeColorEffects를 비움
        activeColorEffects.Clear();
    }

    // 응원봉과 이펙트 컬러 대응
    private Button GetMatchingBongButton(Image colorEffect)
    {
        if (colorEffect == colorEffect01)
        {
            return bong01;
        }
        else if (colorEffect == colorEffect02)
        {
            return bong02;
        }
        else if (colorEffect == colorEffect03)
        {
            return bong03;
        }

        return null;
    }

    // 일반 봉타임 컬러 이펙트 랜덤 활성화
    private void ActivateRandomColorEffect()
    {

        // 랜덤한 colorEffect 활성화 및 일정 시간 후에 비활성화
        Image randomColorEffect = GetRandomColorEffect();
        randomColorEffect.gameObject.SetActive(true);

        // 효과음 재생
        PlayColorEffectSound(randomColorEffect);

        Invoke("DeactivateColorEffect", bongTime);
    }

    // 일반 봉타임 컬러 이펙트 랜덤 뽑기
    private Image GetRandomColorEffect()
    {
        // 랜덤한 colorEffect 반환
        if (availableColorEffects.Count == 0)
        {
            // 사용 가능한 컬러 이펙트가 없으면 모든 컬러 이펙트를 다시 추가
            availableColorEffects.AddRange(new List<Image> { colorEffect01, colorEffect02, colorEffect03 });
        }

        // 랜덤한 컬러 이펙트 반환 및 사용 목록에서 제거
        int randomIndex = Random.Range(0, availableColorEffects.Count);
        Image randomColorEffect = availableColorEffects[randomIndex];
        availableColorEffects.RemoveAt(randomIndex);

        return randomColorEffect;
    }

    private void DeactivateColorEffect()
    {
        // 모든 colorEffect 비활성화
        colorEffect01.gameObject.SetActive(false);
        colorEffect02.gameObject.SetActive(false);
        colorEffect03.gameObject.SetActive(false);
    }

    // 응원봉 클릭 검사
    public void HandleButtonClick()
    {

        // 하드 응원봉 타임일 때 추가된 부분
        if (isHardBongTimeActive == true)
        {
            // 유저가 버튼을 클릭했을 때 조건 검사
            CheckUserInput();

        }

        // bongTime이 끝났을 때 추가된 부분
        if (isBongTimeActive)
        {
            // bong 버튼이 하나도 클릭되지 않았을 때 처리
            if (expectedBongButtons.Count == 0)
            {
                // 버튼이 클릭되지 않았을 때의 처리
                score--;  // 스코어 감소
                costText.text = score.ToString();  // UI 업데이트
                isBongTimeActive = false;  // bongTime 동안 클릭 여부 추적 변수 초기화
                DeactivateColorEffect();  // colorEffect 비활성화

                // 오답 이미지 활성화
                fail.gameObject.SetActive(true);
                badafail_SFX.Play();
                Invoke("DeactivateFailImage", 2.0f);
            }
        }

        if (isGameRunning)
        {
            // 게임 종료 체크
            CheckGameEnd();
        }

    }

    // 하드 응원봉 타임 버튼 클릭 함수
    private void CheckUserInput()
    {
        if (expectedBongButtons.Count > 0)
        {
            Button expectedButton = expectedBongButtons.Dequeue(); // 큐에서 버튼을 순서대로 가져옴

            if (EventSystem.current.currentSelectedGameObject == expectedButton.gameObject)
            {
                // 이미 실행 중인 Invoke 중지
                CancelInvoke("DeactivateSuccessImage");

                // 이미지 초기화
                success.gameObject.SetActive(false);
                badasucces_SFX.Stop();

                // 버튼이 올바른 순서로 클릭되었을 때
                score++;
                costText.text = score.ToString();

                // 정답 이미지 활성화
                success.gameObject.SetActive(true);
                badasucces_SFX.Play();
                Invoke("DeactivateSuccessImage", 0.5f);
            }
            else
            {
                // 이미 실행 중인 Invoke 중지
                CancelInvoke("DeactivateFailImage");

                // 이미지 초기화
                fail.gameObject.SetActive(false);
                badafail_SFX.Stop();

                // 버튼이 잘못 클릭되었을 때
                score--;
                costText.text = score.ToString();

                // 오답 이미지 활성화
                fail.gameObject.SetActive(true);
                badafail_SFX.Play();
                Invoke("DeactivateFailImage", 0.5f);
            }

            // UI 업데이트
            UpdateUI();

            // 클릭 여부 변수 초기화
            isHardBongTimeButtonClick = false;
        }
    }

    // 일반 봉타임 대응 봉 클릭 검사
    public void OnBongButtonClick(Button bongButton)
    {

        // bongTime이 진행 중일 때만 처리
        if (isGameRunning && isBongTimeActive == true)
        {
            // 활성화된 colorEffect와 대응되는 bong 버튼인지 확인
            if (IsMatchingBongButton(bongButton))
            {
                // 정답인 경우
                score++;
                costText.text = score.ToString();

                // 정답 이미지 활성화
                success.gameObject.SetActive(true);
                badasucces_SFX.Play();
                Invoke("DeactivateSuccessImage", 2.0f);
            }
            else
            {
                // 오답인 경우
                score--;
                costText.text = score.ToString();

                // 오답 이미지 활성화
                fail.gameObject.SetActive(true);
                badafail_SFX.Play();
                Invoke("DeactivateFailImage", 2.0f);
            }

            // UI 업데이트
            UpdateUI();

            // colorEffect 비활성화
            DeactivateColorEffect();

            // bongTime 동안 클릭 여부 추적 변수 초기화
            isBongTimeActive = false;
        }
    }

    private void DeactivateSuccessImage()
    {
        // 정답 이미지 비활성화
        success.gameObject.SetActive(false);
    }

    private void DeactivateFailImage()
    {
        // 오답 이미지 비활성화
        fail.gameObject.SetActive(false);
    }

    private bool IsBongTimeActive()
    {
        // bongTime이 진행 중인지 여부 반환
        return isGameRunning;
    }

    // 일반 봉타임 대응 봉 클릭 대응 검사
    private bool IsMatchingBongButton(Button bongButton)
    {
        // 활성화된 colorEffect와 대응되는 bong 버튼인지 확인
        Image activeColorEffect = GetActiveColorEffect();

        if (bongButton == bong01 && activeColorEffect == colorEffect01)
        {
            return true;
        }
        else if (bongButton == bong02 && activeColorEffect == colorEffect02)
        {
            return true;
        }
        else if (bongButton == bong03 && activeColorEffect == colorEffect03)
        {
            return true;
        }

        return false;
    }

    private Image GetActiveColorEffect()
    {
        // 활성화된 colorEffect 반환
        if (colorEffect01.gameObject.activeSelf)
        {
            return colorEffect01;
        }
        else if (colorEffect02.gameObject.activeSelf)
        {
            return colorEffect02;
        }
        else if (colorEffect03.gameObject.activeSelf)
        {
            return colorEffect03;
        }

        return null;
    }

    private void UpdateUI()
    {
        // UI 업데이트 (제한시간, 점수)
        countDown.text = gameTime.ToString();
        costText.text = "Score: " + score.ToString();
    }

    private void CheckGameEnd()
    {
        // 제한시간이 종료되면 게임 종료
        if (gameTime <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        // 게임 종료 시 사운드 중지
        gameAudioSource.Stop();

        // 게임 종료 시 호출되는 함수
        // badaResult = true;
        ResultCanvas.SetActive(true);

        // badaResult = false;

        isGameRunning = false;
    }

    public void OnMessage01ButtonClick()
    {
        if (isBongTimeActive == false && isGameRunning == true)
        {
            badamessage01_SFX.Play();
            // message01 버튼 클릭 시 호출되는 함수
            StartCoroutine(DisplayMessage01());
        }
    }

    public void OnMessage02ButtonClick02()
    {
        if (isBongTimeActive == false && isGameRunning == true)
        {
            badamessage02_SFX.Play();
            // message02 버튼 클릭 시 호출되는 함수
            StartCoroutine(DisplayMessage02());
        }
    }

    private IEnumerator DisplayMessage01()
    {
        // 메시지 출력 및 일정 시간 후에 숨김
        messageBg.SetActive(true);
        loveEffect.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        messageBg.SetActive(false);
        loveEffect.gameObject.SetActive(false);
    }


    private IEnumerator DisplayMessage02()
    {
        // 메시지 출력 및 일정 시간 후에 숨김
        messageBg02.SetActive(true);
        loveEffect.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        messageBg02.SetActive(false);
        loveEffect.gameObject.SetActive(false);
    }


    // 게임 일시정지 버튼 클릭 시 호출되는 함수
    public void StopButtonClick()
    {
        if (isGameRunning && !isGamePaused)
        {
            // 게임 일시정지
            PauseGame();
        }
        else if (isGameRunning && isGamePaused)
        {
            // 게임 재개
            ResumeGame();
        }
    }

    // 게임 일시정지 처리
    private void PauseGame()
    {
        isGamePaused = true;

        // 게임 일시정지 UI 활성화
        stopBg.gameObject.SetActive(true);
    }

    // 게임으로 돌아가기 버튼 함수
    public void keepGoingClick()
    {
        // 게임 일시정지 UI 비활성화
        stopBg.gameObject.SetActive(false);
        ResumeGame();
    }

    // 굿즈구매로 돌아가기 버튼 함수
    public void goTitleClick()
    {
        // 게임 일시정지 UI 비활성화
        stopBg.gameObject.SetActive(false);

        // 리얼스톱Bg 활성화
        realStopBg.gameObject.SetActive(true);
    }

    // 게임으로 돌아가기 버튼 함수
    public void stopNoClick()
    {
        // 리얼스톱Bg 활성화
        realStopBg.gameObject.SetActive(false);
        ResumeGame();
    }

    // 게임 재개 처리
    private void ResumeGame()
    {
        isGamePaused = false;
    }
}
