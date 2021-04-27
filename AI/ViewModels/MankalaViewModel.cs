using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using AI.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;

namespace AI.ViewModels
{
    public class MankalaViewModel : ViewModelBase
    {
        public int[][] State { get; set; }
        public ReactiveCommand<string, Unit> OnClickCommand { get; }

        public MankalaViewModel()
        {
            State = new int[6][];
            Array.Fill(State, new[] { 3, 3 });
            OnClickCommand = ReactiveCommand.Create<string,Unit>(buttonName => OnButtonClick(buttonName));
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
            return new Unit();
        }
    }
}
