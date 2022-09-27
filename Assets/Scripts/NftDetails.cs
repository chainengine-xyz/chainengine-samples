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

public class NftDetails : MonoBehaviour
{
    public TMP_InputField inputFieldNftId;

    public GameObject mainMenu;
    public GameObject nftDetailsMenu;
    public GameObject playerNftsMenu;

    public GameObject navFrom = null;
    public GameObject navTo = null;

    public Image nftDetailsImage;
    public Sprite nftBlankSprite;
    public TMP_Text nftDetailsName;
    public TMP_Text nftDetailsDescription;

    public ChainEngineSDK client;
    public Helper helper;

    // Start is called before the first frame update
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
    }

    public void OnBackButtonClick()
    {
        if (navFrom != null && navTo != null)
        {
            helper.NavFromTo(navFrom, navTo);
        }
        else
        {
            helper.NavFromTo(nftDetailsMenu, mainMenu);
        }

        InitValues();
    }
    public void SetNftDetailsObjects(Nft nft)
    {
        StartCoroutine(helper.GetImageUriNftCoroutine(nft.Metadata.Image, nftDetailsImage));

        nftDetailsName.text = nft.Metadata.Name;
        nftDetailsDescription.text = nft.Metadata.Description;
    }

    public async void GetNFT()
    {
        inputFieldNftId = inputFieldNftId.GetComponent<TMP_InputField>();
        string nftId = inputFieldNftId.text;

        InitValues();

        if (nftId != "")
        {
            var nft = await client.GetNFT(nftId);

            helper.NavFromTo(mainMenu, nftDetailsMenu);

            SetNftDetailsObjects(nft);

            inputFieldNftId.text = "";
            //Debug.Log($"NFT: {nft.Metadata.Image}");
        }
        else
        {
            Debug.LogError("Nft Id cannot be empty");
        }
    }
}
