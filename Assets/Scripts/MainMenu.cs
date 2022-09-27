using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using ChainEngine;
using ChainEngine.Actions;
using ChainEngine.Shared.Exceptions;
using ChainEngine.Model;
using ChainEngine.Types;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField inputFieldWalletAddress;
    public TMP_InputField inputFieldNftId;

    public GameObject mainMenu;
    public GameObject nftDetails;
    public GameObject nftGrid;

    public Image nftImage;
    public TMP_Text nftName;
    public TMP_Text nftDescription;
    public Button netModeButton;

    public Sprite mainNetModeSprite;
    public Sprite testNetModeSprite;

    private ChainEngineSDK client;
    public Helper helper;

    // Start is called before the first frame update
    void Start()
    {
        client = ChainEngineSDK.Instance();
    }
    public void OnBackToMainMenuFromNftDetailsClick()
    {
        helper.NavFromTo(nftDetails, mainMenu);
    }

    public void OnBackToMainMenuFromNftGridClick()
    {
        helper.NavFromTo(nftGrid, mainMenu);
    }

    public async void CreateOrFetchPlayer()
    {
        inputFieldWalletAddress = inputFieldWalletAddress.GetComponent<TMP_InputField>();
        string walletAddress = inputFieldWalletAddress.text;

        if (walletAddress != "")
        {
            Player player = await client.CreateOrFetchPlayer(walletAddress);

            inputFieldWalletAddress.text = "";

            Debug.Log($"Player Id: {player?.Id}\n" +
                      $"Wallet Address: {client.Player?.WalletAddress}");
        }
        else
        {
            Debug.LogError("Wallet address cannot be empty");
        }
        
    }

    public void WalletLogin()
    {
        client.PlayerAuthentication();

        Debug.Log("Launching player auth");
    }

    public void TrustWalletLogin()
    {
        client.PlayerAuthentication(WalletProvider.TrustWallet);

        Debug.Log("Launching player auth with TrustWallet provider");
    }

    public void MetamaskLogin()
    {
        client.PlayerAuthentication(WalletProvider.Metamask);

        Debug.Log("Launching player auth with Metamask provider");
    }

    public void CoinbaseLogin()
    {
        client.PlayerAuthentication(WalletProvider.Coinbase);

        Debug.Log("Launching player auth with Coinbase provider");
    }

    public void ToggleNetMode()
    {
        if(client.ApiMode == "mainnet")
        {
            client.SetTestNetMode();
            netModeButton.image.overrideSprite = testNetModeSprite;
        }
        else
        {
            client.SetMainNetMode();
            netModeButton.image.overrideSprite = mainNetModeSprite;
        }

        Debug.Log($"Net mode changed to '{client.ApiMode}'");
    }

    private void OnAuthSuccess(Player player)
    {
        Debug.Log($"Player Id: {player?.Id}\n" +
                  $"Wallet Address: {player?.WalletAddress}\n" +
                  $"Token: {player?.Token}\n");
    }

    private void OnAuthFailure(AuthError error)
    {
        Debug.Log(error);
    }
}
