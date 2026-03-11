using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ai_proxy_client : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text outputText;

    string serverUrl = "http://localhost:3000/ask";

    [Serializable]
    public class RequestBody
    {
        public string message;
    }

    [Serializable]
    public class ResponseBody
    {
        public string reply;
    }

    public void SendMessage()
    {
        StartCoroutine(PostRequest(inputField.text));
    }

    IEnumerator PostRequest(string message)
    {
        RequestBody body = new RequestBody();
        body.message = message;

        string json = JsonUtility.ToJson(body);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                outputText.text = request.error;
                yield break;
            }

            ResponseBody res = JsonUtility.FromJson<ResponseBody>(request.downloadHandler.text);

            outputText.text = res.reply;
        }
    }
}