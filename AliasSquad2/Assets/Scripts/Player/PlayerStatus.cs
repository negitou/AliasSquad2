using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerStatus : NetworkBehaviour
{
	public const int maxHealth = 100;

    [SerializeField] private Image flashPanel;
    [SerializeField] private float flashTime = 0;
	[SerializeField] private float flashCoeff;
	[SerializeField] private AnimationCurve flashCurve;

	[SerializeField] private Image smokePanel;
	private NetworkVariable<float> smokeTime = new NetworkVariable<float>();

    [SerializeField] private float molotovTime;
    private float molotovTimeElapsed;

	public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

	public Text healthNum;
	//public Image healthBar;

    private NetworkVariable<int> killCount = new NetworkVariable<int>() ;
    private NetworkVariable<int> assistCount = new NetworkVariable<int>();
    private NetworkVariable<int> deathCount = new NetworkVariable<int>();

	private NetworkVariable<bool> death = new NetworkVariable<bool>();

	public bool GetDeath { get { return death.Value; } }

	[SerializeField] private GameObject atRag;
	[SerializeField] private GameObject ftRag;

    InGameManager gameManager;

    private void Reset()
    {
        flashPanel = transform.Find("CanvasFP").Find("FlashPanel").GetComponent<Image>();
        smokePanel = transform.Find("CanvasSP").Find("SmokePanel").GetComponent<Image>();
    }

    public void Restart()
    {
        if (IsServer)
        {
            death.Value = false;
            currentHealth.Value = maxHealth;
        }
    }

    public override void OnNetworkSpawn()
	{
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        if (IsOwner)
        {
            currentHealth.OnValueChanged += OnChangeHealth;
            Transform healthObj = GameObject.Find("HUD_Alias").transform.Find("HP");
            healthNum = healthObj.Find("HpDisplay").gameObject.GetComponent<Text>();
            //healthBar = healthObj.Find("HPBar").gameObject.GetComponent<Image>();
        }
	}

    void Update ()
	{
        if (flashPanel.color.a > 0)
        {
            flashTime -= Time.deltaTime;
            if (flashTime <= 0)
            {
                flashPanel.color = new Color(flashPanel.color.r, flashPanel.color.g, flashPanel.color.b, flashPanel.color.a - 0.2f * Time.deltaTime);
            }
        }

        if (smokePanel.color.a > 0)
        {
            smokeTime.Value -= Time.deltaTime;
            if (smokeTime.Value <= 0)
            {
                smokePanel.color = new Color(smokePanel.color.r, smokePanel.color.g, smokePanel.color.b, smokePanel.color.a - 1.0f * Time.deltaTime);
            }
        }
			
		if(IsOwner && death.Value){
            if (IsServer)
            {
                Restart();
            }
            else
            {
                IkikaeruServerRpc();
            }
            transform.position = GameObject.Find("SpawnPoints").transform.GetChild(Random.Range(0, 6)).transform.position;

            GetComponent<PlayerAttack>().AmmoRefill();
        }
    }

	[ServerRpc]
	private void IkikaeruServerRpc(){
		death.Value = false;
		currentHealth.Value = maxHealth;
	}

	/*
	private void OnTriggerStay (Collider other)
	{
		
	}
	*/

	public void TakeDamage (ulong clientId, int amount)
	{
        if (!IsServer)
        {
            return;
        }

		currentHealth.Value -= amount;
        //Debug.Log(currentHealth.Value.ToString()+"/amount:"+ amount.ToString());
		if (currentHealth.Value < 0) {
            currentHealth.Value = 0;
		}
        if (currentHealth.Value == 0 && !death.Value)
        {
            deathCount.Value++;
            death.Value = true;
            if(!gameManager) gameManager = FindObjectOfType<InGameManager>();
            gameManager.AddKillServerRpc(clientId);
            gameManager.AddDeathServerRpc(OwnerClientId)    ;
            if (gameObject.GetComponent<PlayerManager> ().team == "AT") {
				GameObject tmp = Instantiate (atRag,transform.position,transform.rotation);
				tmp.GetComponent<NetworkObject>().Spawn();
			} else {
				GameObject tmp = Instantiate (ftRag, transform.position, transform.rotation);
                tmp.GetComponent<NetworkObject>().Spawn();
            }
        }
	}		

	private void OnChangeHealth (int previous, int current)
	{
		//数値
		healthNum.text = current.ToString ();
		//ヘルスバー
		//healthBar.fillAmount = ((float)health/(float)maxHealth); 
	}

    [ClientRpc]
    public void TakeSmokeClientRpc(float Dis)
    {
        smokeTime.Value = 0.2f;
        smokePanel.color = new Color(smokePanel.color.r, smokePanel.color.g, smokePanel.color.b, Dis);
    }

    [ClientRpc]
    public void TakeFlashClientRpc(float sa)
    {
        flashCoeff = sa / 180f;
        if (flashPanel.color.a < flashCurve.Evaluate(flashCoeff))
        {
            flashPanel.color = new Color(flashPanel.color.r, flashPanel.color.g, flashPanel.color.b, 1);
        }

        flashTime = 2 * flashCurve.Evaluate(flashCoeff);
    }

    private void OnTriggerStay(Collider col)
    {
        molotovTimeElapsed += Time.deltaTime;

        if (col.gameObject.tag == "fire")
        {
            if (molotovTimeElapsed >= molotovTime)
            {
                //TakeDamage(5);

                molotovTimeElapsed = 0.0f;
            }
        }
    }

}
