using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject panelInstrucciones;

    public void Jugar()
    {
        SceneManager.LoadScene("2BOSQUE");
    }

    public void Instrucciones()
    {
        panelInstrucciones.SetActive(true);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
