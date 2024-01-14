using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Demolition : NetworkBehaviour {

    public static Demolition demolition;

    public int roundMaxTime = 180;

    [SerializeField] private Text roundTimeText;
    private NetworkVariable<float> roundTime = new NetworkVariable<float>();

    [SerializeField] private Text roundMessageText;

    private bool gameStart = false;

    public int numRoundsToWin = 6;
    public int maxRound = 10;
    public float warmUpDelay = 10.0f;
    public float roundStartDelay = 5.0f;
    public float roundEndDelay = 5.0f;

    [SerializeField] private GameObject roundWinTextAT;
    [SerializeField] private GameObject roundWinTextFT;

    private WaitForSeconds warmUpWait;
    private WaitForSeconds roundStartWait;
    private WaitForSeconds roundEndWait;

    private NetworkVariable<int> currentWinNumAT = new NetworkVariable<int>();
    private NetworkVariable<int> currentWinNumFT = new NetworkVariable<int>();

    private string gameWinTeam = "";

    private NetworkVariable<GameObject> playerHaveC4 = new NetworkVariable<GameObject>();

    public GameObject GetPlayerHaveC4 { get { return playerHaveC4.Value; } }

    private int roundNum;

    [SerializeField] private GameObject[] spawnAT;
    [SerializeField] private GameObject[] spawnFT;

    private void Awake()
    {
        demolition = this;
    }
	/*
    [ServerCallback]
    void Start () {
        warmUpWait = new WaitForSeconds(warmUpDelay);
        roundStartWait = new WaitForSeconds(roundStartDelay);
        roundEndWait = new WaitForSeconds(roundEndDelay);

        StartCoroutine(WarmUp());
        StartCoroutine(GameLoop());
    }
	*/
	/*
    void Update()
    {
        if (gameStart)
        {
            if (roundTime > 0)
            {
                roundTime -= Time.deltaTime;
            }
            else if(roundTime < 0)
            {
                roundTime = 0;
            }
            string str = Mathf.FloorToInt(roundTime / 60f).ToString("D2") + ":" + Mathf.FloorToInt(roundTime % 60f).ToString("D2");

            roundTimeText.text = str;

        }

    }
	*/
    private IEnumerator WarmUp()
    {
        yield return warmUpWait;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());

        roundTime.Value = roundMaxTime;

        yield return StartCoroutine(RoundPlaying());

        yield return StartCoroutine(RoundEnding());

        if (gameWinTeam != "")
        {
            
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        //ResetPlayer();

        //RpcRoundStarting();

        yield return roundStartWait;
    }
	/*
    [ClientRpc]
    void RpcRoundStarting()
    {
        
        DisablePlayerControl();

        roundNum++;
        roundMessageText.text = "ROUND " + roundNum;
    }
	
    private void ResetPlayer()
    {
        for (int i = 0; i < TeamManager.maxTeamMember; i++)
        {
            if (TeamManager.teamManager.gameObjectAT[i])
            {
                TeamManager.teamManager.gameObjectAT[i].GetComponent<PlayerStatus>().Restart();
            }

            if (TeamManager.teamManager.gameObjectFT[i])
            {
                TeamManager.teamManager.gameObjectFT[i].GetComponent<PlayerStatus>().Restart();
            }
        }
    }
	*/
    private void DisablePlayerControl()
    {
        
    }

    private IEnumerator RoundPlaying()
    {


        while (C4bomb.c4bomb.c4State.Value >= 3 || PlayerCheck()) {
            if (C4bomb.c4bomb.c4State.Value == 3)
            {
                currentWinNumAT.Value++;
            }
            else if (C4bomb.c4bomb.c4State.Value == 4)
            {
                currentWinNumAT.Value++;
            }

            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        yield return roundEndWait;
    }

    public void SetPlayerHaveC4(GameObject gameObject)
    {
        playerHaveC4.Value = gameObject;
    }

    private bool PlayerCheck()
    {
        int atCount = 0;
        int ftCount = 0;
        for (int i = 0; i < TeamManager.maxTeamMember; i++)
        {
            if (TeamManager.teamManager.gameObjectAT[i])
            {
                if (!TeamManager.teamManager.gameObjectAT[i].GetComponent<PlayerStatus>().GetDeath)
                {
                    atCount++;
                }
            }

            if (TeamManager.teamManager.gameObjectFT[i])
            {
                if (!TeamManager.teamManager.gameObjectFT[i].GetComponent<PlayerStatus>().GetDeath)
                {
                    ftCount++;
                }
            }
        }
        if (atCount == 0)
        {
            currentWinNumAT.Value++;
            return true;
        }
        if (ftCount == 0)
        {
            currentWinNumFT.Value++;
            return true;
        }
        return false;
    }


    private void TeamAllocation()
    {

    }
}
