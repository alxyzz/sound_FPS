using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Steamworks;

public class CharacterCosmetics : MonoBehaviour
{
    public int currentColorIndex = 0;
    public Material[] playerColors;
    public Image currentColorImage;
    public TMP_Text currentColorText;
    public Sprite defaultSprite;

    private PlayerObjectController playerObjectController;

    private void Start() {
        currentColorIndex = PlayerPrefs.GetInt("currentColorIndex", 0);
        currentColorImage.color = playerColors[currentColorIndex].color;
        currentColorText.text = playerColors[currentColorIndex].name;

    }

    public void NextColor() {
        if (playerObjectController == null) {
            playerObjectController = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();
        }

        if (currentColorIndex < playerColors.Length - 1) {
            currentColorIndex++;
            PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);

            currentColorImage.color = playerColors[currentColorIndex].color;
            currentColorText.text = playerColors[currentColorIndex].name;

        }

        playerObjectController.PlayerColor = currentColorIndex;
    }

    public void PreviousColor() {
        if (playerObjectController == null) {
            playerObjectController = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();
        }

        if (currentColorIndex > 0) {
            currentColorIndex--;
            PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
            
            currentColorImage.color = playerColors[currentColorIndex].color;
            currentColorText.text = playerColors[currentColorIndex].name;

        }

        playerObjectController.PlayerColor = currentColorIndex;
    }
}
