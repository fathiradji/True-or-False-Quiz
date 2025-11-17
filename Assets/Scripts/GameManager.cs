using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string question;
        public bool answer; // True for True, False for False
    }

    public TextMeshProUGUI questionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;
    public Button trueButton;
    public Button falseButton;
    public Button restartButton;
    public Button backButton;

    public AudioClip correctSound;
    public AudioClip wrongSound;
    private AudioSource audioSource;

    public List<Question> questions = new List<Question>();
    private List<Question> remainingQuestions;
    private Question currentQuestion;

    private float timePerQuestion = 10f; // waktu per soal dalam detik
    private float timer;
    public float correctAnswerDelay = 1f;
    private bool gameActive = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        remainingQuestions = new List<Question>(questions);
        statusText.text = "";
        restartButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        NextQuestion();

        trueButton.onClick.AddListener(() => CheckAnswer(true, trueButton));
        falseButton.onClick.AddListener(() => CheckAnswer(false, falseButton));
        restartButton.onClick.AddListener(RestartGame);
        backButton.onClick.AddListener(BackGame);
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;
        timerText.text = $"{timer:F0}";

        if (timer <= 0f)
        {
            GameOver("Time's Up!", falseButton);
        }
    }

    void NextQuestion()
    {
        if (remainingQuestions.Count == 0)
        {
            GameFinished();
            return;
        }

        int randomIndex = Random.Range(0, remainingQuestions.Count);
        currentQuestion = remainingQuestions[randomIndex];
        questionText.text = currentQuestion.question;
        remainingQuestions.RemoveAt(randomIndex);

        statusText.text = "";
        timer = timePerQuestion;
    }

    void CheckAnswer(bool playerAnswer, Button selectedButton)
    {
        if (!gameActive) return;
        if (playerAnswer == currentQuestion.answer)
        {
            HighlightButton(selectedButton, Color.green);
            PlaySound(correctSound);
            StartCoroutine(ProceedAfterDelay());
        }
        else
        {
            HighlightButton(selectedButton, Color.red);
            PlaySound(wrongSound);
            GameOver("Wrong Answer!", selectedButton);
        }
    }

    IEnumerator ProceedAfterDelay()
    {
        trueButton.interactable = false;
        falseButton.interactable = false;

        yield return new WaitForSeconds(correctAnswerDelay);

        ResetButtonColors();

        trueButton.interactable = true;
        falseButton.interactable = true;

        NextQuestion();
    }

    void GameOver(string message, Button wrongButton)
    {
        gameActive = false;
        statusText.text = "Game Over!";
        trueButton.interactable = false;
        falseButton.interactable = false;
        restartButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        HighlightButton(wrongButton, Color.red);
    }

    void GameFinished()
    {
        gameActive = false;
        statusText.text = "You answered all questions!";
        trueButton.interactable = false;
        falseButton.interactable = false;
        restartButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void BackGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void HighlightButton(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        button.targetGraphic.color = color;
    }

    void ResetButtonColors()
    {
        trueButton.targetGraphic.color = Color.grey;
        falseButton.targetGraphic.color = Color.grey;
    }

    void SetButtonDefaultColor(Button button)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        button.colors = cb;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
