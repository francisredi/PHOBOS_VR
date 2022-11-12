using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using CurvedUI;

//Import SteamVR and add CURVEDUI_VIVE to your custom defines to use this class.
#if CURVEDUI_VIVE
using Valve.VR;

namespace CurvedUI {

    /// <summary>
    /// InputModule made for Vive controllers. Create PointerEvents that are later used by eventSystem to discover interactions.
    /// </summary>
    public class CurvedUIViveInputModule : BaseInputModule
    {

        #region SETTINGS
        [Tooltip("Which controller can cause Events to be fired and interact with game world. Default Right.")]
        public ActiveViveController EventController = ActiveViveController.Right;

        // name of button to use for click/submit
        public string submitButtonName = "Fire1";

        //// name of axis to use for scrolling/sliders
        //public string controlAxisName = "Horizontal";
        #endregion

        #region INTERNAL SETTINGS
        public enum ActiveViveController
        {
            Both = 0,
            Right = 1,
            Left = 2,
        }

        //// smooth axis - default UI move handlers do things in steps, meaning you can smooth scroll a slider or scrollbar
        //// with axis control. This option allows setting value of scrollbar/slider directly as opposed to using move handler
        //// to avoid this
        //bool useSmoothAxis = true;

        //// multiplier controls how fast slider/scrollbar moves with respect to input axis value
        //float smoothAxisMultiplier = 0.01f;
        //// if useSmoothAxis is off, this next field controls how many steps per second are done when axis is on
        //float steppedAxisStepsPerSecond = 10f;

        // ignore input when looking away from all UI elements
        // useful if you want to use buttons/axis for other controls
        bool ignoreInputsWhenLookAway = true;

        // deselect when looking away from all UI elements
        // useful if you want to use axis for other controls
        bool deselectWhenLookAway = false;

        // useLookDrag allows you to use look-based drag and drop (see example)
        // and also drag sliders/scrollbars based on where you are looking
        // only works if usePointerMethod is true
        bool useLookDrag = true;
        bool useLookDragSlider = true;
        bool useLookDragScrollbar = false;
#endregion

#region INTERNAL VARIABLES
        private CurvedUIPointerEventData ControllerData;
        private GameObject currentPointedAt;
        private GameObject currentPressed;
        private GameObject currentDragging;
        private float nextAxisActionTime;
#endregion

#region LIFECYCLE
        protected override void Start()
        {
            base.Start();

            SetupControllers();
        }

        #endregion
        #region EVENT PROCESSING
        /// <summary>
        /// Process is called by UI system to process events 
        /// </summary>
        public override void Process()
        {
            instance = this;

            switch (EventController)
            {
                case ActiveViveController.Right:
                {
                    //in case only one controller is turned on, it will still be used to call events.
                    if (controllerManager.right.activeInHierarchy)
                        ProcessController(controllerManager.right);
                    else if (controllerManager.left.activeInHierarchy)
                        ProcessController(controllerManager.left);
                    break;
                }
                case ActiveViveController.Left:
                {
                    //in case only one controller is turned on, it will still be used to call events.
                    if (controllerManager.left.activeInHierarchy)
                        ProcessController(controllerManager.left);
                    else if (controllerManager.right.activeInHierarchy)
                        ProcessController(controllerManager.right);
                    break;
                }
                case ActiveViveController.Both:
                {
                        ProcessController(controllerManager.left);
                        ProcessController(controllerManager.right);
                    break;
                }
                default: goto case ActiveViveController.Right;
            }          
        }

