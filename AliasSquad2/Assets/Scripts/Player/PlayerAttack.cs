using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;

public class PlayerAttack : NetworkBehaviour {

    GameObject nade;
    Transform pos;
    [SerializeField] private Transform fpsPlayerObj;

    [SerializeField] private GameObject mainWeaponArm;
    [SerializeField] private GameObject[] mainWeaponObjs = new GameObject[3];
    [SerializeField] private WeaponAsset[] mainWeaponAssets = new WeaponAsset[3];

    [SerializeField] private GameObject subWeaponArm;
    [SerializeField] private GameObject[] subWeaponObjs = new GameObject[3];
    [SerializeField] private WeaponAsset[] subWeaponAssets = new WeaponAsset[3];

    [SerializeField] private GameObject meleeWeapon;

    [SerializeField] private GameObject grenade;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject flash;
    [SerializeField] private GameObject molotov;
    [SerializeField] private GameObject c4;
    public GameObject bulletObj;

    [SerializeField] private Transform throwPos;
    [SerializeField] private Transform throwDownPos;

	[SerializeField] private Transform recoilObj;

    private Transform camera;

	private NetworkVariable<bool> fpsObjFlag = new NetworkVariable<bool>();

    // スロット
    // 0 = Main     4 = Smoke
    // 1 = Sub      5 = Flash
    // 2 = Knife    6 = Molotov
    // 3 = Grenade  7 = C4
    private string[] slot = new string[8];

    private NetworkVariable<int> beforeSelectSlot = new NetworkVariable<int>();
    private NetworkVariable<int> currentSelectSlot = new NetworkVariable<int>();

    private NetworkVariable<FixedString32Bytes> selectItemName = new NetworkVariable<FixedString32Bytes>();

    private string[] presetStr = new string[3];
    private Preset[] preset = new Preset[3];

	private Vector3 tmpRotate;

    private int currentMainMagazineAmmo;
    private int currentMainAllAmmo;

    private int currentSubMagazineAmmo;
    private int currentSubAllAmmo;

    private int weight;

    private Rigidbody rigidbody;

    private bool attackDelayMain = false;

    private bool attackDelaySub = false;

    private NetworkVariable<int> nadeThrowRaedy = new NetworkVariable<int>();

    private float slotSwitchTime;

    private GameObject ammoStatus;

    private Text ammoText;

    private GameObject scopePanel;

	private float recoilHealTime = 0;

	private float recoilTime = 0;

	private float time;

    [SerializeField]
    private AudioSource playSE;

	private Vector3 fpsPlayerObjPos;

    private float count;

    private int reloading = -1;

    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptyAmmoSound;

    [SerializeField] private Text reloadHintText;

    public override void OnNetworkSpawn()
    {
        fpsObjFlag.Value = false;
        rigidbody = GetComponent<Rigidbody>();
        beforeSelectSlot.Value = 2;
        currentSelectSlot.Value = 0;
        camera = transform.Find("RecoilObj").Find("PlayerCamera");
        if (IsLocalPlayer)
        {

            slot[0] = "AK47";
            slot[1] = "0";
            slot[2] = "Knife";
            slot[3] = "0";
            slot[4] = "0";
            slot[5] = "0";
            slot[6] = "0";
            for (int i = 0; i < preset.Length; i++)
            {
                //presetStr[i] = AccountManager.accountManager.GetPreset(i);
                //preset[i] = JsonUtility.FromJson<Preset>(presetStr[i]);
            }
            fpsPlayerObjPos = fpsPlayerObj.localPosition;
            FPSObjActive();

            AmmoRefill();

            ammoStatus = GameObject.Find("HUD_Alias").transform.Find("AmmoStatus").gameObject;
            ammoText = ammoStatus.transform.Find("Status").Find("Ammo").gameObject.GetComponent<Text>();
        }
    }

