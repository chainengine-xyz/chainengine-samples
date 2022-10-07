using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChainEngine;
using UnityEngine.UI;
using TMPro;
using ChainEngine.Model;

public class NftTransfer : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playerNftsMenu;
    public GameObject nftTransferMenu;

    public GameObject navFrom = null;
    public GameObject navTo = null;

    private string walletAddress;
    private string nftId;
    private string amount;

    public Image nftDetailsImage;
    public Sprite nftBlankSprite;
    public TMP_Text nftDetailsName;
    public TMP_Text nftDetailsDescription;
    public TMP_Text nftAmount;

    public TMP_InputField inputFieldWalletAddress;
    public TMP_InputField inputFieldAmount;

    public PlayerNFTs playerNFTs;

    public ChainEngineSDK client;
    public Helper helper;

    void Start()
    {
        client = ChainEngineSDK.Instance();
    }
    private void InitValues()
    {
        navFrom = null;
        navTo = null;

        nftDetailsImage.overrideSprite = nftBlankSprite;
        nftDetailsName.text = "Nft Name";
        nftDetailsDescription.text = "Nft Description";
        nftAmount.text = "0";

        inputFieldAmount.text = "";
        inputFieldWalletAddress.text = "";
    }
    public void SetNftObjects(Nft nft)
    {
        StartCoroutine(helper.GetImageUriNftCoroutine(nft.Metadata.Image, nftDetailsImage));

        nftId = nft.Id;
        nftDetailsName.text = nft.Metadata.Name;
        nftDetailsDescription.text = nft.Metadata.Description;
        nftAmount.text = nft.Holders[client.Player.Id].ToString();
}
    public void OnBackButtonClick()
    {
        helper.NavFromTo(nftTransferMenu, playerNftsMenu);
        playerNFTs.GetPlayerNFTs();
        InitValues();
    }
    public async void OnSendButtonClick()
    {
        inputFieldWalletAddress = inputFieldWalletAddress.GetComponent<TMP_InputField>();
        walletAddress = inputFieldWalletAddress.text;

        inputFieldAmount = inputFieldAmount.GetComponent<TMP_InputField>();
        amount = inputFieldAmount.text;

        TransferNFT();
    }
    private void TransferNFT()
    {
        client.TransferNft(walletAddress, nftId, int.Parse(amount));

        Debug.Log($"Wallet: {walletAddress}, NFT: {nftId}, Amount: {int.Parse(amount)}");
    }
}
