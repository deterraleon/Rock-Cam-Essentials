using Il2CppRUMBLE.Interactions.InteractionBase.Extensions;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock_Cam_Essentials
{

    public class HandleBars
    {
        public InteractionHoldableGroup _Handles;
        public int isHeld = 0;
        public int isDoubleHeld = 0;
        public HandleBars(Rock_Cam rockcam)
        {
            _Handles = rockcam._Tablet.InteractionHoldableGroup;
        }
        public int isHeldUpdate()
        {
            try
            {
                if (_Handles.GroupIsBeingHeld() != (isHeld % 2 == 1)) { isHeld = 2; }
                else { isHeld = 0; }
                if (_Handles.GroupIsBeingHeld()) { isHeld++; }
                return isHeld;
            }
            catch (Exception ex)
            {
                MelonLogger.Msg("Hai");
                MelonLogger.Error(ex);
                return isHeld;
            }
        }
        public int isDoubleHeldUpdate()
        {
            try
            {
                if (_Handles.DualHoldingObject != (isDoubleHeld % 2 == 1)) { isDoubleHeld = 2; }
                else { isDoubleHeld = 0; }
                if (_Handles.DualHoldingObject) { isDoubleHeld++; }
                return isDoubleHeld;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return isDoubleHeld;
            }
        }
    }
}