    void Update() {
        if (IsOwner) {

            switch (currentSelectSlot.Value)
            {
                case 0:
                    ammoText.text = currentMainMagazineAmmo.ToString("D2") + "/" + currentMainAllAmmo.ToString("D3");
                    break;

                case 1:
                    ammoText.text = currentMainMagazineAmmo.ToString("D2") + "/" + currentMainAllAmmo.ToString("D3");
                    break;

            }

            //攻撃入力
            if (currentSelectSlot.Value == 0)
            {
                
                if (Input.GetMouseButton(0) && reloading == -1)
                {

                    Attack();

                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && reloading == -1)
                {

                    Attack();

                }
            }


            if (rigidbody.velocity.x > 2.5f ||rigidbody.velocity.x < -2.5f || rigidbody.velocity.z > 2.5f || rigidbody.velocity.z < - 2.5f) {
				fpsPlayerObj.localPosition = Vector3.Slerp (fpsPlayerObj.localPosition, new Vector3(fpsPlayerObjPos.x,fpsPlayerObjPos.y,fpsPlayerObjPos.z - 0.1f),0.075f);
			}else{
				fpsPlayerObj.localPosition = Vector3.Slerp (fpsPlayerObj.localPosition, fpsPlayerObjPos,0.075f);
			}

			if(Input.GetKeyDown(KeyCode.F1)){
				slot[0] = "AK47";
			}
			if(Input.GetKeyDown(KeyCode.F2)){
				slot[0] = "M4A4";
			}
            /*
			if(Input.GetKeyDown(KeyCode.F3)){
				slot[0] = "AWP";
			}
            */
            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            recoilObj.rotation = Quaternion.Slerp(recoilObj.transform.rotation, rotation, 0.07f);
             
            if (!(recoilHealTime > 0)&& recoilTime > 0) {
				recoilTime -= recoilTime/0.5f * Time.deltaTime;
                if (recoilTime < 0)
                {
                    recoilTime = 0;
                }
			} else {
				recoilHealTime -= Time.deltaTime;
				time = 0;
			}
            
            if (Input.GetKeyDown(KeyCode.R) && reloading == -1)
            {
                int maxMagazine = 0;

                switch (currentSelectSlot.Value)
                {
                    case 0:
                        switch (slot[currentSelectSlot.Value])
                        {
                            case "AK47":
                                maxMagazine = mainWeaponAssets[0].magazine;
                                break;

                            case "M4A4":
                                maxMagazine = mainWeaponAssets[1].magazine;
                                break;

                            case "AWP":
                                maxMagazine = mainWeaponAssets[2].magazine;
                                break;
                        }

                        if (currentMainAllAmmo != 0 && currentMainMagazineAmmo < maxMagazine)
                        {
                            reloading = 0;
                            PlaySEServerRpc("Reload");
                        }
                        break;

                    case 1:
                        switch (slot[currentSelectSlot.Value])
                        {
                            case "Glock":
                                maxMagazine = subWeaponAssets[0].magazine;
                                break;

                            case "USP-S":
                                maxMagazine = subWeaponAssets[1].magazine;
                                break;

                            case "DE":
                                maxMagazine = subWeaponAssets[2].magazine;
                                break;
                        }

                        if (currentSubAllAmmo != 0 && currentMainMagazineAmmo < maxMagazine)
                        {
                            reloading = 1;
                            PlaySEServerRpc("Reload");
                        }
                        break;

                    default:
                        break;
                }
            }

            if (currentSelectSlot.Value == reloading)
            {
                count += Time.deltaTime;

                if (count >= 2)
                {
                    switch (reloading)
                    {
                        case 0:
                            int index = 0;
                            switch (slot[0])
                            {
                                case "AK47":
                                    index = 0;
                                    break;

                                case "M4A4":
                                    index = 1;
                                    break;

                                case "AWP":
                                    index = 2;
                                    break;
                            }

                            int ammoMargin = mainWeaponAssets[index].magazine - currentMainMagazineAmmo;
                            if ((currentMainAllAmmo -= ammoMargin) >= 0)
                            {
                                currentMainMagazineAmmo += ammoMargin;
                            }
                            else
                            {
                                ammoMargin += currentMainAllAmmo;
                                currentMainMagazineAmmo += ammoMargin;
                                currentMainAllAmmo = 0;
                            }
                            

                            break;

                        case 1:

                            break;

                        default:
                            break;
                    }
                    count = 0;
                    reloading = -1;
                }
            }
            else if(reloading != -1)
            {
                count = 0;
                reloading = -1;
                PlaySEServerRpc("Stop");
            }

            if (currentSelectSlot.Value >= 3 && currentSelectSlot.Value <= 6) {

                if (Input.GetMouseButtonDown(0) && nadeThrowRaedy.Value == 0)
                {
                    SetNadeThrowRaedyServerRpc(1);
                }

                if (Input.GetMouseButtonUp(0) && nadeThrowRaedy.Value == 1)
                {
                    NadeThrowServerRpc();
                }

                if (Input.GetMouseButtonDown(1) && nadeThrowRaedy.Value == 0)
                {
                    SetNadeThrowRaedyServerRpc(2);
                }

                if (Input.GetMouseButtonUp(1) && nadeThrowRaedy.Value == 2)
                {
                    NadeThrowServerRpc();
                }

            }
            else
            {
                SetNadeThrowRaedyServerRpc(0);
            }


            //武器選択
            //ホイール
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    int tempNum = currentSelectSlot.Value + i + 1;
                    if (tempNum > 7)
                    {
                        tempNum -= 8;
                    }
                    if (slot[tempNum] != "" && slot[tempNum] != "0")
                    {
                        
                        SetCurrentSelectSlotServerRpc(tempNum);
                        
                        break;

                    }
                }

            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    int tempNum = currentSelectSlot.Value - i - 1;
                    if (tempNum < 0)
                    {
                        tempNum += 8;
                    }
                    if (slot[tempNum] != "" && slot[tempNum] != "0")
                    {
                        
                        SetCurrentSelectSlotServerRpc(tempNum);
                        
                        break;

                    }
                }
            }
            //数値キー
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
				if (slot[0] != "" && currentSelectSlot.Value != 0)
                {
                    
                    SetCurrentSelectSlotServerRpc(0);
                    
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
				if (slot[1] != "" && currentSelectSlot.Value != 1)
                {
                    
                    SetCurrentSelectSlotServerRpc(1);
                    
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
				if (slot[2] != "" && currentSelectSlot.Value != 2)
                {
                    
                    SetCurrentSelectSlotServerRpc(2);
                    
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {

                if (currentSelectSlot.Value < 3 || currentSelectSlot.Value > 6)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (slot[3 + i] != "0")
                        {
                            SetCurrentSelectSlotServerRpc(3 + i);
                            
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int tempNum = currentSelectSlot.Value + i + 1;
                        if (tempNum > 6)
                        {
                            tempNum -= 4;
                        }
                        if (slot[tempNum] != "0")
                        {
                            SetCurrentSelectSlotServerRpc(tempNum);
                            
                            break;
                        }
                    }
                }                
            }
			if (Input.GetKeyDown(KeyCode.Alpha5))
            {
				if (slot[7] != ""  && currentSelectSlot.Value != 7)
                {
                    
                    SetCurrentSelectSlotServerRpc(7);
                    
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SetCurrentSelectSlotServerRpc(beforeSelectSlot.Value);
                
            }

            if (Input.GetMouseButtonDown(1))
            {
                switch (currentSelectSlot.Value)
                {
                    case 1:
                        if (slot[0] == "AWP")
                        {
                            scopePanel.SetActive(scopePanel.activeSelf);
                        }
                        break;

                    case 7:
                        //slot[7] = "";
                        //C4TakeOrDrop();
                        //GameObject c4Instance = Instantiate(c4,throwDownPos.position,throwDownPos.rotation);                        
                        ////NetworkServer.Spawn(c4Instance);
                        break;
						
                    default:

                        break;
                }
                
            }

            if (fpsObjFlag.Value){
				FPSObjActive();
			}

            ///isLocalPlayer ここまで
        }
    }

    public void C4TakeOrDrop(GameObject gameObject = null)
    {
        Demolition.demolition.SetPlayerHaveC4(gameObject);
        if (gameObject)
        {
            slot[7] = "C4";
        }
    }
	[ServerRpc]
	private void SetFpsObjFlagServerRpc(){
		fpsObjFlag.Value = false;
	}

    [ServerRpc]
    private void SetNadeThrowRaedyServerRpc(int num)
    {
        nadeThrowRaedy.Value = num;
    }

    [ServerRpc]
    private void SetCurrentSelectSlotServerRpc(int num)
    {
		beforeSelectSlot.Value = currentSelectSlot.Value;
        currentSelectSlot.Value = num; 
        fpsObjFlag.Value = true;
    }

    private void FPSObjActive()
    {
        int index = 0;
		switch (beforeSelectSlot.Value)
		{
		case 0:
			index = 0;
			switch (slot[0])
			{
			case "AK47":
				index = 0;
				break;

			case "M4A4":
				index = 1;
				break;

			case "AWP":
				index = 2;
				break;
			}
			mainWeaponArm.SetActive(false);
			mainWeaponObjs[index].SetActive(false);
            break;

		case 1:
			index = 0;
			switch (slot[1])
			{
			case "Glock":
				index = 0;
				break;

			case "USP-S":
				index = 1;
				break;

			case "DE":
				index = 2;
				break;
			}
			subWeaponArm.SetActive(false);
			subWeaponObjs[index].SetActive(false);
			break;

		case 2:
			meleeWeapon.SetActive(false);
			break;

		case 3:

			break;

		case 4:

			break;

		case 5:

			break;
		}

        switch (currentSelectSlot.Value)
        {
            case 0:
                index = 0;
                switch (slot[0])
                {
                    case "AK47":
                        index = 0;
                        break;

                    case "M4A4":
                        index = 1;
                        break;

                    case "AWP":
                        index = 2;
                        break;
                }
                mainWeaponArm.SetActive(true);
                mainWeaponObjs[index].SetActive(true);

                break;

            case 1:
                index = 0;
                switch (slot[1])
                {
                    case "Glock":
                        index = 0;
                        break;

                    case "USP-S":
                        index = 1;
                        break;

                    case "DE":
                        index = 2;
                        break;
                }
                subWeaponArm.SetActive(true);
                subWeaponObjs[index].SetActive(true);
                break;

            case 2:
                meleeWeapon.SetActive(true);
                break;

            case 3:

                break;

            case 4:

                break;

            case 5:

                break;
        }



    }

    public void Attack() {
        int index = 0;
        switch (currentSelectSlot.Value) {
			case 0:
				recoilTime += Time.deltaTime;
								
                if (!attackDelayMain)
                {
                    index = 0;
                    string name = "";
                    switch (slot[0])
                    {
                        case "AK47":
                            index = 0;
                            name = "AK47";
                            break;

                        case "M4A4":
                            index = 1;
                            name = "M4A4";
                            break;

                        case "AWP":
                            index = 2;
                            name = "AWP";
                            break;
                    }
                    if (currentMainMagazineAmmo != 0) {
                        FireServerRpc(mainWeaponAssets[index].damage);
                        currentMainMagazineAmmo -= 1;
                        recoilObj.localEulerAngles = new Vector3(-mainWeaponAssets[index].y.Evaluate(recoilTime), mainWeaponAssets[index].x.Evaluate(recoilTime), 0);
                        PlaySEServerRpc(name);
                    }
                    else
                    {
                        PlaySEServerRpc("EmptyAmmo");
                    }


                    recoilHealTime = mainWeaponAssets[index].delayTime;
                    attackDelayMain = true;
                    Invoke("DelayMainClear", mainWeaponAssets[index].delayTime);
                }
                break;

            case 1:
				recoilTime += Time.deltaTime;
				recoilHealTime = 1;
                if (!attackDelaySub)
                {
                    index = 0;
                    string name = "";
                    switch (slot[1])
                    {
                        case "Glock":
                            index = 0;
                            name = "Glock";
                            break;

                        case "USP-S":
                            index = 1;
                            name = "USP-S";
                            break;

                        case "DE":
                            index = 2;
                            name = "DE";
                            break;
                    }

                    if (currentMainMagazineAmmo != 0)
                    {
                        FireServerRpc(subWeaponAssets[0].damage);
                        currentSubMagazineAmmo -= 1;
                        recoilObj.localEulerAngles = new Vector3(-subWeaponAssets[index].y.Evaluate(recoilTime), subWeaponAssets[index].x.Evaluate(recoilTime), 0);
                        PlaySEServerRpc(name);
                    }
                    else
                    {
                        PlaySEServerRpc("EmptyAmmo");
                    }

                    recoilHealTime = subWeaponAssets[index].delayTime;
                    attackDelaySub = true;
                    Invoke("DelaySubClear", subWeaponAssets[0].delayTime);
                }
                break;

            case 2:

                break;

            case 3:

                break;

            case 4:

                break;

            case 5:

                break;

            case 6:

                break;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void FireServerRpc(int damage)
    {
		var bullet = Instantiate(bulletObj, camera.position, camera.rotation);

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 100, ForceMode.VelocityChange);
        bullet.GetComponent<BulletController>().SetDefaultDamage(damage);
        bullet.GetComponent<BulletController>().SetHitObj = gameObject;
        bullet.GetComponent<BulletController>().ownerClientId.Value = OwnerClientId;

        bullet.GetComponent<NetworkObject>().Spawn();
    }


    [ServerRpc]
    private void PlaySEServerRpc(string str)
    {
        if (str != "")
        {
            PlaySEClientRpc(str);
        }
    }

    [ClientRpc]
    private void PlaySEClientRpc(string str)
    {
        switch (str)
        {
			case "AK47":
				playSE.PlayOneShot (mainWeaponAssets [0].soundEffect);
				mainWeaponObjs [0].transform.Find("MuzzleFlashEffect").gameObject.GetComponent<ParticleSystem>().Play();
                break;

            case "M4A4":
                playSE.PlayOneShot(mainWeaponAssets[1].soundEffect);
				mainWeaponObjs [1].transform.Find("MuzzleFlashEffect").gameObject.GetComponent<ParticleSystem>().Play();
                break;

            case "AWP":
                playSE.PlayOneShot(mainWeaponAssets[2].soundEffect);
				mainWeaponObjs [2].transform.Find("MuzzleFlashEffect").gameObject.GetComponent<ParticleSystem>().Play();
                break;

            case "Glock":
                playSE.PlayOneShot(subWeaponAssets[0].soundEffect);
                subWeaponObjs[0].transform.Find("MuzzleFlashEffect").gameObject.GetComponent<ParticleSystem>().Play();
                break;

            case "USP-S":
                playSE.PlayOneShot(subWeaponAssets[1].soundEffect);
                subWeaponObjs[1].transform.Find("MuzzleFlashEffect").gameObject.GetComponent<ParticleSystem>().Play();
                break;

            case "DE":
                playSE.PlayOneShot(subWeaponAssets[2].soundEffect);
                subWeaponObjs[2].transform.Find("MuzzleFlashEffect").gameObject.GetComponent<ParticleSystem>().Play();
                break;

            case "Reload":
                playSE.PlayOneShot(reloadSound);
                break;

            case "EmptyAmmo":
                playSE.PlayOneShot(emptyAmmoSound);
                break;

            case "Stop":
                playSE.Stop();
                break;

            default:
                break;
        }
    }


    private void DelayMainClear()
    {
        attackDelayMain = false;
    }

    private void DelaySubClear()
    {
        attackDelaySub = false;
    }

    [ServerRpc]
    private void NadeThrowServerRpc()
    {
        int power = 0;
        switch (nadeThrowRaedy.Value)
        {
            case 1:
                Debug.Log(nadeThrowRaedy);
                pos = throwPos;
                power = 10;
                break;

            case 2:
                pos = throwDownPos;
                power = 5;
                break;
        }

        switch (currentSelectSlot.Value)
        {
            case 3:
                nade = Instantiate(grenade, pos.position, pos.rotation);
                break;

            case 4:
                nade = Instantiate(smoke, pos.position, pos.rotation);
                break;

            case 5:
                nade = Instantiate(flash, pos.position, pos.rotation);
                break;

            case 6:
                nade = Instantiate(molotov, pos.position, pos.rotation);
                break;
        }
        nade.GetComponent<Rigidbody>().velocity = nade.transform.forward * power;
        //NetworkServer.Spawn(nade);

        nadeThrowRaedy.Value = 0;

    }

    private void Plant()
    {

    }

    public void AmmoRefill()
    {
        int index = 0;
        index = 0;
        switch (slot[0])
        {
            case "AK47":
                index = 0;
                break;

            case "M4A4":
                index = 1;
                break;

            case "AWP":
                index = 2;
                break;
        }
        currentMainAllAmmo = mainWeaponAssets[index].ammo;
        currentMainMagazineAmmo = mainWeaponAssets[index].magazine;

        index = 0;
        switch (slot[1])
        {
            case "Glock":
                index = 0;
                break;

            case "USP-S":
                index = 1;
                break;

            case "DE":
                index = 2;
                break;
        }
        currentSubAllAmmo = subWeaponAssets[index].ammo;
        currentSubMagazineAmmo = subWeaponAssets[index].magazine;
    }
}

[System.Serializable]
public class Preset
{
    public string Main;
    public string Sub;
    public string Melee;
    public int Grenade;
    public int Smoke;
    public int Flash;
    public int Molotov;
}
