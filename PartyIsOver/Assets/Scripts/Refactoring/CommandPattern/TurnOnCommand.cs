using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnCommand : ICommand
{
    private Move move;

    public TurnOnCommand(Move move)
    {
        this.move = move;
    }

    public void Execute()
    {
        move.TurnOn();
    }

}
