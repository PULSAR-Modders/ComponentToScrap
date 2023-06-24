using HarmonyLib;

namespace ComponentToScrap
{
    [HarmonyPatch(typeof(PLServer), "CaptainFlushAirlockComponent")]
    internal class Server
    {
        private static int slotIndex = -1;
        private static int level = 0;

        private static bool Prefix(int inShipID, int inNetID, PhotonMessageInfo pmi, out bool __state)
        {
            PLPlayer playerForPhotonPlayer = PLServer.GetPlayerForPhotonPlayer(pmi.sender);
            PLShipInfoBase shipFromID = PLEncounterManager.Instance.GetShipFromID(inShipID);
            PLShipComponent componentFromNetID = shipFromID.MyStats.GetComponentFromNetID(inNetID);
            bool result;

            if (playerForPhotonPlayer == null || playerForPhotonPlayer.GetClassID() != 0 || componentFromNetID == null || componentFromNetID.GetType() == typeof(PLScrapCargo))
            {
                __state = false;
                result = true;
            }
            else
            {
                PLSlot slot = shipFromID.MyStats.GetSlot(ESlotType.E_COMP_CARGO);
                if (slot.Count < slot.MaxItems || componentFromNetID.Slot.Equals(slot))
                {
                    if (componentFromNetID.Slot.Equals(slot))
                    {
                        slotIndex = componentFromNetID.SortID;
                    }
                    level = componentFromNetID.Level;
                    __state = true;
                    result = true;
                }
                else
                {
                    __state = false;
                    result = false;
                }
            }
            return result;
        }

        private static void Postfix(bool __state)
        {
            if (__state)
            {
                PLScrapCargo plscrapCargo = new PLScrapCargo(level);
                PLEncounterManager.Instance.PlayerShip.MyStats.AddShipComponent(plscrapCargo, -1, ESlotType.E_COMP_CARGO);
                if (slotIndex >= 0)
                {
                    PLServer.Instance.photonView.RPC("CaptainRearrangeShipComponent", PhotonTargets.MasterClient, new object[]
                    {
                        PLEncounterManager.Instance.PlayerShip.ShipID,
                        plscrapCargo.NetID,
                        -1,
                        (int)plscrapCargo.Slot.Type,
                        (int)plscrapCargo.VisualSlotType,
                        slotIndex
                    });
                    slotIndex = -1;
                }
            }
        }
    }
}
