using MVVMHeplers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMCore.Utils
{
    public class BaseMachine<State, Trigger> 
    {
        [DebuggerDisplay(" -> {To}")]
        protected class StatePermission<State>
        {
            public State To { get; set; }
            public Func<bool> Predicate { get; set; }
            public StatePermission()
            {
                To = default(State);
                Predicate = () => false;
            }

            public StatePermission(State s,Func<bool> pred)
            {
                To = default(State);
                Predicate = pred;
            }

            public static StatePermission<State> Allow(State t) => new StatePermission<State>() { To = t, Predicate = () => true };
            public static StatePermission<State> Ignore(State t) => new StatePermission<State>() { To = t, Predicate = () => false };

            public override string ToString()
            {
                return $"tranitions to: {To}";
            }
        } 

        [DebuggerDisplay("{Id} -> {Flows}")]
        protected class StateContainer
        {
            public Action OnEnter { get; set; }
            public Action OnExit { get; set; }
            public Dictionary<Trigger, StatePermission<State>> Flows { get; set; }
            public Dictionary<State,Action> EnterFrom { get; set; }
            public Dictionary<State,Action> ExitFrom { get; set; }
            public Dictionary<Trigger, TrigWithParamContainer<Trigger>> TriggerParams;
            public State Id;

            public StateContainer()
            {
                Flows = new Dictionary<Trigger, StatePermission<State>>();
                EnterFrom = new Dictionary<State, Action>();
                ExitFrom = new Dictionary<State, Action>();
                OnEnter = null;
                OnExit = null;
                Id = default(State);
                TriggerParams = new Dictionary<Trigger, TrigWithParamContainer<Trigger>>();
            }

            public override string ToString()
            {
                return $"{Id}: {String.Join(",", Flows)}";
            }
        }

        protected Dictionary<State, StateContainer> BuiltStates;

        protected State CurrentState { get; set; }

        public BaseMachine<State,Trigger> Permit(Trigger t,State state)
        {
            if (Current.Flows.ContainsKey(t))
                throw new StateMachineException($"trying to add more triggers of {t} to state {state}");

            Current.Flows.Add(t, StatePermission<State>.Allow(state));

            return this;
        } 

        public class TriggerWithParam<Trigger, Param>
        {
            public Trigger MyTrigger { get; set; }
            public Param MyParam { get; set; }
        }

        public TriggerWithParam<Trigger,Parameter> SetTriggerWithParameter<Parameter>(Trigger t, Parameter p)
        {
            return new TriggerWithParam<Trigger, Parameter>(); 
        }


        public BaseMachine<State,Trigger> PermitIf(Trigger t,State state,Func<bool> predicate)
        {
            Current.Flows.Add(t, new StatePermission<State>(state, predicate));
            return this;
        }

        private StateContainer Current => BuiltStates[CurrentState];

        public BaseMachine<State,Trigger> OnEnter(Action onEnterAction)
        {
            if (Current.OnEnter == null)
                Current.OnEnter = onEnterAction;
            else
                throw new StateMachineException($"Already added on Enter for state {CurrentState}");

            return this;
        }

        protected class TrigWithParamContainer<Trigger>
        {
            public Type ParamType { get; set; }
            public Trigger T { get; set; }
            public Action<object> OnAction { get; set; }
        }
        public BaseMachine<State, Trigger> OnEntryFrom<Param>(TriggerWithParam<Trigger, Param> t, Action<Param> onEnter)
        {
            var tt = new TrigWithParamContainer<Trigger>() { ParamType = typeof(Param), OnAction = new Action<object>(o => onEnter((Param)o)), T = t.MyTrigger }; 
            Current.TriggerParams.Add(t.MyTrigger, tt);
            return this;
        }

        public BaseMachine<State,Trigger> OnEnterFrom(State t, Action onEnter)
        {
            Current.EnterFrom.Add(t, onEnter);
            return this;
        }

        public BaseMachine<State,Trigger> Ignore(Trigger t)
        {
            return this;
        }

        public State SelectedState { get => CurrentState; }
    }

    public class StateMachine<State,Trigger>  : BaseMachine<State,Trigger>
    {
        private Action<State> _onStatechanged;

        public BaseMachine<State,Trigger> Configure(State state)
        {
            CurrentState = state;
            BuiltStates = BuiltStates ?? new Dictionary<State, StateContainer>();
            BuiltStates.Add(state, new BaseMachine<State, Trigger>.StateContainer());
            return this;
        }

        public ICommand CreateTrigger(Trigger t)
        {
            return UICommandFactory.Create(
                x => { Fire(t); },
                () => CanFire(t)
                );

        }

        public void SetOnStateChanged(Action<State> actionDo)
        {
            _onStatechanged = actionDo;
        }
 
        public void SwitchState(State state)
        {
            if (BuiltStates.TryGetValue(state, out var statecontainer))
            {
                var exitAction = statecontainer.ExitFrom.ContainsKey(CurrentState) ? statecontainer.ExitFrom[CurrentState] : statecontainer.OnExit;
                exitAction?.Invoke();
            }
            else
                throw new StateMachineException($"Tried switching to unconfigured state {state}");

            if(BuiltStates.TryGetValue(state,out var stateContainer))
            {
                var enterAction = statecontainer.EnterFrom.ContainsKey(CurrentState) ? statecontainer.EnterFrom[CurrentState] : statecontainer.OnEnter;
                enterAction?.Invoke(); 
            }
            CurrentState = state;

            _onStatechanged?.Invoke(CurrentState);
        }

        public void Fire<Param>(Trigger trigger, Param parametere)
        { 
            if(BuiltStates.TryGetValue(CurrentState, out var state))
            {
                if(state.Flows.TryGetValue(trigger,out var toState))
                {
                    if(state.TriggerParams.TryGetValue(trigger, out var trigWithParam))
                    {
                        trigWithParam.OnAction(parametere);
                    }

                    SwitchState(toState.To); 
                } 
            }
        }

        public void Fire(Trigger trigger)
        {
            if(BuiltStates.TryGetValue(CurrentState,out var state))
            {
                if(state.Flows.TryGetValue(trigger,out var toState))
                {
                    SwitchState(toState.To);
                }
            }
        }

        public bool CanFire(Trigger trigger)
        {
            if(BuiltStates.TryGetValue(CurrentState,out var state))
            {
                return state.Flows.ContainsKey(trigger);
            }

            return false;
        }

        public string StrinifyState(State state)
        { 
            var built = BuiltStates[state]; 
            return "";
        }
        public void Print()
        { 
            //foreach(var stateKey in BuiltStates.Keys)
            //{
            //    var state = BuiltStates[stateKey];
            //    Debug.WriteLine(state.Id);
            //    Debug.WriteLine("Transitions to:");
            //    foreach(var trans in state.Flows.Keys)
            //    {
            //        var transition = state.Flows[trans];
            //        Debug.WriteLine(transition.To);

            //    }
            //}
            foreach(var stateKey in BuiltStates.Keys)
            {
                var state = BuiltStates[stateKey];
                Debug.WriteLine(state.Id);

                var leafs = state.Flows.Keys.Where(x => BuiltStates[state.Flows[x].To].Flows.Count == 1).Select(x => state.Flows[x].To).ToList();
                foreach(var leaf in leafs )
                {
                    Debug.WriteLine($"{state.Id} -> {leaf}");
                }

                var children = state.Flows.Keys;
                var oneways = new List<StateContainer>();
                foreach(var child in children)
                {
                    var childstate = BuiltStates[ state.Flows[child].To];
                    var singleway = childstate.Flows.Count <= 1;
                    if (singleway)
                        oneways.Add(childstate);
                }

                foreach(var trans in state.Flows.Keys)
                {
                    var transition = state.Flows[trans];
                    Debug.WriteLine(transition.To);

                }
            }
        } 
    }

    public class ExposedMachine<State,Trigger> : StateMachine<State,Trigger>
    {

    }
}
