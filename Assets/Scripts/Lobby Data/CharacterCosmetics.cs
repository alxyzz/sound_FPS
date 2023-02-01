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

    private PlayerController _playerController;

    private void Start() {
        currentColorIndex = PlayerPrefs.GetInt("currentColorIndex", 0);
        currentColorImage.color = playerColors[currentColorIndex].color;
        currentColorText.text = playerColors[currentColorIndex].name;

    }

    public void NextColor() {
        if (_playerController == null) {
            _playerController = GameObject.Find("LocalGamePlayer").GetComponent<PlayerController>();
        }

        if (currentColorIndex < playerColors.Length - 1) {
            currentColorIndex++;
            PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);

            currentColorImage.color = playerColors[currentColorIndex].color;
            currentColorText.text = playerColors[currentColorIndex].name;

        }

        _playerController.PlayerColor = currentColorIndex;
    }

    public void PreviousColor() {
        if (_playerController == null) {
            _playerController = GameObject.Find("LocalGamePlayer").GetComponent<PlayerController>();
        }

        if (currentColorIndex > 0) {
            currentColorIndex--;
            PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
            
            currentColorImage.color = playerColors[currentColorIndex].color;
            currentColorText.text = playerColors[currentColorIndex].name;

        }

        _playerController.PlayerColor = currentColorIndex;
    }
}
