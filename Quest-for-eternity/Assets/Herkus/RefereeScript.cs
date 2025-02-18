using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;


public class RefereeScript : NetworkBehaviour
{
    public List<EnemyScript> enemyList;
    public List<EnemyScript> killedEnemyList;

    public EnemyGenerator enemyGeneratorAccess;

    private bool isGameOver;
    private bool areAllEnemiesDead;
    [SyncVar]
    public bool canTransferTurnToPlayer;

    [SyncVar]
    public bool isServersTurn;

    [SyncVar]
    public bool shouldSwitchTargetPlayer;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    [HideInInspector]
    public int chosenEnemyId;
    [SyncVar]
    public int waveCount = 0;

    [SyncVar]
    public int targetPlayerId = 1;

    public delegate void PreNewWaveAction();
    public event PreNewWaveAction preNewWaveEvent;

    public delegate void NewWaveAction();
    public event NewWaveAction newWaveEvent;

    public delegate void TurnStartAction();
    public event TurnStartAction turnStartEvent;

    private GameObject[] card;
    private GameObject[] button;
    private GameObject[] mainCamera;
    private GameObject[] playerHealth;
    private GameObject[] playerScripts;

    public List<PlayerScript> playerList;

    public static RefereeScript instance;

    public bool singlePlayerMode;

    [SyncVar]
    int[] randomNumbers = new int[4];
    [SyncVar]
    public int randomEnemyCount = 0;
    public readonly SyncList<int> displayCardIdList = new SyncList<int>();
    public List<int> lootIdList;
    public Coroutine myCoroutine = null;

    public int lootCardCount;

    [SerializeField]
    DatabaseMultiplayer databaseMultiplayerAccess;
    
    private void Awake()
    {
        instance = this;        
    }

