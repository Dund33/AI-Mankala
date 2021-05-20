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
#endif
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
                    ((MankalaViewModel)DataContext).AIvsAI = ((ComboBoxItem)comboBox.SelectedItem)?.Content as string == "AIvAI";
                };

                ((MankalaViewModel)DataContext).WhenAnyValue(x => x.DisablePlayerButtons).Subscribe(x =>
                {
                    this.GetVisualDescendants()
                        .OfType<Button>()
                        .Take(6)
                        .ToList()
                        .ForEach(button => button.IsEnabled = !x);
                });

                this.Find<TextBox>("Tb")
                    .WhenValueChanged(x => x.Text).Subscribe(next =>
                    {
                        if (string.IsNullOrWhiteSpace(next)) return;

                        if (int.TryParse(next, out var outInt))
                            ((MankalaViewModel) DataContext).Depth = outInt;
                    });
            };

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
