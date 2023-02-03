using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    // Start is called before the first frame update
    public void DisappearEverything()
    {
        gameObject.SetActive(false);
    }
}
