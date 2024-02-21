using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffCommand : ICommand
{
    private Move move;

    public TurnOffCommand(Move move)
    {
        this.move = move;
    }

    public void Execute()
    {
        move.TurnOff();
    }
}
