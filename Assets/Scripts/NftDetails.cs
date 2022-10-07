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
    private const int CUSTOM_PROPS_GRID_ITEMS_COUNT = 6;

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
    public TMP_Text nftAmount;

    public GameObject customPropsCanvas;
    public RectTransform[] customPropsPanels;
    public TMP_Text[] customPropsKeys;
    public TMP_Text[] customPropsValues;

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

        for (int i = 0; i < CUSTOM_PROPS_GRID_ITEMS_COUNT; i++)
        {
            customPropsPanels[i].gameObject.SetActive(false);
            customPropsKeys[i].text = "Custom Props Key";
            customPropsValues[i].text = "Custom Props Value";
        };
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
        nftAmount.text = nft.Holders[client.Player.Id].ToString();

        if (nft.Metadata.Attributes != null)
        {
            int i = 0;
            foreach (var attr in nft.Metadata.Attributes)
            {
                if(i < CUSTOM_PROPS_GRID_ITEMS_COUNT)
                {
                    customPropsKeys[i].text = attr.Key;
                    customPropsValues[i].text = attr.Value.ToString();
                    customPropsPanels[i].gameObject.SetActive(true);
                    i++;
                }
            }
        }
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
