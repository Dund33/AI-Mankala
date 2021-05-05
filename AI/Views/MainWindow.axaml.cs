using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using DynamicData.Binding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;

namespace AI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();

            DataContextChanged += (obj, args) =>
            {
                var comboBox = this.Find<ComboBox>("ModeSelection");
                if (comboBox == null)
                    throw new NullReferenceException();
                if (DataContext == null)
                    throw new NullReferenceException();
                comboBox.SelectionChanged += (sender, eventArgs) =>
                {
                    Debugger.Log(3, "Button", comboBox.SelectedItem as string);
                    ((MankalaViewModel)DataContext).AivsAi = ((ComboBoxItem)comboBox.SelectedItem).Content as string == "AIvAI";
                };

                ((MankalaViewModel) DataContext).WhenAnyValue(x => x.AivsAi).Subscribe(x=>
                {
                    this.GetVisualDescendants()
                        .OfType<Button>()
                        .Take(6)
                        .ToList()
                        .ForEach(button => button.IsEnabled = !x);
                });

                ((MankalaViewModel)DataContext).WhenAnyValue(x => x.DisableAllButtons).Subscribe(x =>
                {
                    this.GetVisualDescendants()
                        .OfType<Button>()
                        .Take(12)
                        .ToList()
                        .ForEach(button => button.IsEnabled = !x);
                });
            };
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
