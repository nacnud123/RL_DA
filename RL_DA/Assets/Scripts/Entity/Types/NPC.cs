using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public void onTalk()
    {
        UIManager.init.addMsg("You are talking to NPC", "#4287f5");
    }

}
