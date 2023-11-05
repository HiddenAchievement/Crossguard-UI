using UnityEngine;
using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public static class CrossExecuteEvents
    {
        public static ExecuteEvents.EventFunction<IMoveEndHandler> MoveEndHandler => s_moveEndHandler;

        private static readonly ExecuteEvents.EventFunction<IMoveEndHandler> s_moveEndHandler = Execute;

        private static void Execute(IMoveEndHandler handler, BaseEventData eventData)
        {
            handler.OnMoveEnd(eventData);
        }
        
        public static ExecuteEvents.EventFunction<IPrevFieldHandler> PrevFieldHandler => s_prevFieldHandler;

        private static readonly ExecuteEvents.EventFunction<IPrevFieldHandler> s_prevFieldHandler = Execute;

        private static void Execute(IPrevFieldHandler handler, BaseEventData eventData)
        {
            handler.OnPrevField(eventData);
        }
        
        public static ExecuteEvents.EventFunction<INextFieldHandler> NextFieldHandler => s_nextFieldHandler;

        private static readonly ExecuteEvents.EventFunction<INextFieldHandler> s_nextFieldHandler = Execute;

        private static void Execute(INextFieldHandler handler, BaseEventData eventData)
        {
            handler.OnNextField(eventData);
        }
        

    }
}
