using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Match3.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {

        [RelayCommand]
        async Task StartGame()
        {
            await Shell.Current.GoToAsync(nameof(GamePage));

            GameManager.StartGame();
        }
    }
}
