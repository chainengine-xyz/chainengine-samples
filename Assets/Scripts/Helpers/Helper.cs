using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
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
}
