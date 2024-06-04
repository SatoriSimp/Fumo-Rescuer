using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4Script : MonoBehaviour
{
    public GameObject[] Triggers;
    public GameObject[] Gates;
    private short currentSection = 1;

    [SerializeField] private GameObject section2_GateObjPrefab;
    private GameObject section2_GateObj;
    [SerializeField] private GameObject section2_HallChokepoint;
    
    WispSpawnpoint[] Spawnpoints;
    [SerializeField] private GameObject section3_WispTooltips;
    public AudioSource BGM;

    public GameObject Fumo;

    private void Start()
    {
        Spawnpoints = FindObjectsOfType<WispSpawnpoint>();
        DisableAllWispSpawnpoints();
        StartCoroutine(PlayAudio());
        Fumo = GameObject.FindGameObjectWithTag("FumoTrophy");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentSection)
        {
            case 1:
                CheckSection_1();
                break;
            case 2:
                CheckSection_2();
                break;
            case 3:
                CheckSection_3();
                break;
            case 4:
                CheckSection_4();
                break;
        }
    }

    void CheckSection_1() 
    {
        if (Triggers[0]) return;

        EnemyBehavior_Tyrant Gatekeeper_Tyrant = FindFirstObjectByType<EnemyBehavior_Tyrant>();
        
        if (!Gatekeeper_Tyrant || Gatekeeper_Tyrant.currentHealth <= 0)
        {
            if (Gates[0]) Destroy(Gates[0]);
            currentSection = 2;
        }
    }

    void CheckSection_2()
    {
        if (Triggers[1]) return;

        EnemyBehavior_Splitcaster Gatekeeper_Splitcaster = FindFirstObjectByType<EnemyBehavior_Splitcaster>();

        if (!section2_GateObj) section2_GateObj = Instantiate(section2_GateObjPrefab);

        if (!Gatekeeper_Splitcaster || Gatekeeper_Splitcaster.currentHealth <= 0)
        {
            if (Gates[1]) Destroy(Gates[1]);
            if (section2_GateObj) Destroy(section2_GateObj);
            Instantiate(section2_HallChokepoint, new Vector3(860f, 260f, 0), Quaternion.identity);
            currentSection = 3;
        }
    }

    void CheckSection_3()
    {
        if (Triggers[2]) return;

        EnemyBehavior_Flayer Gatekeeper_Flayer = FindFirstObjectByType<EnemyBehavior_Flayer>();

        if (!Gatekeeper_Flayer || Gatekeeper_Flayer.currentHealth <= 0)
        {
            if (Gates[2]) Destroy(Gates[2]);
            EnableAllWispSpawnpoints();
            Instantiate(section3_WispTooltips, new Vector3(2457f, 122f, 0f), Quaternion.identity);
            currentSection = 4;
        }
    }

    void CheckSection_4()
    {
        if (Fumo) return;
        DisableAllWispSpawnpoints();
        currentSection = 5;
    }

    void DisableAllWispSpawnpoints()
    {
        foreach (WispSpawnpoint sp in Spawnpoints)
        {
            sp.gameObject.SetActive(false);
        }
    }

    void EnableAllWispSpawnpoints()
    {
        foreach (WispSpawnpoint sp in Spawnpoints)
        {
            sp.gameObject.SetActive(true);
        }
    }

    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(5);
        if (!BGM.isPlaying) BGM.Play();
    }
}
