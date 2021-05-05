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
        const int _nInitialStones = 4;
        private int? _nextBotMove = 0;
        private Timer _timer;

        public string PlayerMoveString => $"Kolej gracza {_state.Player + 1}";
        public ReactiveCommand<string, Unit> OnClickCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OnClickStartCommand { get; }
        public bool AivsAi { get; set; }
        public State State
        {
            get => _state;
            set => _state = this.RaiseAndSetIfChanged(ref _state, value);
        }

        private Algorithm _selectedAlgorithm;

        public MankalaViewModel()
        {
            _state = new State
            {
                HolesState = new int[6].Select(_ => new[] { _nInitialStones, _nInitialStones }.ToImmutableArray()).ToImmutableArray(),
                Wells = new[]{0,0}.ToImmutableArray()
            };
            OnClickStartCommand = ReactiveCommand.Create<Unit, Unit>(StartButtonClick);
        }

        //TODO: Implement sensibly
        private void RefreshButtonState()
        {

        }

        public Unit StartButtonClick(Unit prop)
        {
            if (AivsAi)
            {
                _timer = new Timer(_ => PlayRoundAivsAi(), null, 1000, 100);
            }
            else
            {
                OnClickCommand = ReactiveCommand.Create<string, Unit>(OnButtonClick);
            }
            return new();
        }

        private void PlayRoundAivsAi()
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

            _nextBotMove = _selectedAlgorithm.GetMove(State, 6,true);
            Debugger.Log(3, "Recursion", _nextBotMove.ToString());
            if (_nextBotMove == null)
            {
                State.GameOver = true;
                _timer.Dispose();
                return;
            }
            var move = new Move
            {
                OldState = State,
                Selection = (_nextBotMove.Value, 0)
            };
            State = GameEngine.GameEngine.MakeMove(move);

            _nextBotMove = _selectedAlgorithm.GetMove(State, 6, false);
            if (_nextBotMove == null)
            {
                State.GameOver = true;
                _timer.Dispose();
                return;
            }
            move = new Move
            {
                OldState = State,
                Selection = (_nextBotMove.Value, 1)
            };
            State = GameEngine.GameEngine.MakeMove(move);

        }

        public Unit OnButtonClick(string buttonName)
        {
            return new ();
        }
    }
}
