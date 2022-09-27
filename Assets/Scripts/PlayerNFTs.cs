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

public class PlayerNFTs : MonoBehaviour
{
    private const int NFT_GRID_ITEMS_LENGTH = 8;

    public GameObject mainMenu;
    public GameObject nftDetailsMenu;
    public GameObject playerNftsMenu;
    public GameObject nftTransferMenu;

    public Sprite nftBlankSprite;

    public RectTransform[] nftPanels;
    public Image[] nftImages;
    public TMP_Text[] nftNames;
    public Button[] nftDetailsButtons;
    public Button[] nftTransferButtons;

    public NftDetails nftDetails;
    public NftTransfer nftTransfer;

    public ChainEngineSDK client;
    public Helper helper;

    void Start()
    {
        client = ChainEngineSDK.Instance();
    }
    private void InitValues()
    {       
        for(int i = 0; i < NFT_GRID_ITEMS_LENGTH; i++)
        {
            nftPanels[i].gameObject.SetActive(false);
            nftImages[i].overrideSprite = nftBlankSprite;
            nftNames[i].text = "Nft Name";
            nftDetailsButtons[i].onClick.RemoveAllListeners();
            nftTransferButtons[i].onClick.RemoveAllListeners();
        };
    }

    private IEnumerator GetPlayerNftsDataCoroutine(PlayerNftCollection nftsRetreiveds)
    {
        int i = 0;

        foreach (var singleNft in nftsRetreiveds.Items())
        {
            nftNames[i].text = singleNft.Metadata.Name;

            nftDetailsButtons[i].onClick.AddListener(delegate
            {
                helper.NavFromTo(playerNftsMenu, nftDetailsMenu);

                nftDetails.navFrom = nftDetailsMenu;
                nftDetails.navTo = playerNftsMenu;
                nftDetails.SetNftDetailsObjects(singleNft);
            });

            nftTransferButtons[i].onClick.AddListener(delegate
            {
                helper.NavFromTo(playerNftsMenu, nftTransferMenu);

                nftTransfer.SetNftObjects(singleNft);
            });

            nftPanels[i].gameObject.SetActive(true);
            StartCoroutine(helper.GetImageUriNftCoroutine(singleNft.Metadata.Image, nftImages[i]));
            Debug.Log($"NFT: {singleNft.Metadata.Name}");
            i++;
        }

        yield break;
    }

    public void OnBackToMainMenuClick(GameObject from, GameObject to)
    {
        helper.NavFromTo(playerNftsMenu, mainMenu);
    }

    public void OnPlayerNftsMenuButtonOnClick()
    {
        helper.NavFromTo(mainMenu, playerNftsMenu);
        GetPlayerNFTs();
    }
    public async void GetPlayerNFTs()
    {
        InitValues();
        PlayerNftCollection nftsRetreiveds = await client.GetPlayerNFTs();

        StartCoroutine(GetPlayerNftsDataCoroutine(nftsRetreiveds));
    }
}
