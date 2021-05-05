using System;
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
        private const int NInitialStones = 4;
        private int? _nextBotMove = 0;
        private Timer _timer;
        private bool _aivsai;
        private bool _abSelected;

        private bool _disableAllButtons = true;

        public bool DisableAllButtons
        {
            get => _disableAllButtons;
            set => this.RaiseAndSetIfChanged(ref _disableAllButtons, value);
        }

        public string PlayerMoveString => $"Kolej gracza {_state.Player + 1}";
        public ReactiveCommand<string, Unit> OnClickCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OnClickStartCommand { get; }
        public ReactiveCommand<bool, Unit> OnABSelected { get; }

        public bool AivsAi
        {
            get => _aivsai;
            set => _aivsai = this.RaiseAndSetIfChanged(ref _aivsai, value);
        }

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
                HolesState = new int[6].Select(_ => new[] {NInitialStones, NInitialStones}.ToImmutableArray())
                    .ToImmutableArray(),
                Wells = new[] {0, 0}.ToImmutableArray()
            };
            OnClickStartCommand = ReactiveCommand.Create<Unit, Unit>(StartButtonClick);
            OnClickCommand = ReactiveCommand.Create<string, Unit>(OnButtonClick);
            OnABSelected = ReactiveCommand.Create<bool, Unit>(b =>
            {
                _abSelected = !_abSelected;
                return new Unit();
            });
        }

        //TODO: Implement sensibly
        private void RefreshButtonState()
        {
        }

        public Unit StartButtonClick(Unit prop)
        {
            if (_abSelected)
                _selectedAlgorithm = new ABCuts();
            else
                _selectedAlgorithm = new MinMax();
            if (AivsAi)
                _timer = new Timer(_ => PlayRoundAivsAi(), null, 1000, 100);
            else
                DisableAllButtons = false;
            
            return new Unit();
        }

        private void PlayRoundAivsAi()
        {
            if (State.GameOver) return;

            _nextBotMove = _selectedAlgorithm.GetMove(State, 6, true);
            Debugger.Log(3, "Recursion", _nextBotMove.ToString());
            if (_nextBotMove == null)
            {
                DisableAllButtons = true;
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
                DisableAllButtons = true;
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
            var choice = buttonName switch
            {
                "B1" => (0, 0),
                "B2" => (1, 0),
                "B3" => (2, 0),
                "B4" => (3, 0),
                "B5" => (4, 0),
                "B6" => (5, 0),
                "B7" => (0, 1),
                "B8" => (1, 1),
                "B9" => (2, 1),
                "B10" => (3, 1),
                "B11" => (4, 1),
                "B12" => (5, 1),
                _ => throw new NotImplementedException()
            };

            var move = new Move
            {
                OldState = State,
                Selection = choice
            };

            var isValid = GameEngine.GameEngine.IsValidMove(move);

            if (isValid)
                State = GameEngine.GameEngine.MakeMove(move);
            this.RaisePropertyChanged("PlayerMoveString");
            RefreshButtonState();

            if (State.GameOver)
            {
                DisableAllButtons = true;
                return new Unit();
            }

            _nextBotMove = _selectedAlgorithm.GetMove(State, 6, false);
            if (_nextBotMove == null)
            {
                DisableAllButtons = true;
                State.GameOver = true;
                return new Unit();
            }


            move = new Move
            {
                OldState = State,
                Selection = (_nextBotMove.Value, 1)
            };

            State = GameEngine.GameEngine.MakeMove(move);

            if (!State.GameOver) return new Unit();

            DisableAllButtons = true;
            return new Unit();
        }
    }
}