using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoScript01 : MonoBehaviour
{
    public Rigidbody rd;

    public Text timeCostText;
    
    public GameObject finalTimeCostText;

    public GameObject winText;

    public GameObject replayButton;

    public GameObject startButton;
    
    public GameObject resetButton;
    
    public GameObject countDownTextObj;

    public AudioClip rewardAudio;
    
    public AudioClip successAudio;

    private AudioSource audioSource;
    
    private Text countDownText;
    
    private int remainingFood = 8;

    private DateTime gameStartTime;

    private const string timeCostTextLabel = "Time Cost: ";

    private bool startFlag;

    private bool countDownOver;
    
    private int countDownTime = 3;
    
    private DateTime lastCountDownTime;
    
    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 60;
        countDownText = countDownTextObj.GetComponent<Text>();
        rd = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        winText.SetActive(false);
        replayButton.SetActive(false);
        finalTimeCostText.SetActive(false);
        countDownTextObj.SetActive(false);
        replayButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        startButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(startButton);
            gameStartTime = DateTime.Now;
            lastCountDownTime = DateTime.Now;
            countDownTextObj.SetActive(true);
            countDownText.text = countDownTime.ToString();
            startFlag = true;
        });
        resetButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    // Update is called once per frame
    private void Update()
    {
        if (countDownOver)
        {
            float speed = 10f;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            rd.AddForce(new Vector3(speed * h, 0, speed * v));
            TimeSpan timeSpan = DateTime.Now - gameStartTime;
            timeCostText.text = timeCostTextLabel + timeSpan.TotalSeconds.ToString("0.000");

            var rdVelocity = rd.velocity;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rdVelocity.y = 8;
            }
            if (rdVelocity.x > speed)
            {
                rdVelocity.x = speed;
            }
            if (rdVelocity.x < -speed)
            {
                rdVelocity.x = -speed;
            }
            if (rdVelocity.z > speed)
            {
                rdVelocity.z = speed;
            }
            if (rdVelocity.z < -speed)
            {
                rdVelocity.z = -speed;
            }
            rd.velocity = rdVelocity;
        }
        
        if (startFlag && countDownTime >= 0)
        {
            TimeSpan countDownSpan = DateTime.Now - lastCountDownTime;
            if (countDownSpan.TotalMilliseconds >= 500)
            {
                countDownTime--;
                if (countDownTime == 0)
                {
                    countDownText.text = "GO!";
                }
                else if (countDownTime < 0)
                {
                    countDownTextObj.SetActive(false);
                    countDownOver = true;
                }
                else
                {
                    countDownText.text = countDownTime.ToString();
                }
                lastCountDownTime = DateTime.Now;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag($"Food"))
        {
            audioSource.PlayOneShot(rewardAudio);
            Destroy(other.gameObject);
            remainingFood--;
            if (remainingFood == 0)
            {
                winText.SetActive(true);
                replayButton.SetActive(true);
                finalTimeCostText.SetActive(true);
                TimeSpan timeSpan = DateTime.Now - gameStartTime;
                finalTimeCostText.GetComponent<Text>().text = "Final Time Cost: " + timeSpan.TotalSeconds.ToString("0.000");
                audioSource.PlayOneShot(successAudio);
            }
        }
    }
}
