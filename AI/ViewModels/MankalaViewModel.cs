using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using AI.Models;
using ReactiveUI;

namespace AI.ViewModels
{
    public class MankalaViewModel : ReactiveObject
    {

        private State _state;
        private int _nInitialStones = 4;
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
        }

        //TODO: Implement sensibly
        private void RefreshButtonState()
        {

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
            return new Unit();
        }
    }
}
