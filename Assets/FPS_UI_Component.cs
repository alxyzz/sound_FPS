using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_UI_Component : MonoBehaviour
{

    public TextMeshProUGUI GameLogRef;
    public TextMeshProUGUI health;
    public TextMeshProUGUI kills;
    public GameObject beatSymbol;
    [HideInInspector]public FPSMovementController fpscontrol;

    private bool started;

    public void StartBeat()
    {
        StopCoroutine(beat());
        SizeUpBeat(false);


        StartCoroutine(beat());
    }

    IEnumerator beat()
    {
        fpscontrol.onbeat = true;
        SizeUpBeat(true);

        yield return new WaitForSeconds(0.8f);
        SizeUpBeat(false);
        fpscontrol.onbeat = false;
    }

    private void SizeUpBeat(bool b)
    {
        if (b)
        {
            beatSymbol.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
        }
        else
        {
            beatSymbol.transform.localScale = Vector3.one;
        }
    }

    

    void Start()
    {
        StartBeat();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
