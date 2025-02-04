using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

public class RefereeScript : NetworkBehaviour
{
    public List<EnemyScript> enemyList;

    public EnemyGenerator enemyGeneratorAccess;

    private bool isGameOver;
    private bool areAllEnemiesDead;
    [SyncVar]
    public bool canTransferTurnToPlayer;

    [SyncVar]
    public bool isServersTurn;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    [HideInInspector]
    public int chosenEnemyId;
    [SyncVar]
    public int waveCount = 0;

    [SyncVar]
    int targetPlayerId = 1;

    public delegate void PreNewWaveAction();
    public static event PreNewWaveAction preNewWaveEvent;

    public delegate void NewWaveAction();
    public static event NewWaveAction newWaveEvent;

    public delegate void TurnStartAction();
    public static event TurnStartAction turnStartEvent;

    private GameObject[] card;
    private GameObject[] button;
    private GameObject[] mainCamera;
    private GameObject[] playerHealth;
    private GameObject[] playerScripts;

    public List<PlayerScript> playerList;

    public static RefereeScript instance;


    [SyncVar]
    int[] randomNumbers = new int[4];
    [SyncVar]
    public int randomEnemyCount = 0;
    public readonly SyncList<int> displayCardIdList = new SyncList<int>();


    private void Awake()
    {
        instance = this;        
    }

    private void Start()
    {
        if (playerScripts == null)
        {
            playerScripts = GameObject.FindGameObjectsWithTag("PlayerScriptTag");
            SetPlayerList(playerScripts);
        }
        
        areAllEnemiesDead = false;
        canTransferTurnToPlayer = true;
        TurnScript.restartGameEvent += RefereeReset;       
               
        isGameOver = false;
        
        if (card == null)
        {
            card = GameObject.FindGameObjectsWithTag("Cards");
            DeactivateCards(card);
        }
        if (button == null)
        {
            button = GameObject.FindGameObjectsWithTag("EndTurnButton");
            DeactivateButton(button);
        }
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectsWithTag("MainCamera");
            DeactivateCamera(mainCamera);
        }
        if (playerHealth == null)
        {
            playerHealth = GameObject.FindGameObjectsWithTag("Health");
            DeactivateHealth(playerHealth);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChooseNewEnemy(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChooseNewEnemy(-1);
        }

    }

    private void SetPlayerList(GameObject[] Scripts)
    {
        foreach (GameObject Script in Scripts)
        {
            playerList.Add(Script.GetComponent<PlayerScript>());
        }
        RandomNumbersSetUpRoot();
        enemyGeneratorAccess.GenerateEnemies(1);
    }
    public void RandomNumbersSetUpRoot()
    {
        DatabasePlayer databasePlayerAccess = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
        if (isServer)
        {
            if (databasePlayerAccess != null)
            {
                RandomNumberGeneration(databasePlayerAccess.cardList.Count);
            }
        }
    }
    public void RandomNumberGeneration(int maximumValue)
    {
        displayCardIdList.Clear();
        if (isServer)
        {
            randomEnemyCount = Random.Range(2, 4);
            enemyGeneratorAccess.RandomNumber(randomEnemyCount);
            for (int i = 0; i < 4; i++)
            {
                displayCardIdList.Add(Random.Range(0, maximumValue));
            }
        }
    }

    public int GetRandomNumber(int inputIndex)
    {
        return displayCardIdList[inputIndex];
    }
    private void DeactivateHealth(GameObject[] health)
    {
        if (isServer)
        {
            health[1].SetActive(false);
        }
        else
        {
            health[0].SetActive(false);
        }
    }

    private void DeactivateCamera(GameObject[] camera)
    {
        if (isServer)
        {
            camera[1].SetActive(false);
        }
        else
        {
            camera[0].SetActive(false);
        }
    }

    private void DeactivateButton(GameObject[] button)
    {
        if (isServer)
        {
            button[1].SetActive(false);
        }
        else
        {
            button[0].SetActive(false);
        }
    }

