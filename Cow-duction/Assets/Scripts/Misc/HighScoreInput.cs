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
    void FixedUpdate()
    {
        if(inputField.activeSelf)
        {
            bool isRight = (Input.GetAxis("P1DPADX") < -0.1f) ? true : false;

            bool isLeft = (Input.GetAxis("P1DPADX") > 0.1f) ? true : false;

            bool isDown = (Input.GetAxis("P1DPADY") < -0.1f) ? true : false;

            bool isUp = (Input.GetAxis("P1DPADY") > 0.1f) ? true : false;
            bool is2Right = (Input.GetAxis("P2DPADX") < -0.1f) ? true : false;

            bool is2Left = (Input.GetAxis("P2DPADX") > 0.1f) ? true : false;

            bool is2Down = (Input.GetAxis("P2DPADY") < -0.1f) ? true : false;

            bool is2Up = (Input.GetAxis("P2DPADY") > 0.1f) ? true : false;
            if(isUp) //player 1 input up
            {
                player1Index[player1position] = Increase(player1Input[player1position], player1Index[player1position]);
            }
            if(is2Up) //player2 input up
            {
                player2Index[player2position] = Increase(player2Input[player2position], player2Index[player2position]);
            }
            if (isDown) //player 1 input down
            {
                player1Index[player1position] = Decrease(player1Input[player1position], player1Index[player1position]);
            }
            if (is2Down) //player2 input down
            {
                player2Index[player2position] = Decrease(player2Input[player2position], player2Index[player2position]);
            }
            if (isLeft) //player 1 input left
            {
                player1Input[player1position].fontStyle = FontStyle.Bold;
                player1position--;
                if (player1position < 0)
                    player1position = 2;
                player1Input[player1position].fontStyle = FontStyle.BoldAndItalic;

            }
            if (is2Left) //player2 input left
            {
                player2Input[player2position].fontStyle = FontStyle.Bold;
                player2position--;
                if (player2position < 0)
                    player2position = 2;
                player2Input[player2position].fontStyle = FontStyle.BoldAndItalic;

            }
            if (isRight) //player 1 input right
            {
                player1Input[player1position].fontStyle = FontStyle.Bold;
                player1position = (player1position + 1) % 3;
                player1Input[player1position].fontStyle = FontStyle.BoldAndItalic;
            }
            if (is2Right) //player2 input right
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
