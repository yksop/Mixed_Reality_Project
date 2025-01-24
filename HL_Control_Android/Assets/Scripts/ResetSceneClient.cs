using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// client-side script that sends a POST request to the server to reset the scene.
/// </summary>
public class ResetSceneClient : MonoBehaviour
{
    private const string ServerUrl = "http://localhost:8080/reset-scene";

    // Metodo collegato al bottone UI
    public async void SendResetCommand()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(ServerUrl, null);
                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Comando inviato con successo! Scena resettata.");
                }
                else
                {
                    Debug.LogError("Errore nell'invio del comando.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Errore nella connessione al server: {ex.Message}");
            }
        }
    }
}