    private void Start()
    {
        preNewWaveEvent += SwitchPlayerAttackIdNest;
        if (playerScripts == null)
        {
            playerScripts = GameObject.FindGameObjectsWithTag("PlayerScriptTag");
            SetPlayerList(playerScripts);
        }
        if(playerList.Count == 2)
        {
            singlePlayerMode = false;
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
        else
        {
            singlePlayerMode = true;
            playerList[0].EndTurnSubscription();
            targetPlayerId = 0;
        }
        
        areAllEnemiesDead = false;
        canTransferTurnToPlayer = true;                  
        isGameOver = false;    
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
        enemyGeneratorAccess.GenerateEnemies(1, false);
    }
    public void RandomNumbersSetUpRoot()
    {
        databaseMultiplayerAccess.updatedLootList.AddRange(databaseMultiplayerAccess.genericLootList);
        DatabasePlayer databasePlayerAccess = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
        if (isServer)
        {
            if (databasePlayerAccess != null)
            {
                RandomNumberGeneration(databaseMultiplayerAccess.updatedLootList.Count);
            }
        }
    }
    public void RandomNumberGeneration(int maximumValue)
    {
        displayCardIdList.Clear();
        if (isServer)
        {
            randomEnemyCount = Random.Range(2, 3);
            enemyGeneratorAccess.RandomNumber(randomEnemyCount);
            for (int i = 0; i < 8; i++)
            {
                int randomValue = Random.Range(0, maximumValue);
                while(!IsLootIdValid(randomValue))
                {
                    randomValue = Random.Range(0, maximumValue);
                }
                displayCardIdList.Add(databaseMultiplayerAccess.updatedLootList[randomValue]);
                lootIdList.Add(randomValue);
            }
            databaseMultiplayerAccess.updatedLootList.Clear();
            lootIdList.Clear();
        }
    }
    private bool IsLootIdValid(int inputId)
    {
        for(int i = 0; i < lootIdList.Count; i++)
        {
            if(inputId == lootIdList[i])
            {
                return false;
            }
        }
        return true;
    }
    public int GetRandomNumber(int inputIndex)
    {
        return displayCardIdList[inputIndex];
    }
    private void DeactivateHealth(GameObject[] health)
    {
        Vector3 vec = new Vector3(-3.56f, 16.53f, 1.050181f);
        if (isServer)
        {
            health[1].transform.localPosition = vec;
        }
        else
        {
            health[0].transform.localPosition = vec;
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
        if(!singlePlayerMode)
        {
            if (isServer)
            {
                Debug.Log("this shouldn't get executed in the beginning");
                if (!playerList[1].isPlayerAlive)
                {
                    Debug.Log("I am DEAD!! I AM A COrPSE! 1");
                    Invoke("RpcCallEndTurnEventForPlayer", 0.1f);
                }
            }
            else
            {
                Debug.Log("this shouldn't get executed in the beginning 2");
                if (!playerList[0].isPlayerAlive)
                {
                    Debug.Log("I am DEAD!! I AM A COrPSE! 2");
                    Invoke("CmdCallEndTurnEventForPlayer", 0.1f);
                }
            }
        }
        TurnScript.instance.ShouldStartPlayerTurn(true);
    }

    [ClientRpc]
    private void RpcCallEndTurnEventForPlayer()
    {
        if (isClientOnly)
        {
            playerList[1].turnScriptAccess.CallEndTurnEvent();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdCallEndTurnEventForPlayer()
    {
        playerList[0].turnScriptAccess.CallEndTurnEvent();
    }
    public void CallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        if(singlePlayerMode)
        {
            playerList[0].EndTurnPlayerScript();
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                playerList[i].EndTurnPlayerScript();
            }
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
        if(!areAllEnemiesDead)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                enemy.ChangeSelectedStatus(false);
            }
            chosenEnemyId = 0;
            enemyList[chosenEnemyId].ChangeSelectedStatus(true);
        }
        
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
        Debug.Log("checking new wave");
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
            if(!singlePlayerMode)
            {
                canTransferTurnToPlayer = false;
            }
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
        if(myCoroutine != null)
        {
            Debug.Log("Coroutine still running!");
            StopCoroutine(myCoroutine);
            if (canTransferTurnToPlayer)
            {
                CallStartTurnEvent();
            }
            myCoroutine = null;
        }
        foreach (EnemyScript enemy in killedEnemyList)
        {
            if(enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        killedEnemyList.Clear();
        enemyList.Clear();
        if(waveCount == 3)
        {
            enemyGeneratorAccess.GenerateEnemies(1, false);
        }
        else
        {
            enemyGeneratorAccess.GenerateEnemies(randomEnemyCount, false);
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
        if (!isGameOver)
        {
            if(myCoroutine == null)
            {
                myCoroutine = StartCoroutine(ForeachEnemyTurnCoroutine());
            }   
        }
    }
    private IEnumerator ForeachEnemyTurnCoroutine()
    {
        // COROUTINE GETS CALLED ONCE.
        Debug.Log("Coroutine gets called");
        yield return new WaitForSeconds(1.5f);
        if (!areAllEnemiesDead)
        {
            int loopCount = enemyList.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if(enemyList[i].canAttack)
                {   
                    int enemyDamage = enemyList[i].GenerateAttack();
                    int enemyType = enemyList[i].myEnemyType;
                    if (isClientOnly)
                    {
                        Debug.Log("Client damage call");
                        CmdDealDamageToPlayer(enemyDamage, enemyType);
                    }
                    else
                    {
                        Debug.Log("server damage call");
                        RpcDealDamageToPlayer(enemyDamage, enemyType);
                    }
                    UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
                }
                else
                {
                    enemyList[i].canAttack = true;
                }
                yield return new WaitForSeconds(0.75f);
            }
            ResetDamageMultiplier();
            if (shouldSwitchTargetPlayer)
            {
                SwitchPlayerAttackIdNest();
            }
            shouldSwitchTargetPlayer = true; //might nest
        }
        if (canTransferTurnToPlayer)
        {
            CallStartTurnEvent();
        }
        myCoroutine = null;
    }

    [ClientRpc]
    public void RpcDealDamageToPlayer(int inputDamage, int inputType)
    {
        if(isClientOnly ^ singlePlayerMode)
        {
            DealDamageLogic(inputDamage, inputType);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdDealDamageToPlayer(int inputDamage, int inputType)
    {
        DealDamageLogic(inputDamage, inputType);
    }

    public void DealDamageLogic(int inputDamage, int inputType)
    {
        //Debug.Log("Deal Damage Logic");
        if (playerList[targetPlayerId].transform.root.GetComponentInChildren<PlayerStatScript>().TakeDamageAndCheckIfDead(inputDamage, inputType))
        {
            TurnScript.instance.ShouldStartPlayerTurn(false);
            playerList[targetPlayerId].isPlayerAlive = false;
            if (AreAllPlayersDead())
            {
                EndGame(false);
            }
        }
        else
        {
            playerList[targetPlayerId].isPlayerAlive = true;
        }
    }
    
    public void SwitchPlayerAttackIdNest()
    {     
        if (isServer)
        {
            SwitchPlayerAttackId();
        }
        else
        {
            CmdSwitchPlayerAttackId();
        }
    }

    private void SwitchPlayerAttackId()
    {
        if(!singlePlayerMode)
        {           
            if (targetPlayerId == 1)
            {
                targetPlayerId = 0;
            }
            else
            {
                targetPlayerId++;
            }
        }
        //playerList[targetPlayerId].transform.root.GetComponentInChildren<PlayerStatScript>().ResetPlayerStatList();
    }

    [Command(requiresAuthority = false)]
    private void CmdSwitchPlayerAttackId()
    {
        if (!singlePlayerMode)
        {
            if (targetPlayerId == 1)
            {
                targetPlayerId = 0;
            }
            else
            {
                targetPlayerId++;
            }
        }
        //playerList[targetPlayerId].transform.root.GetComponentInChildren<PlayerStatScript>().ResetPlayerStatList();

    }

    public void SpecialAttackCounterNest(bool shouldSet)
    {
        if (isServer)
        {
            RpcAttackCounter(shouldSet);
        }
        else
        {
            CmdAttackCounter(shouldSet);
        }
    }

    [ClientRpc]
    private void RpcAttackCounter(bool shouldSet)
    {
        if (isClientOnly)
        {
            if(shouldSet)
            {
                enemyList[0].specialAttackCounter = 0;
            }
            else
            {
                enemyList[0].specialAttackCounter++;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAttackCounter(bool shouldSet)
    {
        if (shouldSet)
        {
            enemyList[0].specialAttackCounter = 0;
        }
        else
        {
            enemyList[0].specialAttackCounter++;
        }
    }

    public void EnemyGenerationNest()
    {
        enemyGeneratorAccess.GenerateEnemies(1, true);
        if (isServer)
        {
            RpcGenerateEnemy();
        }
        else
        {
            CmdGenerateEnemy();
        }
    }

    [ClientRpc]
    private void RpcGenerateEnemy()
    {
        if (isClientOnly)
        {
            enemyGeneratorAccess.GenerateEnemies(1, true);
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdGenerateEnemy()
    {
        enemyGeneratorAccess.GenerateEnemies(1, true);
    }

    public void HealEnemyRefereeScript()
    {
        if (isServer)
        {
            playerList[0].HealEnemyPlayerScript();
        }
        else
        {
            playerList[1].HealEnemyPlayerScript();
        }
    }

    public bool AreAllPlayersDead()
    {
        bool areAllPlayersDead = true;
        foreach (PlayerScript player in playerList)
        {
            if (player.isPlayerAlive)
            {
                areAllPlayersDead = false;
            }
        }
        return areAllPlayersDead;
    }

    private void ResetDamageMultiplier()
    {
        if (isServer)
        {
            RpcResetDamageMultiplier();
        }
        else
        {
            CmdResetDamageMultiplier();
        }
    }

    [ClientRpc]
    private void RpcResetDamageMultiplier()
    {
        if (isClientOnly)
        {
            playerList[1].transform.root.GetComponentInChildren<PlayerStatScript>().damageMultiplier = 1;
            Debug.Log("Playerlist 1");
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdResetDamageMultiplier()
    {
        playerList[0].transform.root.GetComponentInChildren<PlayerStatScript>().damageMultiplier = 1;
        Debug.Log("Playerlist 0");
    }
}
