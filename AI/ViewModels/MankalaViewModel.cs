using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading;
using AI.GameEngine;
using AI.Models;
using ReactiveUI;

namespace AI.ViewModels
{
    public class MankalaViewModel : ReactiveObject
    {

        private State _state;
        private int _nInitialStones = 4;
        private int? nextBotMove = 0;
        private Timer timer;
        public string PlayerMoveString => $"Kolej gracza {_state.Player + 1}";
        public ReactiveCommand<string, Unit> OnClickCommand { get; }
        public State State
        {
            get => _state;
            set => _state = this.RaiseAndSetIfChanged(ref _state, value);
        }
        


        public MankalaViewModel()
        {
            _state = new State
            {
                HolesState = new int[6].Select(_ => new[] { _nInitialStones, _nInitialStones }.ToImmutableArray()).ToImmutableArray(),
                Wells = new[]{0,0}.ToImmutableArray()
            };
            OnClickCommand = ReactiveCommand.Create<string,Unit>(OnButtonClick);
            timer = new Timer(_ => PlayRound(), null, 1000, 100);
        }

        //TODO: Implement sensibly
        private void RefreshButtonState()
        {

        }

        private void PlayRound()
        {
            if (State.GameOver)
            {
                var sum = 0;
                for (var col = 0; col < 6; col++)
                {
                    sum += State.HolesState[col][0];
                    sum += State.HolesState[col][1];
                }

                return;
            }

            nextBotMove = MinMax.DoMinMax(State, 6,true);
            Debugger.Log(3, "Recursion", nextBotMove.ToString());
            if (nextBotMove == null)
            {
                State.GameOver = true;
                timer.Dispose();
                return;
            }
            var move = new Move
            {
                OldState = State,
                Selection = (nextBotMove.Value, 0)
            };
            State = GameEngine.GameEngine.MakeMove(move);

            nextBotMove = MinMax.DoMinMax(State, 6, false);
            if (nextBotMove == null)
            {
                State.GameOver = true;
                timer.Dispose();
                return;
            }
            move = new Move
            {
                OldState = State,
                Selection = (nextBotMove.Value, 1)
            };
            State = GameEngine.GameEngine.MakeMove(move);

        }

        public Unit OnButtonClick(string buttonName)
        {
            return new ();
        }
    }
}
