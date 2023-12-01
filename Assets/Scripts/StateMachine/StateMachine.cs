using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
    // 各State毎のdelegateを登録しておくクラス
    public class StateMapping
    {
        public Action onEnter;
        public Action onExit;
        public Action<float> onUpdate;
    }

    public class Transition<TState, TTrigger>
    {
        public TState To { get; set; }
        public TTrigger Trigger { get; set; }
    }

    public class StateMachine<TState, TTrigger>
        where TState : struct, IConvertible, IComparable
        where TTrigger : struct, IConvertible, IComparable
    {
        private TState _stateType;
        private StateMapping _stateMapping;
        public bool _isChangingState = false;
    
        private Dictionary<object, StateMapping> _stateMappings = new Dictionary<object, StateMapping>();
        private Dictionary<TState, List<Transition<TState, TTrigger>>> _transitionLists = new Dictionary<TState, List<Transition<TState, TTrigger>>>();
    
        public StateMachine()
        {
            // StateからStateMappingを作成
            var enumValues  = Enum.GetValues(typeof(TState));
            for (int i = 0; i < enumValues.Length; i++)
            {
                var mapping = new StateMapping();
                _stateMappings.Add(enumValues.GetValue(i), mapping);
            }
        
        }

        /// <summary>
        /// Stateを初期化する
        /// </summary>
        public void SetupState(TState state, Action onEnter, Action onExit, Action<float> onUpdate)
        {
            var stateMapping = _stateMappings[state];
            stateMapping.onEnter = onEnter;
            stateMapping.onExit = onExit;
            stateMapping.onUpdate = onUpdate;
        }

        /// <summary>
        /// 更新する
        /// </summary>
        public void Update(float deltaTime)
        {
            if (_stateMapping != null && _stateMapping.onUpdate != null) {
                _stateMapping.onUpdate(deltaTime);
            }
        }

        /// <summary>
        /// Stateを直接変更する
        /// </summary>
        public void ChangeState(TState to)
        {
            if (_isChangingState) return;
        
            // OnExit
            if (_stateMapping != null && _stateMapping.onExit != null) {
                _stateMapping.onExit();
                _isChangingState = true;
            }

            // OnEnter
            _stateType = to;
            _stateMapping = _stateMappings[to];
            if (_stateMapping.onEnter != null) {
                _stateMapping.onEnter();
            }
        }
    }
}