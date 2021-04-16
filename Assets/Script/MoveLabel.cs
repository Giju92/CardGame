
using UnityEngine;
using UnityEngine.UI;

public class MoveLabel : MonoBehaviour {

    void Start()
    {
        PlayManager.OnMovesChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        GetComponent<Text>().text = "mosse\n" + newVal;
    }
}
