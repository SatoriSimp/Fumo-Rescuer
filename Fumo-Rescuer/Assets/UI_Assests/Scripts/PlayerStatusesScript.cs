using UnityEngine;

public class PlayerStatusesScript : MonoBehaviour
{
    public GameObject status_P_Melee;
    public GameObject status_P_Caster;

    public void OnSystemStart_UpdatePlayerStatus(bool isCurrentPlayerMelee)
    {
        if (isCurrentPlayerMelee)
        {
            status_P_Melee.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DEFAULT);
            status_P_Caster.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DISABLED);
        }
        else
        {
            status_P_Melee.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DEFAULT);
            status_P_Caster.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DEFAULT);
        }
    }

    public void OnPlayerChange_UpdatePlayerStatus(bool isChangedCharacterMelee)
    {
        if (isChangedCharacterMelee)
        {
            status_P_Melee.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DEFAULT);
            status_P_Caster.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DISABLED);
        }
        else
        {
            status_P_Melee.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DISABLED);
            status_P_Caster.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.DEFAULT);
        }
    }

    public void OnSwitchReady_UpdatePlayerStatus(bool isWaitingCharacterMelee)
    {
        if (isWaitingCharacterMelee)
        {
            status_P_Caster.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.READY);
        }
        else
        {
            status_P_Melee.GetComponent<PlayerIcon>().ChangePlayerSprite(PlayerIcon.SPRITE_STATUS.READY);
        }
    }
}
