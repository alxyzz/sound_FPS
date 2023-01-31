
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

using UnityEngine.UI;
using Ping = UnityEngine.Ping;

public class MainMenuUI : MonoBehaviour
{
    //[SerializeField] private TMP_InputField _lobbyNicknameTextbox;

    public GameObject skillIssue;
   
    private void Start()
    {
        EpilepsyMenu.SetActive(true);
    }



    public string GetLocalIP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "Your network adapter does not seem to have an Ipv4 Local/AddressFamily.InterNetwork address.";;
    }

    public GameObject creditsmenu;
    public void OnClickCreditsEnter()
    {
        creditsmenu.SetActive(true);
    }

    public void OnClickCreditsExit()
    {
        creditsmenu.SetActive(false);
    }

    private void SetChildrenEnabled(bool enabled)
    {
        foreach (var item in GetComponentsInChildren<Selectable>())
        {
            item.interactable = enabled;
        }       
    }

    public void OnClickHost()
    {
       
        SetChildrenEnabled(false);
       // NetworkManager_ArenaFPS.singleton.StartHost();
    }
    [Header("Popup Hint")]
    [SerializeField] private RectTransform _popupHintList;
    [SerializeField] private GameObject _pfbPopupHint;

    public void onClickQuit()
    {
        Application.Quit();
    }

    public void SkillIssue()
    {
        skillIssue.SetActive(true);
        StartCoroutine(skiller());
;    }

    IEnumerator skiller()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        skillIssue.SetActive(false);
    }

    public static void AddPopupHint(string content)
    {
        //UI_Cmn_PopupHint popup = Instantiate(instance._pfbPopupHint, instance._popupHintList).GetComponent<UI_Cmn_PopupHint>();
        //popup.Appear(content);

        Debug.Log("Popup message simulated with content: " + content);
    }
   

 
    

  
    
    
    

    public void OnClickJoin()
    {
        SetChildrenEnabled(false);
       


    }

    [SerializeField] GameObject EpilepsyMenu;
    [SerializeField] AnimateGridMaterialWithSound menuScript;
    public void onClickEpilepticYes()
    {
        menuScript.started = true;
        menuScript.epilepsy = true;
        EpilepsyMenu.SetActive(false);
        //grid.GetComponent<Animator>().enabled = false;

    }

    public void onClickEpilepticNo()
    {
        menuScript.started = true;
        menuScript.epilepsy = false;
        EpilepsyMenu.SetActive(false);
    }

}
