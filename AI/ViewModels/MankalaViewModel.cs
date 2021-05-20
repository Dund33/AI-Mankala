using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading;
using AI.Algorithms;
using AI.GameEngine;
using AI.Models;
using ReactiveUI;

namespace AI.ViewModels
{
    public class MankalaViewModel : ReactiveObject
    {
        private State _state;
        private const int NInitialStones = 4;
        public int Depth { get; set; } = 4;
        private int? _nextBotMove = 0;
        private Timer? _timer;
        private bool _abSelected;
        private bool _heuristicEnabled;
        private Algorithm? _selectedAlgorithm;
        private bool _disablePlayerButtons = true;
        private List<long> _moveTimesPlayer1 = new();
        private List<long> _moveTimesPlayer2 = new();
        private int _moves = 0;

        public bool DisablePlayerButtons
        {
            get => _disablePlayerButtons;
            set => this.RaiseAndSetIfChanged(ref _disablePlayerButtons, value);
        }

        public string TopText { get; set; } = "Wybierz tryb i naciśnij start";
        public ReactiveCommand<string, Unit> OnClickCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OnClickStartCommand { get; }
        public ReactiveCommand<bool, Unit> OnABSelected { get; }
        public ReactiveCommand<Unit,Unit> OnHeuristicSelected { get; set; }

        public bool AIvsAI { get; set; }

        public State State
        {
            get => _state;
            set => _state = this.RaiseAndSetIfChanged(ref _state, value);
        }

        public int Moves
        {
            get => _moves;
            set => _moves = this.RaiseAndSetIfChanged(ref _moves, value);
        }


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
            OnHeuristicSelected = ReactiveCommand.Create<Unit,Unit>(b =>
            {
                _heuristicEnabled = !_heuristicEnabled;
                return new Unit();
            });
        }

        //TODO: Implement sensibly
        private void RefreshButtonState()
        {
        }

        public Unit StartButtonClick(Unit prop)
        {
            TopText = "";
            this.RaisePropertyChanged("TopText");
            if (_abSelected)
                if (_heuristicEnabled)
                    _selectedAlgorithm = new ABCutsHeuristic();
                else 
                    _selectedAlgorithm = new ABCuts();
            else if (_heuristicEnabled)
                _selectedAlgorithm = new MinMaxHeuristic();
            else
                _selectedAlgorithm = new MinMax();

            if (AIvsAI)
                _timer = new Timer(_ => PlayRoundAivsAi(), null, 1000, 500);
            else
                DisablePlayerButtons = false;
            
            return new Unit();
        }

        private void PlayRoundAivsAi()
        {
            var stopwatch = new Stopwatch();
            if (State.GameOver)
            {
                var av1 = _moveTimesPlayer1.Average();
                var av2 = _moveTimesPlayer2.Average();
                var correctAv = State.Player == 0 ? av1 : av2;
                TopText = $"Average move time {correctAv}";
                this.RaisePropertyChanged("TopText");
                Console.WriteLine(_moves);
                return;
            }
            
            stopwatch.Start();
            _nextBotMove = _selectedAlgorithm?.GetMove(State, Depth, true);
            stopwatch.Stop();
            Moves++;
            _moveTimesPlayer1.Add(stopwatch.ElapsedMilliseconds);
            if (_nextBotMove == null)
            {
                GameOver(_moveTimesPlayer2.Average());
                return;
            }

            var move = new Move
            {
                OldState = State,
                Selection = (_nextBotMove.Value, 0)
            };
            State = GameEngine.GameEngine.MakeMove(move);
            stopwatch.Reset();
            stopwatch.Start();
            _nextBotMove = _selectedAlgorithm?.GetMove(State, Depth, false);
            stopwatch.Stop();
            _moveTimesPlayer2.Add(stopwatch.ElapsedMilliseconds);
            if (_nextBotMove == null)
            {
                GameOver(_moveTimesPlayer1.Average());
                return;
            }

            move = new Move
            {
                OldState = State,
                Selection = (_nextBotMove.Value, 1)
            };
            State = GameEngine.GameEngine.MakeMove(move);
        }

        private void GameOver(double moveTime)
        {
            DisablePlayerButtons = true;
            State.GameOver = true;
            _timer?.Dispose();
            TopText = $"Average move time {moveTime}";
            this.RaisePropertyChanged("TopText");
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
            RefreshButtonState();

            if (State.GameOver)
            {
                DisablePlayerButtons = true;
                return new Unit();
            }

            _nextBotMove = _selectedAlgorithm?.GetMove(State, 2, false);
            if (_nextBotMove == null)
            {
                DisablePlayerButtons = true;
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
            TopText = "Game Over";
            this.RaisePropertyChanged("TopText");
            DisablePlayerButtons = true;
            return new Unit();
        }
    }
}