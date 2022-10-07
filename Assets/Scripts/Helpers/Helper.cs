using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    public Button[] paginationButtons;
    public Sprite paginationCurrentPageSprite;
    public Sprite paginationNormalPageSprite;
    private Color paginationCurrentFontColor = Color.white;
    private Color paginationNormalFontColor = Color.black;

    public IEnumerator GetImageUriNftCoroutine(string uri, Image img)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture($"{uri}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D nftTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            Sprite sprite = Sprite.Create(nftTexture, new Rect(0, 0, nftTexture.width, nftTexture.height), new Vector2(nftTexture.width / 2, nftTexture.height / 2));
            img.overrideSprite = sprite;
        }

        www.Dispose();
        yield break;
    }
    public void NavFromTo(GameObject fromMenu, GameObject toMenu)
    {
        fromMenu.SetActive(false);
        toMenu.SetActive(true);
    }

    public void SetPaginationButtonState(Button button, string page, bool isCurrent)
    {
        button.GetComponentInChildren<TMP_Text>().text = page;

        if (isCurrent)
        {
            button.GetComponentInChildren<TMP_Text>().color = paginationCurrentFontColor;
            button.image.overrideSprite = paginationCurrentPageSprite;
        }
        else
        {
            button.GetComponentInChildren<TMP_Text>().color = paginationNormalFontColor;
            button.image.overrideSprite = paginationNormalPageSprite;
        }
    }
    public void SetPaginationCurrentPage(int currentPage, long totalPage)
    {
        for (int i = 0; i < 3; i++)
        {
            int page = int.Parse(paginationButtons[i].GetComponentInChildren<TMP_Text>().text);
            if (page == currentPage && currentPage != 1 && currentPage != totalPage)
            {
                SetPaginationButtonState(paginationButtons[i], page.ToString(), true);
                break;
            }
        }
    }
}
