using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreInput : MonoBehaviour
{
    public List<Text> player1Input;
    private int[] player1Index;
    private int player1position;
    public List<Text> player2Input;
    private int[] player2Index;
    private int player2position;
    private readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public GameObject inputField;

    void Awake()
    {
        player1Index = new int[3];
        player2Index = new int[3];
        player1position = 0;
        player2position = 0;
        player1Input[player1position].fontStyle = FontStyle.BoldAndItalic;
        player2Input[player2position].fontStyle = FontStyle.BoldAndItalic;
    }
    void Update()
    {
        if(inputField.activeSelf)
        {
            if(Input.GetButtonDown("P1UP")) //player 1 input up
            {
                player1Index[player1position] = Increase(player1Input[player1position], player1Index[player1position]);
            }
            if(Input.GetButtonDown("P2UP") && Input.GetJoystickNames().Length > 1) //player2 input up
            {
                player2Index[player2position] = Increase(player2Input[player2position], player2Index[player2position]);
            }
            if (Input.GetButtonDown("P1DOWN")) //player 1 input down
            {
                player1Index[player1position] = Decrease(player1Input[player1position], player1Index[player1position]);
            }
            if (Input.GetButtonDown("P2DOWN") && Input.GetJoystickNames().Length > 1) //player2 input down
            {
                player2Index[player2position] = Decrease(player2Input[player2position], player2Index[player2position]);
            }
            if (Input.GetButtonDown("P1LEFT")) //player 1 input left
            {
                player1Input[player1position].fontStyle = FontStyle.Bold;
                player1position--;
                if (player1position < 0)
                    player1position = 2;
                player1Input[player1position].fontStyle = FontStyle.BoldAndItalic;
            }
            if (Input.GetButtonDown("P2LEFT") && Input.GetJoystickNames().Length > 1) //player2 input left
            {
                player2Input[player2position].fontStyle = FontStyle.Bold;
                player2position--;
                if (player2position < 0)
                    player2position = 2;
                player2Input[player2position].fontStyle = FontStyle.BoldAndItalic;
            }
            if (Input.GetButtonDown("P1RIGHT")) //player 1 input right
            {
                player1Input[player1position].fontStyle = FontStyle.Bold;
                player1position = (player1position + 1) % 3;
                player1Input[player1position].fontStyle = FontStyle.BoldAndItalic;
            }
            if (Input.GetButtonDown("P2RIGHT") && Input.GetJoystickNames().Length > 1) //player2 input right
            {
                player2Input[player2position].fontStyle = FontStyle.Bold;
                player2position = (player2position + 1) % 3;
                player2Input[player2position].fontStyle = FontStyle.BoldAndItalic;
            }
        }
    }
    public int Increase(Text t, int index)
    {
        index = (index + 1) % 26;
        t.text = Alphabet[index].ToString();
        return index;

    }
    public int Decrease(Text t, int index)
    {
        index--;
        if (index < 0)
            index = 25;
        t.text = Alphabet[index].ToString();
        return index;
    }
}