    private void DeactivateCards(GameObject[] cards)
    {
        if (isServer)
        {
            for (int i = 7; i < cards.Length; i++)
            {
                card[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i < 7; i++)
            {
                card[i].SetActive(false);
            }
        } 
    }
    public void CallStartTurnEvent()
    {
        if (turnStartEvent != null)
        {
            turnStartEvent();
        }
        TurnScript.instance.ShouldStartPlayerTurn(true);
    }
    public void CallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        for (int i = 0; i < 2; i++)
        {
            playerList[i].EndTurnPlayerScript();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdCallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        CallEndTurnForBothPlayers();
    }
    public void ResetChosenEnemy()
    {
        foreach(EnemyScript enemy in enemyList)
        {
            enemy.ChangeSelectedStatus(false);
        }
        chosenEnemyId = 0;
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void ChooseNewEnemy(int inputDirection)
    {
        enemyList[chosenEnemyId].ChangeSelectedStatus(false);
        if (chosenEnemyId + inputDirection < enemyList.Count && chosenEnemyId + inputDirection >= 0)
        {
            chosenEnemyId += inputDirection;
        }
        else if(chosenEnemyId + inputDirection >= enemyList.Count)
        {
            chosenEnemyId = 0;
        }
        else if (chosenEnemyId + inputDirection < 0)
        {
            chosenEnemyId = enemyList.Count - 1;
        }
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    public void NewWaveCheck()
    {
        areAllEnemiesDead = true;
        foreach (EnemyScript enemy in enemyList)
        {
            if (enemy.enemyHealth > 0)
            {
                areAllEnemiesDead = false;
            }
        }
        if (areAllEnemiesDead)
        {

            canTransferTurnToPlayer = false;
            if (playerList[0].isThisPlayersTurn)
            {
                playerList[0].BeginPreNewWaveCall();
            }
            else
            {
                playerList[1].BeginPreNewWaveCall();
            }
        }
    }
    public void CallPreNewWaveEvent()
    {
        if (isServer)
        {
            if (waveCount < 3)
            {
                if (preNewWaveEvent != null)
                {
                    preNewWaveEvent();
                    waveCount++;
                }
            }
            else
            {
                EndGame(true);
            }
        }
    }
    private void EndGame(bool didPlayerWin)
    {
        TurnScript.instance.SetPlayerTurnBool(false);
        isGameOver = true;
        if(didPlayerWin)
        {
            winImage.SetActive(true);
        }
        else
        {
            lostImage.SetActive(true);
        }
        restartGameButton.SetActive(true);
    }
    public void CallNewWaveEvent()
    {
        if (newWaveEvent != null)
        {
            newWaveEvent();
        }
    }
    public void StartNextWaveInitalize()
    {
        if(isClientOnly)
        {
            CmdStartNextWave();
        }
        else
        {
            RpcStartNextWave(true);
        }
    }
    [ClientRpc]
    public void RpcStartNextWave(bool shouldStartEvents)
    {
        StartNextWaveLogic(shouldStartEvents);
    }
    [Command(requiresAuthority = false)]
    private void CmdStartNextWave()
    {
        RpcStartNextWave(true);
    }
    private void StartNextWaveLogic(bool shouldStartEvents)
    {
        areAllEnemiesDead = false;
        foreach (EnemyScript enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
        if(waveCount == 3)
        {
            enemyGeneratorAccess.GenerateEnemies(1);
        }
        else
        {
            enemyGeneratorAccess.GenerateEnemies(randomEnemyCount);
        }
        if (shouldStartEvents)
        {
            CallNewWaveEvent();   
        }
    }
    
    public void RefereeReset()
    {
        isGameOver = false;
        StartNextWaveInitalize();
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
    public void StartForeachEnemyCoroutine()
    {
        if(!isGameOver)
        {
            StartCoroutine(ForeachEnemyTurnCoroutine());
        }
    }
    private IEnumerator ForeachEnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        if (!areAllEnemiesDead)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                int enemyDamage = enemy.GenerateAttack();
                if (isClientOnly)
                {
                    CmdDealDamageToPlayer(enemyDamage);
                }
                else
                {
                    RpcDealDamageToPlayer(enemyDamage);
                }
                UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
                yield return new WaitForSeconds(0.75f);
            }
        }
        if (canTransferTurnToPlayer)
        {
            CallStartTurnEvent();
        }
    }

    [ClientRpc]
    public void RpcDealDamageToPlayer(int inputDamage)
    {
        DealDamageLogic(inputDamage);
    }
    [Command(requiresAuthority = false)]
    public void CmdDealDamageToPlayer(int inputDamage)
    {
        DealDamageLogic(inputDamage);
    }

    public void DealDamageLogic(int inputDamage)
    {
        if (playerList[targetPlayerId].transform.root.GetComponentInChildren<PlayerStatScript>().TakeDamageAndCheckIfDead(inputDamage))
        {
            TurnScript.instance.ShouldStartPlayerTurn(false);
            EndGame(false);
        }
        if (targetPlayerId == 1)
        {
            targetPlayerId = 0;
        }
        else
        {
            targetPlayerId++;
        }
    }
    
}
