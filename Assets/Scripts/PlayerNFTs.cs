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
    private const int NFT_GRID_ITEMS_COUNT = 8;
    private const int PAGINATION_BUTTONS_COUNT = 3;
    private int perPage = NFT_GRID_ITEMS_COUNT;
    private int currentPage = 1;
    private int totalItems;
    private long totalPage = 0;

    public GameObject mainMenu;
    public GameObject nftDetailsMenu;
    public GameObject playerNftsMenu;
    public GameObject nftTransferMenu;

    public Sprite nftBlankSprite;
    public Sprite paginationCurrentPageSprite;
    public Sprite paginationNormalPageSprite;

    public RectTransform[] nftPanels;
    public Image[] nftImages;
    public TMP_Text[] nftNames;
    public Button[] nftDetailsButtons;
    public Button[] nftTransferButtons;
    public Button[] paginationButtons;
    public TMP_Text paginationEllipsisPrev;
    public TMP_Text paginationEllipsisNext;
    public Button paginationPrevButton;
    public Button paginationNextButton;
    public Button paginationFirstPageButton;
    public Button paginationLastPageButton;

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
        for(int i = 0; i < NFT_GRID_ITEMS_COUNT; i++)
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
            if(i < NFT_GRID_ITEMS_COUNT)
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
                i++;
            }
        }

        yield break;
    }

    public void OnBackButtonClick()
    {
        helper.NavFromTo(playerNftsMenu, mainMenu);
    }

    public void OnPlayerNftsMenuButtonOnClick()
    {
        helper.NavFromTo(mainMenu, playerNftsMenu);
        GetPlayerNFTs();
    }
    public void OnPaginationPrevClick()
    {
        currentPage -= 1;
        HandlePagination();
    }
    public void OnPaginationNextClick()
    {
        currentPage += 1;
        HandlePagination();
    }
    public void OnPaginationFirstClick()
    {
        currentPage = 1;
        HandlePagination();
    }
    public void OnPaginationLastClick()
    {
        currentPage = (int)totalPage;
        HandlePagination();
    }
    public void OnPaginationButton0Click()
    {
        currentPage = int.Parse(paginationButtons[0].GetComponentInChildren<TMP_Text>().text);
        HandlePagination();
    }
    public void OnPaginationButton1Click()
    {
        currentPage = int.Parse(paginationButtons[1].GetComponentInChildren<TMP_Text>().text);
        HandlePagination();
    }
    public void OnPaginationButton2Click()
    {
        currentPage = int.Parse(paginationButtons[2].GetComponentInChildren<TMP_Text>().text);
        HandlePagination();
    }

    public async void HandlePagination()
    {
        InitValues();
        PlayerNftCollection nftsRetreiveds = await client.GetPlayerNFTs();
        totalItems = nftsRetreiveds.Items().Count;
        totalPage = (-1L + totalItems + perPage) / perPage;

        nftsRetreiveds = await client.GetPlayerNFTs(currentPage, perPage);
        StartCoroutine(GetPlayerNftsDataCoroutine(nftsRetreiveds));

        paginationLastPageButton.gameObject.SetActive(true);
        paginationButtons[0].gameObject.SetActive(false);
        paginationButtons[1].gameObject.SetActive(false);
        paginationButtons[2].gameObject.SetActive(false);

        switch (totalPage)
        {
            case 1:
                paginationLastPageButton.gameObject.SetActive(false);
                break;

            case 2:
                paginationLastPageButton.gameObject.SetActive(true);
                break;

            case 3:
                paginationButtons[0].gameObject.SetActive(true);

                helper.SetPaginationButtonState(paginationButtons[0], "2", false);
                break;

            case 4:
                paginationButtons[0].gameObject.SetActive(true);
                paginationButtons[1].gameObject.SetActive(true);

                helper.SetPaginationButtonState(paginationButtons[0], "2", false);
                helper.SetPaginationButtonState(paginationButtons[1], "3", false);
                break;

            case 5:
                paginationButtons[0].gameObject.SetActive(true);
                paginationButtons[1].gameObject.SetActive(true);
                paginationButtons[2].gameObject.SetActive(true);

                helper.SetPaginationButtonState(paginationButtons[0], "2", false);
                helper.SetPaginationButtonState(paginationButtons[1], "3", false);
                helper.SetPaginationButtonState(paginationButtons[2], "4", false);
                break;

            default:
                paginationButtons[0].gameObject.SetActive(true);
                paginationButtons[1].gameObject.SetActive(true);
                paginationButtons[2].gameObject.SetActive(true);
                break;
        }

        if (totalPage < 6)
        {
            paginationEllipsisPrev.gameObject.SetActive(false);
            paginationEllipsisNext.gameObject.SetActive(false);
        }
        
        if (totalPage >= 6 && currentPage >= 4 && currentPage <= totalPage - 3)
        {
            paginationEllipsisPrev.gameObject.SetActive(true);
            paginationEllipsisNext.gameObject.SetActive(true);
        }

        if (totalPage >= 6 && currentPage >= 4)
        {
            paginationEllipsisPrev.gameObject.SetActive(true);
        }

        if (totalPage >= 6 && currentPage <= totalPage - 3)
        {
            paginationEllipsisNext.gameObject.SetActive(true);
        }

        if (totalPage >= 6 && currentPage < 4)
        {
            paginationEllipsisPrev.gameObject.SetActive(false);
        }

        if (totalPage >= 6 && currentPage > totalPage - 3)
        {
            paginationEllipsisNext.gameObject.SetActive(false);
        }

        if (totalPage >= 6 && currentPage < 4)
        {
            helper.SetPaginationButtonState(paginationButtons[0], "2", false);
            helper.SetPaginationButtonState(paginationButtons[1], "3", false);
            helper.SetPaginationButtonState(paginationButtons[2], "4", false);
        }

        if (totalPage >= 6 && currentPage > totalPage - 3)
        {
            helper.SetPaginationButtonState(paginationButtons[0], (totalPage - 3).ToString(), false);
            helper.SetPaginationButtonState(paginationButtons[1], (totalPage - 2).ToString(), false);
            helper.SetPaginationButtonState(paginationButtons[2], (totalPage - 1).ToString(), false);
        }

        if (totalPage >= 6 && currentPage >= 4 && currentPage <= totalPage - 3)
        {
            helper.SetPaginationButtonState(paginationButtons[0], (currentPage - 1).ToString(), false);
            helper.SetPaginationButtonState(paginationButtons[1], (currentPage).ToString(), false);
            helper.SetPaginationButtonState(paginationButtons[2], (currentPage + 1).ToString(), false);
        }

        if (currentPage <= 1)
        {
            paginationPrevButton.interactable = false;
        }
        else
        {
            paginationPrevButton.interactable = true;
        }

        if (currentPage >= totalPage)
        {
            paginationNextButton.interactable = false;
        }
        else
        {
            paginationNextButton.interactable = true;
        }

        helper.SetPaginationButtonState(paginationFirstPageButton, "1", false);
        helper.SetPaginationButtonState(paginationLastPageButton, totalPage.ToString(), false);

        if (currentPage == 1)
        {
            helper.SetPaginationButtonState(paginationFirstPageButton, "1", true);
        }

        if (currentPage == totalPage)
        {
            helper.SetPaginationButtonState(paginationLastPageButton, totalPage.ToString(), true);
        }

        helper.SetPaginationCurrentPage(currentPage, totalPage);
    }
    public void GetPlayerNFTs()
    {
        currentPage = 1;
        HandlePagination();
    }
}
