using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("To assign")]
    public GameObject rootObject;
    public GameObject introScreen;
    public GameObject menuScreen;
    public GameObject LoadingScreen;

    public GameObject FadeInOutObj;
    public Button SkipButton;
    public Button NextButton;
    public Button StartButton;
    public float fadeSpeed = 0.01f;

    [Header("Debug Part")]
    public Image FadeInOutImage;
    public string levelLoading;
    public float TransitioningTarget;
    public TextEventPrinter tep;
    public bool isTransitioning;

    // Use this for initialization
    void Start()
    {
        if (!menuScreen || !introScreen)
            Debug.LogError("No Menu and Intro defined.");
        if (FadeInOutObj)
            FadeInOutImage = FadeInOutObj.GetComponentInChildren<Image>();

        tep = GetComponentInChildren<TextEventPrinter>();
        introScreen.SetActive(false);
        menuScreen.SetActive(true);
    }

    public void FromMenuToIntro()
    {
        isTransitioning = true;
        TransitioningTarget = 1f;
        //tep = introScreen.GetComponentInChildren<TextEventPrinter>();
        if (tep)
        {
            tep.StartPrintingText();
            tep.allTextBlocksDoneEvent += Tep_AllTextBlocksDoneEvent;
            tep.currentTextBlockDoneEvent += Tep_CurrentTextBlockDoneEvent;
            StartButton.interactable = false;
            NextButton.interactable = false;
        }
    }

    void Tep_AllTextBlocksDoneEvent()
    {
        SkipButton.interactable = false;
        StartButton.interactable = true;
        NextButton.interactable = false;
    }

    void Tep_CurrentTextBlockDoneEvent()
    {
        NextButton.interactable = true;
    }


    public void SkipText()
    {
        NextButton.interactable = true;
        tep.StopAllCoroutines();
        tep.textObj.text = "You decided to skip the intro.";
    }

    public void NextText()
    {
        NextButton.interactable = false;
        tep.StartPrintingText();
    }

    public void goToNextScene(string newScene)
    {
        goToNextScene(newScene, true);
    }

    public void goToNextScene(string newScene, bool loadingScreen = true)
    {
        if (loadingScreen)
        {
            rootObject.SetActive(false);
            LoadingScreen.SetActive(true);
            foreach (RectTransform o in LoadingScreen.GetComponentsInChildren<RectTransform>())
            {
                o.gameObject.SetActive(true);
            }
            Camera.main.gameObject.SetActive(true);
        }
        levelLoading = newScene;
        StartCoroutine(LoadNewScene());
    }
    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation o = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelLoading, UnityEngine.SceneManagement.LoadSceneMode.Single);
        o.completed += Handle_Completed;
        while (!o.isDone)
        {
            yield return null;
        }
    }


    void Handle_Completed(AsyncOperation obj)
    {
        obj.allowSceneActivation = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (isTransitioning)
        {

            FadeInOutImage.color = new Color(FadeInOutImage.color.r, FadeInOutImage.color.g, FadeInOutImage.color.b, Mathf.Lerp(FadeInOutImage.color.a, TransitioningTarget, fadeSpeed));
            if (FadeInOutImage.color.a >= TransitioningTarget - 0.001f && TransitioningTarget >= 1f)
            {
                menuScreen.SetActive(false);
                introScreen.SetActive(true);
                TransitioningTarget = 0f;
                //isTransitioning = false;

            }
            if (FadeInOutImage.color.a <= TransitioningTarget + 0.001f && TransitioningTarget <= 0f)
            {
                isTransitioning = false;
            }

        }
    }
}