        /// <summary>
        /// Processes Events from given controller.
        /// </summary>
        /// <param name="myController"></param>
        void ProcessController(GameObject myController)
        {
            //do not process events from this controller if it's off or not visible by base stations.
            if (!myController.gameObject.activeInHierarchy) return;

            CurvedUIViveController myControllerAssitant = myController.GetComponent<CurvedUIViveController>();
            if(myControllerAssitant == null)
            {
                myControllerAssitant = myController.AddComponent<CurvedUIViveController>();
            }

            // send update events if there is a selected object - this is important for InputField to receive keyboard events
            SendUpdateEventToSelectedObject();

            // see if there is a UI element that is currently being pointed at
            PointerEventData ControllerData = GetControllerPointerData(myController);
            currentPointedAt = ControllerData.pointerCurrentRaycast.gameObject;

            // deselect when pointed away.
            if (deselectWhenLookAway && currentPointedAt == null)
            {
                ClearSelection();
            }

            // handle enter and exit events (highlight)
            // using the function that is already defined in BaseInputModule
            HandlePointerExitAndEnter(ControllerData, currentPointedAt);

            if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentPointedAt != null)
            {
                // button down handling
                _buttonUsed = false;
                //if (Input.GetButtonDown(submitButtonName))
                if(myControllerAssitant.IsTriggerDown)
                {
                    ClearSelection();
                    ControllerData.pressPosition = ControllerData.position;
                    ControllerData.pointerPressRaycast = ControllerData.pointerCurrentRaycast;
                    ControllerData.pointerPress = null;
                    if (currentPointedAt != null)
                    {
                        currentPressed = currentPointedAt;
                        GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, ControllerData, ExecuteEvents.pointerDownHandler);

                        if (newPressed == null)
                        {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, ControllerData, ExecuteEvents.pointerClickHandler);
                            if (newPressed != null)
                            {
                                currentPressed = newPressed;
                            }
                        }
                        else {
                            currentPressed = newPressed;
                            // we want to do click on button down at same time, unlike regular mouse processing
                            // which does click when mouse goes up over same object it went down on
                            // reason to do this is that holding a controller might be jittery and this makes it easier to click buttons
                            ExecuteEvents.Execute(newPressed, ControllerData, ExecuteEvents.pointerClickHandler);
                        }

                        if (useLookDrag)
                        {
                            bool useLookTest = true;
                            if (!useLookDragSlider && currentPressed.GetComponent<Slider>())
                            {
                                useLookTest = false;
                            }
                            else if (!useLookDragScrollbar && currentPressed.GetComponent<Scrollbar>())
                            {
                                useLookTest = false;
                                // the following is for scrollbars to work right
                                // apparently they go into an odd drag mode when pointerDownHandler is called
                                // a begin/end drag fixes that
                                if (ExecuteEvents.Execute(currentPressed, ControllerData, ExecuteEvents.beginDragHandler))
                                {
                                    ExecuteEvents.Execute(currentPressed, ControllerData, ExecuteEvents.endDragHandler);
                                }
                            }
                            if (useLookTest)
                            {
                                ExecuteEvents.Execute(currentPressed, ControllerData, ExecuteEvents.beginDragHandler);
                                ControllerData.pointerDrag = currentPressed;
                                currentDragging = currentPressed;
                            }
                        }
                        else if (currentPressed.GetComponent<Scrollbar>())
                        {
                            // the following is for scrollbars to work right
                            // apparently they go into an odd drag mode when pointerDownHandler is called
                            // a begin/end drag fixes that
                            if (ExecuteEvents.Execute(currentPressed, ControllerData, ExecuteEvents.beginDragHandler))
                            {
                                ExecuteEvents.Execute(currentPressed, ControllerData, ExecuteEvents.endDragHandler);
                            }
                        }
                    }
                }
            }

            // have to handle button up even if looking away
            if (myControllerAssitant.IsTriggerUp)
            {
                if (currentDragging)
                {
                    ExecuteEvents.Execute(currentDragging, ControllerData, ExecuteEvents.endDragHandler);
                    if (currentPointedAt != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(currentPointedAt, ControllerData, ExecuteEvents.dropHandler);
                    }
                    ControllerData.pointerDrag = null;
                    currentDragging = null;
                }
                if (currentPressed)
                {
                    ExecuteEvents.Execute(currentPressed, ControllerData, ExecuteEvents.pointerUpHandler);
                    ControllerData.rawPointerPress = null;
                    ControllerData.pointerPress = null;
                    currentPressed = null;
                }
            }

            // drag handling
            if (currentDragging != null)
            {
                ExecuteEvents.Execute(currentDragging, ControllerData, ExecuteEvents.dragHandler);
            }

            // Touchpad axis handling. This is still WIP and does not work for Vive Controller
            //if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentPointedAt != null)
            //{
            //    _controlAxisUsed = false;
            //    if (eventSystem.currentSelectedGameObject && controlAxisName != null && controlAxisName != "")
            //    {
            //        float newVal = Input.GetAxis(controlAxisName);
            //        if (newVal > 0.01f || newVal < -0.01f)
            //        {
            //            if (useSmoothAxis)
            //            {
            //                Slider sl = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
            //                if (sl != null)
            //                {
            //                    float mult = sl.maxValue - sl.minValue;
            //                    sl.value += newVal * smoothAxisMultiplier * mult;
            //                    _controlAxisUsed = true;
            //                }
            //                else {
            //                    Scrollbar sb = eventSystem.currentSelectedGameObject.GetComponent<Scrollbar>();
            //                    if (sb != null)
            //                    {
            //                        sb.value += newVal * smoothAxisMultiplier;
            //                        _controlAxisUsed = true;
            //                    }
            //                }
            //            }
            //            else {
            //                _controlAxisUsed = true;
            //                float time = Time.unscaledTime;
            //                if (time > nextAxisActionTime)
            //                {
            //                    nextAxisActionTime = time + 1f / steppedAxisStepsPerSecond;
            //                    AxisEventData axisData = GetAxisEventData(newVal, 0.0f, 0.0f);
            //                    if (!ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler))
            //                    {
            //                        _controlAxisUsed = false;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
        #endregion

        #region PRIVATE FUNCTIONS
        /// <summary>
        /// Create a pointerEventData that saves the controller associated with it.
        /// </summary>
        private CurvedUIPointerEventData GetControllerPointerData(GameObject controller)
        {
            if (ControllerData == null)
                ControllerData = new CurvedUIPointerEventData(eventSystem);

            ControllerData.Reset();
            ControllerData.delta = Vector2.zero;
            ControllerData.position = Vector2.zero; // this will be overriden by raycaster
            ControllerData.Controller = controller; // raycaster will use this object to override pointer position on screen. Keep it safe.
            ControllerData.scrollDelta = Vector2.zero;

            eventSystem.RaycastAll(ControllerData, m_RaycastResultCache); //Raycast all the things!. Position will be overridden here by CurvedUIRaycaster

            //GEt a current raycast to find if we're pointing at GUI object. 
            ControllerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            _guiRaycastHit = (ControllerData.pointerCurrentRaycast.gameObject != null ? true : false);
            m_RaycastResultCache.Clear();

            return ControllerData;
        }


        /// <summary>
        /// send update event to selected object
        /// needed for InputField to receive keyboard input
        /// </summary>
        /// <returns></returns>
        private bool SendUpdateEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;
            BaseEventData data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Force selection of a gameobject.
        /// </summary>
        private void Select(GameObject go)
        {
            ClearSelection();
            if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
            {
                eventSystem.SetSelectedGameObject(go);
            }
        }

        /// <summary>
        /// Adds necessary components to Vive controller gameobjects. These will let us know what inputs are used on them.
        /// </summary>
        private void SetupControllers()
        {
            foreach (GameObject cont in new List<GameObject>() { controllerManager.left, controllerManager.right})
            {
                if (cont.GetComponent<CurvedUIViveController>() == null)
                {
                    cont.AddComponent<CurvedUIViveController>();
                }
            }
        }
        #endregion


        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Clear the current selection
        /// </summary>
        public void ClearSelection()
        {
            if (eventSystem.currentSelectedGameObject)
            {
                eventSystem.SetSelectedGameObject(null);
            }
        }
        #endregion

        #region SETTERS AND GETTERS
        /// <summary>
        /// guiRaycastHit is helpful if you have other places you want to use look input outside of UI system
        /// you can use this to tell if the UI raycaster hit a UI element
        /// </summary>
        public bool guiRaycastHit {
            get
            {
                return _guiRaycastHit;
            }
        }
        private bool _guiRaycastHit;

        ///// <summary>
        ///// controlAxisUsed is helpful if you use same axis elsewhere
        ///// you can use this boolean to see if the UI used the axis control or not
        ///// if something is selected and takes move event, then this will be set
        ///// </summary>
        //public bool controlAxisUsed {
        //    get
        //    {
        //        return _controlAxisUsed;
        //    }
        //}
        //private bool _controlAxisUsed;

        /// <summary>
        /// buttonUsed is helpful if you use same button elsewhere
        /// you can use this boolean to see if the UI used the button press or not
        /// </summary>
        public bool buttonUsed {
            get
            {
                return _buttonUsed;
            }
        }
        private bool _buttonUsed;

        
        /// <summary>
        /// Get or Set controller manager used by this input module.
        /// </summary>
        public SteamVR_ControllerManager ControllerManager {
            get { return controllerManager; }
            set
            {
                controllerManager = value;
                SetupControllers();
            }
        }
        SteamVR_ControllerManager controllerManager;


        public static CurvedUIViveInputModule Instance {
            get
            {
                return instance;
            }
        }
        private static CurvedUIViveInputModule instance;
        #endregion
    }
}

#else

namespace CurvedUI
{

    public class CurvedUIViveInputModule : BaseInputModule
    {
        public override void Process() { }
    }

}
#endif 

