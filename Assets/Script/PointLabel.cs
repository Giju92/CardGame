using UnityEngine;
using UnityEngine.UI;

public class PointLabel : MonoBehaviour {

    void Start()
    {
        PlayManager.OnPointChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        GetComponent<Text>().text = "punti\n" + newVal;
    }
}
